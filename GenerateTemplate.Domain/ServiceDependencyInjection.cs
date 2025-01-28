using GenerateTemplate.Application.Controllers.v1;
using GenerateTemplate.Domain.Interface.Services;
using GenerateTemplate.Domain.Interface.Services.v1;
using GenerateTemplate.Domain.Interface.Utils;
using GenerateTemplate.Domain.JwtHelper;
using GenerateTemplate.Domain.Services.v1;
using GenerateTemplate.Domain.Utils;
using GenerateTemplate.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using VarzeaLeague.Domain.Service;

namespace GenerateTemplate.Domain;

[ExcludeFromCodeCoverage]
public static class ServiceDependencyInjection
{
    public static void ServiceDependencyInjectionModule(this IServiceCollection services)
    {
#if DEBUG
        services.AddScoped<IAuthService, AuthService>();
#elif Authorization
        Authorization.ConfigureAuth(services);
#endif
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IGenerateTemplateService, GenerateTemplateService>();

        services.AddScoped<IMemoryCacheService, MemoryCacheService>();

        services.AddScoped<IGetClientIdToken, GetClientIdToken>();

        services.AddScoped<IGenerateHash, GenerateHash>();

        services.AddScoped<IRedisService, RedisService>();

        services.AddScoped<INotificationBase, NotificationBase>();
    }
}
