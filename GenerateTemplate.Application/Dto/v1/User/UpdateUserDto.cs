using GenerateTemplate.Domain.Enum;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Dto.v1.User;

[ExcludeFromCodeCoverage]
public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;

    public  AccountStatus AccountStatus { get; set; }
}
