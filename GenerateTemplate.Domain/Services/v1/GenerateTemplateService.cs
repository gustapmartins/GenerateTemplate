using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Enum;
using GenerateTemplate.Domain.Exceptions;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GenerateTemplate.Application.Controllers.v1;

public class GenerateTemplateService : IGenerateTemplateService
{
    private readonly IConfiguration _configuration;
    private readonly IGenerateTemplateDao _generateTemplateDao;
    private readonly IRedisService _redisService;
    private readonly INotificationBase _notificationBase;

    public GenerateTemplateService(IConfiguration configuration, IGenerateTemplateDao generateTemplateDao, IRedisService redisService, INotificationBase notificationBase)
    {
        _configuration = configuration;
        _generateTemplateDao = generateTemplateDao;
        _redisService = redisService;
        _notificationBase = notificationBase;
    }

    public async Task<OperationResult<IEnumerable<GenerateTemplateEntity>>> GetAsync(int page, int pageSize)
    {
        try
        {
            IEnumerable<GenerateTemplateEntity> GetAll = await _generateTemplateDao.GetAllAsync(page, pageSize);

            if (GetAll.IsNullOrEmpty())
            {
                return ResponseObject(GetAll, "Não há objetos cadastrados.", StatusCodes.Status404NotFound, false);
            }

            return ResponseObject(GetAll, "Objetos encontrados.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(Enumerable.Empty<GenerateTemplateEntity>(), ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<GenerateTemplateEntity>> GetIdAsync(string Id)
    {
        try
        {
            GenerateTemplateEntity GetUserCacheAsyncId = await _redisService.GetAsync<GenerateTemplateEntity>(Id);

            if (GetUserCacheAsyncId is null)
            {
                GenerateTemplateEntity GetUserAsyncId = await _generateTemplateDao.GetIdAsync(Id);

                if (GetUserAsyncId is null)
                {
                    await _notificationBase.NotifyAsync("Usuário não encontrado.", $"Usuário com id: {Id} não existe.");
                    return ResponseObject<GenerateTemplateEntity>(GetUserAsyncId, $"Usuário com id: {Id} não existe.", StatusCodes.Status204NoContent, false);
                }

                await _redisService.SetAsync(Id, GetUserAsyncId, TimeSpan.FromMinutes(5));

                return ResponseObject(GetUserAsyncId, "Usuário encontrado", StatusCodes.Status200OK, true);
            }

            return ResponseObject(GetUserCacheAsyncId, "Usuário encontrado", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            await _notificationBase.NotifyAsync("Error internal", ex.Message);
            return ResponseObject<GenerateTemplateEntity>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<GenerateTemplateEntity>> CreateAsync(GenerateTemplateEntity generateTemplateEntity)
    {
        try
        {
            await _generateTemplateDao.CreateAsync(generateTemplateEntity);

            return ResponseObject(generateTemplateEntity, "Objeto criado com sucesso.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<GenerateTemplateEntity>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<GenerateTemplateEntity>> RemoveAsync(string Id)
    {
        try
        {
            OperationResult<GenerateTemplateEntity> userView = await GetIdAsync(Id);

            if (userView.Content == null)
                return ResponseObject(userView.Content!, userView.Message, userView.StatusCode, userView.Status);

            var updateFields = new Dictionary<string, object>
            {
                { nameof(AccountStatus), AccountStatus.Blocked },
            };

            GenerateTemplateEntity updateUser = await _generateTemplateDao.UpdateAsync(Id, updateFields);

            return ResponseObject(updateUser, "Usuário bloqueado.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<GenerateTemplateEntity>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<GenerateTemplateEntity>> UpdateAsync(string Id, GenerateTemplateEntity updateObject)
    {
        try
        {
            OperationResult<GenerateTemplateEntity> GenerateTemplate = await GetIdAsync(Id);

            if (GenerateTemplate == null)
                return ResponseObject(GenerateTemplate.Content!, GenerateTemplate.Message, GenerateTemplate.StatusCode, GenerateTemplate.Status);

            var updateFields = new Dictionary<string, object>
            {
                { nameof(updateObject.Name), updateObject.Name }
            };

            GenerateTemplateEntity updateGenerateTemplate = await _generateTemplateDao.UpdateAsync(Id, updateFields);

            return ResponseObject(updateGenerateTemplate, "Objeto atualizado com sucesso.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject<GenerateTemplateEntity>(default!, ex.Message, StatusCodes.Status500InternalServerError, false);
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
