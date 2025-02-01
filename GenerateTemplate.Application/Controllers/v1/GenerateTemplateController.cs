using GenerateTemplate.Application.AppServices.v1;
using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.Dto.v1;
using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Interface.Services.v1;
using GenerateTemplate.Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenerateTemplate.Application.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v1/[controller]", Order = 1)]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class GenerateTemplateController : BaseController
{

    private readonly ILogger<GenerateTemplateController> _logger;
    private readonly IGenerateTemplateAppServices _generateTemplateAppService;

    public GenerateTemplateController(
        ILogger<GenerateTemplateController> logger,
        IGenerateTemplateAppServices appService,
        INotificationBase notificationBase) : base(notificationBase)
    {
        _logger = logger;
        _generateTemplateAppService = appService;
    }

    /// <summary>
    ///   Consultar todos os elementos
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    /// <response code="204">Caso a busca seja feita com sucesso</response>
    [HttpGet("Get-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get matches with optional pagination parameters")]
    public async Task<ActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        OperationResult<IEnumerable<GenerateTemplateResponse>> result = await _generateTemplateAppService.GetAsync(page, pageSize);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }

    /// <summary>
    ///   Criar um novo elemento
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    /// <response code="204">Caso a busca seja feita com sucesso</response>
    [HttpPost("Created")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get matches with optional pagination parameters")]
    public async Task<ActionResult<GenerateTemplateResponse>> CreateAsync([FromBody] GenerateTemplateRequest generateTemplateRequest)
    {
        OperationResult<GenerateTemplateResponse> result = await _generateTemplateAppService.CreateAsync(generateTemplateRequest);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }
}
