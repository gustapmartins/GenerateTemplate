using AutoMapper;
using GenerateTemplate.Application.Dto.v1;
using GenerateTemplate.Domain.Entity;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Mapper.v1;

[ExcludeFromCodeCoverage]
public class GenerateTemplateMapper : Profile
{
    public GenerateTemplateMapper()
    {
        CreateMap<GenerateTemplateRequest, GenerateTemplateEntity> ();
        CreateMap<GenerateTemplateEntity, GenerateTemplateResponse>();
        CreateMap(typeof(OperationResult<>), typeof(OperationResult<>));
    }
}
