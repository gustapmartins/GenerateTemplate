using AutoMapper;
using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.Dto.v1;
using GenerateTemplate.Domain.Entity;
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

    public async Task<OperationResult<IEnumerable<GenerateTemplateResponse>>> GetAsync(int page, int pageSize)
    {
        OperationResult<IEnumerable<GenerateTemplateEntity>> result = await _generateTemplateService.GetAsync(page, pageSize);

        return _mapper.Map<OperationResult<IEnumerable<GenerateTemplateResponse>>>(result);
    }

    public async Task<OperationResult<GenerateTemplateResponse>> CreateAsync(GenerateTemplateRequest generateTemplateResponse)
    {
        GenerateTemplateEntity GenerateTemplateMapper = _mapper.Map<GenerateTemplateEntity>(generateTemplateResponse);

        OperationResult<GenerateTemplateEntity> result = await _generateTemplateService.CreateAsync(GenerateTemplateMapper);

        return _mapper.Map<OperationResult<GenerateTemplateResponse>>(result);
    }
}
