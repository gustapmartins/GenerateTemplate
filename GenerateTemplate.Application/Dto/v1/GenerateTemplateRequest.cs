using System.ComponentModel.DataAnnotations;

namespace GenerateTemplate.Application.Dto.v1;

public class GenerateTemplateRequest
{
    /// <summary>
    /// Nome do objeto
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
}
