using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Domain.Interface.Services.v1;

public interface IGenerateTemplateService
{
    Task<OperationResult<IEnumerable<GenerateTemplateEntity>>> GetAsync(int page, int pageSize);

    Task<OperationResult<GenerateTemplateEntity>> CreateAsync(GenerateTemplateEntity generateTemplateEntity);
}
