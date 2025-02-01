using MongoDB.Bson.Serialization.Attributes;

namespace GenerateTemplate.Application.Dto.v1;

public class GenerateTemplateResponse
{
    /// <summary>
    /// Obtém ou define o ID do objeto.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome do objeto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a data de criação do objeto.
    /// </summary>
    public DateTime DateCreated { get; set; }
}
