using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Interface.Services.v1;
using Microsoft.Extensions.Configuration;

namespace GenerateTemplate.Application.Controllers.v1;

public class GenerateTemplateService : IGenerateTemplateService
{

    private readonly IConfiguration _configuration;

    public GenerateTemplateService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<OperationResult<string>> GetAsync()
    {
        return ResponseObject("Mensagem de retorno", "retorno", 200, true);
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
