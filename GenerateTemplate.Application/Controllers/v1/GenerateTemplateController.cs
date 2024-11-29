using GenerateTemplate.Application.AppServices.v1;
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
public class GenerateTemplateController : ControllerBase
{

    private readonly ILogger<GenerateTemplateController> _logger;
    private readonly GenerateTemplateAppServices _appService;

    public GenerateTemplateController(ILogger<GenerateTemplateController> logger, GenerateTemplateAppServices appService)
    {
        _logger = logger;
        _appService = appService;
    }

    /// <summary>
    ///     Consultar todas as partidas criadas
    /// </summary>
    /// <param name="page">Objeto com os campos necessários para definir as paginas</param> 
    /// <param name="pageSize">Objeto com os campos necessários para os limites das paginas</param> 
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    /// <response code="204">Caso a busca seja feita com sucesso</response>
    [HttpGet("Get-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get matches with optional pagination parameters")]
    public ActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        Ok();
        throw new NotImplementedException();
    }
}
