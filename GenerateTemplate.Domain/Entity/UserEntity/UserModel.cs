using GenerateTemplate.Domain.Enum;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Domain.Entity.UserEntity;

/// <summary>
/// Representa um usuário no sistema.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserModel : IEntity<string>
{
    /// <summary>
    /// Obtém ou define o ID do usuário.
    /// </summary>
    [BsonId] // Atributo que indica que esta propriedade é o ID do documento no MongoDB
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)] // Representação do tipo de dados ObjectId do MongoDB
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome de usuário.
    /// </summary>
    [BsonElement("UserName")] // Atributo que mapeia essa propriedade para o campo 'UserName' no MongoDB
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o email do usuário.
    /// </summary>
    [BsonElement("Email")] // Atributo que mapeia essa propriedade para o campo 'Email' no MongoDB
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a senha do usuário.
    /// </summary>
    [BsonElement("Password")] // Atributo que mapeia essa propriedade para o campo 'Password' no MongoDB
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o CPF do usuário.
    /// </summary>
    [BsonElement("Cpf")] // Atributo que mapeia essa propriedade para o campo 'Cpf' no MongoDB
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a função do usuário.
    /// </summary>
    [BsonElement("Role")] // Atributo que mapeia essa propriedade para o campo 'Role' no MongoDB
    public Role Role { get; set; }

    /// <summary>
    /// Obtém ou define o status da conta do usuário.
    /// </summary>
    [BsonElement("AccountStatus")] // Atributo que mapeia essa propriedade para o campo 'AccountStatus' no MongoDB
    public AccountStatus AccountStatus { get; set; }

    /// <summary>
    /// Obtém ou define a data de criação do usuário.
    /// </summary>
    [BsonElement("DateCreated")] // Atributo que mapeia essa propriedade para o campo 'DateCreated' no MongoDB
    public DateTime DateCreated { get; set; }
}
