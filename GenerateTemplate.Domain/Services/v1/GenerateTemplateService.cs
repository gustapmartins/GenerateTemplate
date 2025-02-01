using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Exceptions;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GenerateTemplate.Application.Controllers.v1;

public class GenerateTemplateService : IGenerateTemplateService
{

    private readonly IConfiguration _configuration;
    private readonly IGenerateTemplateDao _generateTemplateDao;

    public GenerateTemplateService(IConfiguration configuration, IGenerateTemplateDao generateTemplateDao)
    {
        _configuration = configuration;
        _generateTemplateDao = generateTemplateDao;
    }

    public async Task<OperationResult<IEnumerable<GenerateTemplateEntity>>> GetAsync(int page, int pageSize)
    {
        try
        {
            IEnumerable<GenerateTemplateEntity> GetAll = await _generateTemplateDao.GetAllAsync(page, pageSize);

            if (GetAll.Count() == 0)
            {
                return ResponseObject(GetAll, "N�o h� objetos cadastrados.", StatusCodes.Status404NotFound, false);
            }

            return ResponseObject(GetAll, "Objetos encontrados.", StatusCodes.Status200OK, true);
        }
        catch (ExceptionFilter ex)
        {
            return ResponseObject(Enumerable.Empty<GenerateTemplateEntity>(), ex.Message, StatusCodes.Status500InternalServerError, false);
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
