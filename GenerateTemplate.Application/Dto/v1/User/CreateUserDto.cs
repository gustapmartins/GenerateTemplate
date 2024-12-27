using GenerateTemplate.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GenerateTemplate.Application.Dto.v1.User;

[ExcludeFromCodeCoverage]
public class CreateUserDto
{
    public string _id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$", ErrorMessage = "Must contain uppercase and lowercase letter and number")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password")]
    [Compare("Password", ErrorMessage = "'ConfirmPassword' and 'Password' do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm is required")]
    [RegularExpression("([0-9]{2}[\\.]?[0-9]{3}[\\.]?[0-9]{3}[\\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\\.]?[0-9]{3}[\\.]?[0-9]{3}[-]?[0-9]{2})", ErrorMessage = "Invalid CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "YearsOld is required")]
    public Role Role { get; set; }
}
