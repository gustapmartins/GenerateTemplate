using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

#if Authentication || DEBUG
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Repository.EfCore;
#endif

namespace GenerateTemplate.Infra.Data;

[ExcludeFromCodeCoverage]
public static class RepositoryDependencyInjectionModule
{
    public static void RepositoryDependencyInjectionModuleModule(this IServiceCollection services)
    {
#if Authentication || DEBUG
        services.AddSingleton<IAuthDao, AuthDaoEfCore>();
#endif
    }
}
