using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Dto.v1.User;

[ExcludeFromCodeCoverage]
public class LoginDto
{
    [Required(ErrorMessage = "O Email do login é obrigatório")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Password do login é obrigatório")]
    public string Password { get; set; } = string.Empty;
}
