using AutoMapper;
using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Domain.Interface.Services.v1;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.AppServices.v1;

[ExcludeFromCodeCoverage]
public class GenerateTemplateAppServices : IGenerateTemplateAppServices
{
    private readonly ILogger<GenerateTemplateAppServices> _logger;
    private readonly IGenerateTemplateService _generateTemplateService;
    private readonly IMapper _mapper;

    public GenerateTemplateAppServices(IMapper mapper, ILogger<GenerateTemplateAppServices> logger, IGenerateTemplateService generateTemplateService)
    {
        _mapper = mapper;
        _logger = logger;
        _generateTemplateService = generateTemplateService;
    }

    public async Task<string> GetAsync()
    {
        var result = await _generateTemplateService.GetAsync();

        return result;
    }
}
