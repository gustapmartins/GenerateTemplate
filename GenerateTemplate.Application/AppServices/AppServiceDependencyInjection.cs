using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.AppServices.v1;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.AppServices;

[ExcludeFromCodeCoverage]
public static class AppServiceDependencyInjection
{
    public static void AppServiceDependencyInjectionModule(this IServiceCollection services)
    {
#if Authentication || DEBUG 
        services.AddScoped<IAuthAppService, AuthAppService>();
#endif

        services.AddScoped<IGenerateTemplateAppServices, GenerateTemplateAppServices>();
    }
}
