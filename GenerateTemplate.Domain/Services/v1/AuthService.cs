namespace GenerateTemplate.Domain.Services.v1;

using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Enum;
using GenerateTemplate.Domain.Exceptions;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Domain.Interface.Services;
using GenerateTemplate.Domain.Interface.Services.v1;
using GenerateTemplate.Domain.Interface.Utils;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Collections.Generic;

public class AuthService : IAuthService
{
    private readonly IAuthDao _authDao;
    private readonly IGenerateHash _generateHash;
    private readonly HttpContext _httpContext;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IEmailService _emailService;
    private readonly IGetClientIdToken _getClientIdToken;
    private readonly IRedisService _redisService;
    private readonly INotificationBase _notificationBase;

    public AuthService(
        IAuthDao authDao,
        IGenerateHash generateHash,
        IEmailService emailService,
        IMemoryCacheService memoryCacheService,
        IHttpContextAccessor httpContextAccessor,
        IGetClientIdToken getClientIdToken,
        IRedisService redisService,
        INotificationBase notificationBase
        )
    {
        _authDao = authDao;
        _generateHash = generateHash;
        _emailService = emailService;
        _memoryCacheService = memoryCacheService;
        _getClientIdToken = getClientIdToken;
        _httpContext = httpContextAccessor.HttpContext!;
        _redisService = redisService;
        _notificationBase = notificationBase;
    }

    public async Task<OperationResult<IEnumerable<UserModel>>> GetAllAsync(int page, int pageSize)
    {
        try
        {
            IEnumerable<UserModel> GetAll = await _authDao.GetAllAsync(page, pageSize,
                filter: Builders<UserModel>.Filter.Where(x => x.AccountStatus == AccountStatus.Active));

            if (GetAll.Count() == 0)
            {
                return ResponseObject(GetAll, "Não há usuários cadastrados.", StatusCodes.Status404NotFound, false);
            }

            return ResponseObject(GetAll, "Usuários encontrados.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(Enumerable.Empty<UserModel>(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<UserModel>> GetIdAsync(string Id)
    {
        try
        {
            var GetUserCacheId = await _redisService.GetAsync<UserModel>(Id);

            if (GetUserCacheId is not null)
            {
                return ResponseObject(GetUserCacheId, "Usuário encontrado.", StatusCodes.Status200OK, false);
            }

            UserModel GetUserAsyncId = await _authDao.GetIdAsync(Id);

            if (GetUserAsyncId is null)
            {
                await _notificationBase.NotifyAsync("Usuário não encontrado.", $"Usuário com id: {Id} não existe.");
                return ResponseObject<UserModel>(GetUserAsyncId, $"Usuário com id: {Id} não existe.", StatusCodes.Status204NoContent, false);
            }

            if (GetUserAsyncId.AccountStatus == 0)
            {
                await _notificationBase.NotifyAsync("Usuário bloqueado", "Usuário permanece com a conta bloqueada");
                return ResponseObject(GetUserAsyncId, "Usuário bloqueado.", StatusCodes.Status204NoContent, false);
            }

            await _redisService.SetAsync(Id, GetUserAsyncId, TimeSpan.FromMinutes(5));
            return ResponseObject(GetUserAsyncId, "Usuário encontrado", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            await _notificationBase.NotifyAsync("Error internal", ex.Message);
            return ResponseObject<UserModel>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<UserModel>> CreateAsync(UserModel addObject)
    {
        try
        {
            UserModel findEmail = await _authDao.FindEmailAsync(addObject.Email);

            if (findEmail != null)
            {
                return ResponseObject<UserModel>(default!, $"Usuário com esse email: '{addObject.Email}', já existe.", StatusCodes.Status409Conflict, false);
            }

            addObject.Password = _generateHash.GenerateHashParameters(addObject.Password);
            addObject.AccountStatus = AccountStatus.Active;
            addObject.DateCreated = DateTime.UtcNow;

            await _authDao.CreateAsync(addObject);

            return ResponseObject(addObject, "Usuário criado com sucesso.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<UserModel>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<string>> LoginAsync(UserModel userLogin)
    {
        try
        {
            UserModel findUser = await _authDao.FindEmailAsync(userLogin.Email);

            if (findUser == null)
            {
                return ResponseObject<string>(default!, $"Este email: {userLogin.Email} não existe.", StatusCodes.Status409Conflict, false);
            }

            //Faz uma validação para verificar se a senha que o usuariop está passando corresponde a senha salva no banco, em formato hash
            bool isPasswordCorrect = _generateHash.VerifyPassword(userLogin.Password, findUser.Password);

            if (!isPasswordCorrect)
            {
                return ResponseObject<string>(default!, $"Senha incorreta.", StatusCodes.Status400BadRequest, false);
            }

            //Gera um token a partir do usuario buscado pelo E-mail
            string token = _generateHash.GenerateToken(findUser);

            return ResponseObject(token, "Usuário logado com sucesso.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(ex.ToString(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<UserModel>> RemoveAsync(string Id)
    {
        try
        {
            OperationResult<UserModel> userView = await GetIdAsync(Id);

            if (userView.Content == null)
                return ResponseObject(userView.Content!, userView.Message, userView.StatusCode, userView.Status);

            var updateFields = new Dictionary<string, object>
            {
                { nameof(AccountStatus), AccountStatus.Blocked },
            };

            UserModel updateUser = await _authDao.UpdateAsync(Id, updateFields);

            return ResponseObject(updateUser, "Usuário bloqueado.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<UserModel>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<string>> ForgetPasswordAsync(string email)
    {
        try
        {
            UserModel findEmail = await _authDao.FindEmailAsync(email);

            if (findEmail == null)
            {
                return ResponseObject<string>(default!, $"Este E-mail: {email} não é válido", StatusCodes.Status404NotFound, false);
            }

            string token = _generateHash.GenerateRandomNumber().ToString();

            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #0056b3;'>Redefinição da sua senha</h2>
                    <p>Olá,</p>
                    <p>Recebemos uma solicitação para redefinir sua senha. Use o código abaixo para verificar sua conta:</p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 24px; font-weight: bold; color: #0056b3;'>{token}</span>
                    </div>
                    <p style='color: #777;'>Se você não solicitou essa redefinição, ignore este email.</p>
                    <p>Atenciosamente,</p>
                    <p>Equipe de Suporte</p>
                </div>";

            await _emailService.SendMailAsync(
                  "no-reply@yourdomain.com", // Use a valid email address here
                  email,
                  "Redefinição da sua senha",
                  emailBody
               );

            _memoryCacheService.AddToCache(token, findEmail, 5);

            return ResponseObject<string>(token, $"Email enviado com sucesso!", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(ex.ToString(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<UserModel>> UpdateAsync(string Id, UserModel updateObject)
    {
        try
        {
            OperationResult<UserModel> userView = await GetIdAsync(Id);

            if (userView.Content == null)
                return ResponseObject(userView.Content!, userView.Message, userView.StatusCode, userView.Status);

            var updateFields = new Dictionary<string, object>
            {
                { nameof(updateObject.UserName), updateObject.UserName },
                { nameof(updateObject.Password), updateObject.Password },
                { nameof(updateObject.AccountStatus), updateObject.AccountStatus }
            };

            UserModel userUpdate = await _authDao.UpdateAsync(Id, updateFields);

            return ResponseObject(userUpdate, "Usuário atualizado com sucesso.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<UserModel>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<string>> ResetPasswordAsync(PasswordReset passwordReset)
    {
        try
        {
            string clientId = _getClientIdToken.GetClientIdFromToken(_httpContext);

            UserModel user = await _authDao.GetIdAsync(clientId);

            if (user == null)
            {
                return ResponseObject<string>(default!, $"Este clientId: {clientId} não é válido", StatusCodes.Status204NoContent, false);
            }

            var updateFields = new Dictionary<string, object>
            {
                { nameof(passwordReset.Password), _generateHash.GenerateHashParameters(passwordReset.Password) },
            };

            UserModel updatePassword = await _authDao.UpdateAsync(user.Id, updateFields);

            return ResponseObject<string>(default!, "Senha Redefinida", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(ex.ToString(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public OperationResult<string> VerificationPasswordOTP(string token)
    {
        try
        {
            UserModel passwordResetCache = _memoryCacheService.GetCache<UserModel>(token);

            if (passwordResetCache == null)
            {
                return ResponseObject<string>(default!, "Este token está expirado", StatusCodes.Status204NoContent, false);
            }

            string generateToken = _generateHash.GenerateToken(passwordResetCache);

            _memoryCacheService.RemoveFromCache<PasswordReset>(token);

            return ResponseObject(generateToken, "Token verififcado", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(ex.ToString(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    #region Metodos privados 

    private OperationResult<T> ResponseObject<T>(T content, string message, int statusCode, bool status)
    {
        return new OperationResult<T>()
        {
            Content = content,
            Message = message,
            StatusCode = statusCode,
            Status = status
        };
    }

    #endregion
}
