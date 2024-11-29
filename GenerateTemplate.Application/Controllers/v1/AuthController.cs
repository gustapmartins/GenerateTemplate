using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenerateTemplate.Application.Controllers.v1;

[ApiVersion("1")]
[ApiController]
[Route("api/v1/[controller]", Order = 1)]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AUTHCONTROLLER : ControllerBase
{
    private readonly IAuthAppService _authAppService;

    public AUTHCONTROLLER(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    /// <summary>
    ///     Consultar todas as partidas criadas
    /// </summary>
    /// <param name="page">Objeto com os campos necessários para definir as paginas</param> 
    /// <param name="pageSize">Objeto com os campos necessários para os limites das paginas</param> 
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    [HttpGet("search-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Get matches with optional pagination parameters")]
    public async Task<ActionResult<OperationResult<IEnumerable<ViewUserDto>>>> GetUsersControlleraAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        OperationResult<IEnumerable<ViewUserDto>> result = await _authAppService.GetAsync(page, pageSize);

        if (result.Content.Any())
            return Ok(result);
        else
            return NoContent();
    }

    /// <summary>
    ///     Consultar todas as partidas criadas
    /// </summary>
    /// <param name="Id">Objeto com os campos necessários para definir as paginas</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    [HttpGet("search-user/{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get matches with optional pagination parameters")]
    public async Task<ActionResult<OperationResult<ViewUserDto>>> GetUserIdControllerAsync([FromRoute] string Id)
    {
        OperationResult<ViewUserDto> result = await _authAppService.GetIdAsync(Id);

        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }


    /// <summary>
    ///     Cria um novo usuario no banco de dados
    /// </summary>
    /// <param name="createUserDto">Objeto com os campos necessários para criação de um usuário</param>
    /// <remarks> Create User </remarks>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso a requisição esteja errada</response>
    [HttpPost("created-user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OperationResult<CreateUserDto>>> CreateUserControllerAsync([FromBody] CreateUserDto createUserDto)
    {
        OperationResult<CreateUserDto> result = await _authAppService.CreateAsync(createUserDto);

        if (result.Content is not null)
            return CreatedAtAction(nameof(GetUsersControlleraAsync), result);
        else
            return NoContent();
    }

    /// <summary>
    ///     Apagar um usuario no banco de dados
    /// </summary>
    /// <param name="Id">Objeto com os campos necessários para delete um usuário</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso a requisição esteja errada</response>
    [HttpDelete("delete-user/{Id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OperationResult<ViewUserDto>>> RemoveUserAsync([FromQuery] string Id)
    {
        OperationResult<ViewUserDto> result = await _authAppService.RemoveAsync(Id);

        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }

    /// <summary>
    ///     Atualiza o usuario a partir do Id
    /// </summary>
    /// <param name="Id">Objeto com os campos necessários para criação de um filme</param>
    /// <param name="updateUserDto">TEAM</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    /// <response code="404">Caso inserção não seja feita com sucesso</response>
    [HttpPatch("update-user/{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationResult<ViewUserDto>>> UpdateMatchAsync([FromRoute] string Id, [FromBody] UpdateUserDto updateUserDto)
    {
        OperationResult<ViewUserDto> result = await _authAppService.UpdateAsync(Id, updateUserDto);

        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }

    /// <summary>
    ///     faz o login e retorna um token de acesso
    /// </summary>
    /// <param name="loginDto">Objeto com os campos necessários para logar um usuário</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    /// <response code="404">Caso inserção não seja feita com sucesso</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginAsync(LoginDto loginDto)
    {
        OperationResult<string> token = await _authAppService.LoginAsync(loginDto);

        if (token is not null)
            return Ok(new { token });
        else
            return NoContent();
    }

    /// <summary>
    ///     Redefinir a senha de um usuario no banco de dados
    /// </summary>
    /// <param name="passwordResetDto">Objeto com os campos necessários para mudar a senha de um usuário</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso a requisição esteja errada</response>
    [Authorize]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPasswordAsync([FromBody] PasswordResetDto passwordResetDto)
    {
        var result = await _authAppService.ResetPasswordAsync(passwordResetDto);

        if (result is not null)
            return Ok(result);
        else
            return NoContent();
    }

    /// <summary>
    ///     Redefinir a senha de um usuario no banco de dados
    /// </summary>
    /// <param name="email">Objeto com os campos necessários para mudar a senha de um usuário</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso a requisição esteja errada</response>
    [HttpPost("forget-password")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgetPasswordAsync([FromHeader] string email)
    {
        var code = await _authAppService.ForgetPasswordAsync(email);

        if (code is not null)
            return Ok(new { code });
        else
            return NoContent();
    }

    /// <summary>
    ///     Redefinir a senha de um usuario no banco de dados
    /// </summary>
    /// <param name="token">Objeto com os campos necessários para mudar a senha de um usuário</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso a requisição esteja errada</response>
    [HttpPost("verification-password-OTP")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult VerificationPasswordOTP([FromHeader] string token)
    {
        OperationResult<string> hashToken = _authAppService.VerificationPasswordOTP(token);

        if (hashToken is not null)
            return Ok(new { token = hashToken });
        else
            return NoContent();
    }
}
