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

    public async Task<string> GetAsync()
    {
       return "MetodoService";
    }
}
