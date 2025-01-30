using GenerateTemplate.Application.Controllers.v1;
using GenerateTemplate.Domain.Interface.Services;
using GenerateTemplate.Domain.Interface.Services.v1;
using GenerateTemplate.Domain.Interface.Utils;
using GenerateTemplate.Domain.Services.v1;
using GenerateTemplate.Domain.Utils;
using GenerateTemplate.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using VarzeaLeague.Domain.Service;
#if Authentication|| DEBUG
using GenerateTemplate.Domain.JwtHelper;
#endif

namespace GenerateTemplate.Domain;

[ExcludeFromCodeCoverage]
public static class ServiceDependencyInjection
{
    public static void ServiceDependencyInjectionModule(this IServiceCollection services)
    {
#if Authentication|| DEBUG
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGetClientIdToken, GetClientIdToken>();
#endif
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IGenerateTemplateService, GenerateTemplateService>();

        services.AddScoped<IMemoryCacheService, MemoryCacheService>();

        services.AddScoped<IGenerateHash, GenerateHash>();

        services.AddScoped<IRedisService, RedisService>();

        services.AddScoped<INotificationBase, NotificationBase>();
    }
}
