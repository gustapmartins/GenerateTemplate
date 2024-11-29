using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Dto.v1.User;

[ExcludeFromCodeCoverage]
public class PasswordResetDto
{
    [Required(ErrorMessage = "O Password is required")]
    public required string? Password { get; set; }

    [Required(ErrorMessage = "Confirm your password")]
    [Compare("Password")]
    public required string? ConfirmPassword { get; set; }
}
