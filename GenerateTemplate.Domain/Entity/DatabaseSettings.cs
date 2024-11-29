using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Domain.Entity;

//Implementação com mongodb
[ExcludeFromCodeCoverage]
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;
}
