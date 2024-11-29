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

public class AuthService : IAuthService
{
    private readonly IAuthDao _authDao;
    private readonly IGenerateHash _generateHash;
    private readonly HttpContext _httpContext;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IEmailService _emailService;
    private readonly IGetClientIdToken _getClientIdToken;

    public AuthService(
        IAuthDao authDao,
        IGenerateHash generateHash,
        IEmailService emailService,
        IMemoryCacheService memoryCacheService,
        IHttpContextAccessor httpContextAccessor,
        IGetClientIdToken getClientIdToken)
    {
        _authDao = authDao;
        _generateHash = generateHash;
        _emailService = emailService;
        _memoryCacheService = memoryCacheService;
        _getClientIdToken = getClientIdToken;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<OperationResult<IEnumerable<UserModel>>> GetAllAsync(int page, int pageSize)
    {
        try
        {
            IEnumerable<UserModel> GetAll = await _authDao.GetAllAsync(page, pageSize,
                filter: Builders<UserModel>.Filter.Where(x => x.AccountStatus == AccountStatus.Active));

            if (GetAll.Count() == 0)
            {
                return new OperationResult<IEnumerable<UserModel>>()
                {
                    Message = "Não há usuários cadastrados.",
                    Content = GetAll,
                    StatusCode = StatusCodes.Status404NotFound,
                    Status = false
                };
            }

            return new()
            {
                StatusCode = StatusCodes.Status200OK,
                Content = GetAll,
                Message = "Usuários encontrados.",
                Status = true,
            };
        }
        catch (ExceptionFilter ex)
        {
            return new()
            {
                Content = Enumerable.Empty<UserModel>(),
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false,
            };
        }
    }

    public async Task<OperationResult<UserModel>> GetIdAsync(string Id)
    {
        try
        {
            UserModel GetAsyncId = await _authDao.GetIdAsync(Id);

            if (GetAsyncId == null)
                return new OperationResult<UserModel>(
                    content: default!,
                    message: $"Usuário com id: {Id} não existe.",
                    statusCode: StatusCodes.Status404NotFound,
                    status: false);


            if (GetAsyncId.AccountStatus == 0)
                return new OperationResult<UserModel>(
                    content: GetAsyncId,
                    message: "Usuário bloqueado.",
                    statusCode: StatusCodes.Status404NotFound,
                    status: false);

            return new OperationResult<UserModel>()
            {
                Content = GetAsyncId,
                StatusCode = StatusCodes.Status200OK,
                Message = "Usuário encontrado.",
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<UserModel>()
            {
                Content = default!,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false,
            };
        }
    }

    public async Task<OperationResult<UserModel>> CreateAsync(UserModel addObject)
    {
        try
        {
            UserModel findEmail = await _authDao.FindEmailAsync(addObject.Email);

            if (findEmail != null)
            {
                return new OperationResult<UserModel>()
                {
                    Content = default!,
                    Message = $"Usuário com esse email: '{addObject.Email}', já existe.",
                    StatusCode = StatusCodes.Status409Conflict,
                    Status = false
                };
            }

            addObject.Password = _generateHash.GenerateHashParameters(addObject.Password);
            addObject.AccountStatus = AccountStatus.Active;
            addObject.DateCreated = DateTime.UtcNow;

            await _authDao.CreateAsync(addObject);

            return new OperationResult<UserModel>() 
            {
                Content = addObject,    
                Message = "Usuário criado com sucesso.",
                StatusCode = StatusCodes.Status200OK,
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<UserModel>()
            {
                Content = default!,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false,
            };
        }
    }

    public async Task<OperationResult<string>> LoginAsync(UserModel userLogin)
    {
        try
        {
            UserModel findUser = await _authDao.FindEmailAsync(userLogin.Email);

            if (findUser == null)
            {
                return new OperationResult<string>()
                {
                    Content = string.Empty,
                    Message = $"Este email: {userLogin.Email} não existe.",
                    StatusCode = StatusCodes.Status409Conflict,
                    Status = false,
                };
            }

            //Faz uma validação para verificar se a senha que o usuariop está passando corresponde a senha salva no banco, em formato hash
            bool isPasswordCorrect = _generateHash.VerifyPassword(userLogin.Password, findUser.Password);

            if (!isPasswordCorrect)
            {
                return new OperationResult<string>()
                {
                    Content = string.Empty,
                    Message = $"Senha incorreta.",
                    StatusCode = StatusCodes.Status400BadRequest,
                    Status = false,
                };
            }

            //Gera um token a partir do usuario buscado pelo E-mail
            string token = _generateHash.GenerateToken(findUser);

            return new OperationResult<string>()
            {
                Content = token,    
                Message = "Usuário logado com sucesso.",
                StatusCode = StatusCodes.Status200OK,
                Status = true,
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<string>()
            {
                Content = string.Empty,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false,
            };
        }
    }

    public async Task<OperationResult<UserModel>> RemoveAsync(string Id)
    {
        try
        {
            OperationResult<UserModel> userView = await GetIdAsync(Id);

            if (userView.Content == null)
                return new OperationResult<UserModel>()
                {
                    Content = userView.Content!,
                    Message = userView.Message,
                    StatusCode = userView.StatusCode,
                    Status = userView.Status
                };

            var updateFields = new Dictionary<string, object>
            {
                { nameof(AccountStatus), AccountStatus.Blocked },
            };

            UserModel updateUser = await _authDao.UpdateAsync(Id, updateFields);

            return new OperationResult<UserModel>()
            {
                Content = updateUser,
                Message = "Usuário bloqueado.",
                StatusCode = StatusCodes.Status200OK,
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<UserModel>()
            {
                Content = default!,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false
            };
        }
    }

    public async Task<OperationResult<string>> ForgetPasswordAsync(string email)
    {
        try
        {
            UserModel findEmail = await _authDao.FindEmailAsync(email);

            if (findEmail == null)
            {
                return new OperationResult<string>()
                {
                    Content = string.Empty,
                    Message = $"Este E-mail: {email} não é válido",
                    StatusCode = StatusCodes.Status404NotFound,
                    Status = false
                };
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

            return new OperationResult<string>()
            { 
                Content = token,
                Message = "Email enviado com sucesso!",
                StatusCode = StatusCodes.Status200OK,
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<string>()
            {
                Content = string.Empty,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false
            };
        }
    }

    public async Task<OperationResult<UserModel>> UpdateAsync(string Id, UserModel updateObject)
    {
        try
        {
            OperationResult<UserModel> userView = await GetIdAsync(Id);

            if (userView.Content == null)
                return new OperationResult<UserModel>()
                {
                    Content = userView.Content!,
                    Message = userView.Message,
                    StatusCode = userView.StatusCode,
                    Status = userView.Status
                };

            var updateFields = new Dictionary<string, object>
            {
                { nameof(updateObject.UserName), updateObject.UserName },
                { nameof(updateObject.Password), updateObject.Password },
                { nameof(updateObject.AccountStatus), updateObject.AccountStatus }
            };

            UserModel userUpdate = await _authDao.UpdateAsync(Id, updateFields);

            return new OperationResult<UserModel>()
            {
                Content = userUpdate,
                Message = "Usuário atualizado com sucesso.",
                StatusCode = StatusCodes.Status200OK,
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<UserModel>()
            {
                Content = default!,
                Message = ex.Message,
                StatusCode = StatusCodes.Status200OK,
                Status = false
            };
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
                return new OperationResult<string>()
                {
                    Content = default!,
                    Message = $"Este clientId: {clientId} não é válido",
                    StatusCode = StatusCodes.Status404NotFound,
                    Status = false,
                };
            }

            var updateFields = new Dictionary<string, object>
            {
                { nameof(passwordReset.Password), _generateHash.GenerateHashParameters(passwordReset.Password) },
            };

            UserModel updatePassword = await _authDao.UpdateAsync(user.Id, updateFields);

            return new OperationResult<string>() 
            { 
                Status = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Senha Redefinida",
                Content = default!
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<string>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = ex.Message,
                Content = string.Empty,
                Status = false
            };
        }
    }

    public OperationResult<string> VerificationPasswordOTP(string token)
    {
        try
        {
            UserModel passwordResetCache = _memoryCacheService.GetCache<UserModel>(token);

            if (passwordResetCache == null)
            {
                return new OperationResult<string>()
                {
                    Content = default!,
                    Message = "Este token está expirado",
                    StatusCode = StatusCodes.Status404NotFound,
                    Status = false
                };
            }

            string generateToken = _generateHash.GenerateToken(passwordResetCache);

            _memoryCacheService.RemoveFromCache<PasswordReset>(token);

            return new OperationResult<string>()
            {
                Content = generateToken,
                Message = "Token verififcado",
                StatusCode = StatusCodes.Status200OK,
                Status = true
            };
        }
        catch (ExceptionFilter ex)
        {
            return new OperationResult<string>()
            {
                Content = default!,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Status = false
            };
        }
    }
}
