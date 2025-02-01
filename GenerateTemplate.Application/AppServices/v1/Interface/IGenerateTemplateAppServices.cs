using GenerateTemplate.Application.Dto.v1;
using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Application.AppServices.v1.Interfaces;

public interface IGenerateTemplateAppServices
{
    Task<OperationResult<IEnumerable<GenerateTemplateResponse>>> GetAsync(int page, int pageSize);

    Task<OperationResult<GenerateTemplateResponse>> CreateAsync(GenerateTemplateRequest generateTemplateResponse);
}
