using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Repository.EfCore;

namespace GenerateTemplate.Infra.Data;

[ExcludeFromCodeCoverage]
public static class RepositoryDependencyInjectionModule
{
    public static void RepositoryDependencyInjectionModuleModule(this IServiceCollection services)
    {
#if Authentication || DEBUG
        services.AddSingleton<IAuthDao, AuthDaoEfCore>();
#endif
        services.AddSingleton<IGenerateTemplateDao, GenerateTemplateEfCore>();
    }
}
