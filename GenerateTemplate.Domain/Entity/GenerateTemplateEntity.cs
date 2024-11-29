using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Domain.Entity;

/// <summary>
/// Representa o objeto do seu negócio.
/// </summary>
[ExcludeFromCodeCoverage]
public class GenerateTemplateEntity : IEntity<string>
{
    /// <summary>
    /// Obtém ou define o ID do objeto.
    /// </summary>
    [BsonId] // Atributo que indica que esta propriedade é o ID do documento no MongoDB
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)] // Representação do tipo de dados ObjectId do MongoDB
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a data de criação do objeto.
    /// </summary>
    [BsonElement("DateCreated")] // Atributo que mapeia essa propriedade para o campo 'DateCreated' no MongoDB
    public DateTime DateCreated { get; set; }
}
