using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.AppServices.v1;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.AppServices;

[ExcludeFromCodeCoverage]
public static class AppServiceDependencyInjection
{
    public static void AppServiceDependencyInjectionModule(this IServiceCollection services)
    {
    #if DEBUG
        services.AddScoped<IAuthAppService, AuthAppService>();
    #elif Authorization
        services.AddScoped<IAuthAppService, AuthAppService>();
    #endif

        services.AddScoped<IGenerateTemplateAppServices, GenerateTemplateAppServices>();
    }
}
