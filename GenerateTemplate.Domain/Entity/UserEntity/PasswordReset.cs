using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Domain.Entity.UserEntity;

/// <summary>
/// Representa a redefinição de senha.
/// </summary>
[ExcludeFromCodeCoverage]
public class PasswordReset
{
    public string Password { get; set; } = string.Empty;
}