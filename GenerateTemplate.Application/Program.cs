using GenerateTemplate.Application.AppServices;
using GenerateTemplate.Domain;
using GenerateTemplate.Infra.CrossCutting.Ioc;
using GenerateTemplate.Infra.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

AppServiceDependencyInjection.AppServiceDependencyInjectionModule(builder.Services);

DependencyInjection.ConfigureService(builder.Services, builder.Configuration, xmlFilename);

RepositoryDependencyInjectionModule.RepositoryDependencyInjectionModuleModule(builder.Services);

ServiceDependencyInjection.ServiceDependencyInjectionModule(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnableSwaggerSupport"))
{
    #if DEBUG
        app.UseSwagger();
        app.UseSwaggerUI();
    #elif EnableSwaggerSupport
        app.UseSwagger();
        app.UseSwaggerUI();
    #endif
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.Run();