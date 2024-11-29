using GenerateTemplate.Domain.Interface.Dao;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using VarzeaLeague.Infra.Data.Repository.EfCore;

namespace GenerateTemplate.Infra.Data;

[ExcludeFromCodeCoverage]
public static class RepositoryDependencyInjectionModule
{
    public static void RepositoryDependencyInjectionModuleModule(this IServiceCollection services)
    {
        #if DEBUG
           services.AddSingleton<IAuthDao, AuthDaoEfCore>();
        #elif Authentication
           services.AddSingleton<IAuthDao, AuthDaoEfCore>();
        #endif
    }
}
