using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Application.AppServices.v1.Interfaces;

public interface IGenerateTemplateAppServices
{
    Task<OperationResult<string>> GetAsync();
}
