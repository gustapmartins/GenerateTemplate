using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Domain.Interface.Services.v1;

public interface IGenerateTemplateService
{
    Task<OperationResult<string>> GetAsync();
}
