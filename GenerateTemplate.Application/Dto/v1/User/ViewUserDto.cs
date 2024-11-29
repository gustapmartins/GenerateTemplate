using GenerateTemplate.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Dto.v1.User;

[ExcludeFromCodeCoverage]
public class ViewUserDto
{
    public string Id { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; }

    public Role Role { get; set; } = Role.User;

    public AccountStatus AccountStatus { get; set; }
}
