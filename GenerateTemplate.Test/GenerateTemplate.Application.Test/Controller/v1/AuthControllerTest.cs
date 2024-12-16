using AutoFixture;
using AutoMapper;
using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.Controllers.v1;
using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenerateTemplate.Application.Test.Controller.v1;

public class AuthControllerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<IAuthAppService> _authAppServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _fixture = new Fixture();
        _authAppServiceMock = new Mock<IAuthAppService>();
        _mapperMock = new Mock<IMapper>();
        _authController = new AuthController(_authAppServiceMock.Object);
    }

    [Fact]
    public async Task GetUsers_WhenCalled_ReturnsOkResultWithUsersAsync()
    {
        // Arrange
        var users = _fixture.Create<OperationResult<IEnumerable<ViewUserDto>>>();

        _authAppServiceMock.Setup(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(users);

        // Act
        var result = await _authController.GetUsersControlleraAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var operationResult = okResult.Value as OperationResult<IEnumerable<ViewUserDto>>;
        Assert.NotNull(operationResult);
        Assert.Equal(3, operationResult.Content.Count());
    }

    [Fact]
    public async Task GetUserId_WhenCalled_ReturnsOkResultWithUserAsync()
    {
        // Arrange
        var User = _fixture.Create<OperationResult<ViewUserDto>>();
        var userViewDto = _fixture.Create<OperationResult<ViewUserDto>>();

        _authAppServiceMock.Setup(service => service.GetIdAsync(It.IsAny<string>()))
            .ReturnsAsync(User);

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<ViewUserDto>>(It.IsAny<UserModel>()))
            .Returns(userViewDto);

        // Act
        var result = await _authController.GetUserIdControllerAsync(It.IsAny<string>()) as ActionResult<OperationResult<ViewUserDto>>;

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var operationResult = okResult.Value as OperationResult<ViewUserDto>;
        Assert.NotNull(operationResult);
    }

    [Fact]
    public async Task loginasync_whencalled_returnsokresultwithtokenAsync()
    {
        // arrange
        var usermodel = _fixture.Create<UserModel>();
        var token = _fixture.Create<OperationResult<string>>();

        _authAppServiceMock.Setup(service => service.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(token);

        // act
        var result = await _authController.LoginAsync(It.IsAny<LoginDto>()) as OkObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        // converta result.value para jobject para acessar a propriedade 'token'
        var json = JsonConvert.SerializeObject(result);
        var resultvalue = JObject.Parse(json);

        // acesse a propriedade 'token' diretamente
        var tokenvalue = resultvalue["Value"]["token"]["Content"].ToString();
        Assert.Equal(token.Content, tokenvalue);
    }

    [Fact]
    public async Task PostTeam_WhenCalled_ReturnsCreatedAtActionResultAsync()
    {

        // Configure the fixture to create valid instances of RegisterDto
        _fixture.Customize<CreateUserDto>(composer => composer
           .With(r => r.Username, "ValidUsername")
           .With(r => r.Email, "valid@example.com")
           .With(r => r.Password, "Valid1234")
           .With(r => r.ConfirmPassword, "Valid1234")
           .With(r => r.Cpf, "123.456.789-09")
           .With(r => r.Role, Role.Admin)); // Adjust Role to a valid enum value

        var createUserDto = _fixture.Create<CreateUserDto>(); // Generate the DTO instance
        var userModel = new OperationResult<CreateUserDto> { Content = createUserDto }; // Simulate the response from service

        // Configure the mock to return the simulated OperationResult
        _authAppServiceMock.Setup(service => service.CreateAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(userModel);

        // Act
        var result = await _authController.CreateUserControllerAsync(createUserDto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult); // Ensure it returns a CreatedAtActionResult
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode); // Status code should be 201

        var operationResult = createdResult.Value as OperationResult<CreateUserDto>;
        Assert.NotNull(operationResult); // Ensure the result is an OperationResult
    }

    [Fact]
    public async Task ForgetPassword_WhenCalled_ReturnsOkResultWithTokenAsync()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var token = _fixture.Create<OperationResult<string>>();

        _authAppServiceMock.Setup(service => service.ForgetPasswordAsync(It.IsAny<string>()))
            .ReturnsAsync(token);

        // Act
        var result = await _authController.ForgetPasswordAsync(It.IsAny<string>()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        // Converta result.Value para JObject para acessar a propriedade 'token'
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Value);
        var resultValue = JObject.Parse(json);

        // Acesse a propriedade 'token' diretamente
        var tokenValue = resultValue["code"]["Content"].ToString();
        Assert.Equal(token.Content, tokenValue);
    }

    [Fact]
    public async Task ResetPassword_WhenCalled_ReturnsOkResultAsync()
    {
        // Arrange
        var passwordReset = _fixture.Create<PasswordReset>();
        var AuthAppServiceMock = _fixture.Create<OperationResult<string>>();

        _mapperMock.Setup(mapper => mapper.Map<PasswordReset>(It.IsAny<PasswordResetDto>()))
            .Returns(passwordReset);

        // Ajuste na configuração do mock para o método ResetPasswordAsync
        _authAppServiceMock.Setup(service => service.ResetPasswordAsync(It.IsAny<PasswordResetDto>()))
            .ReturnsAsync(AuthAppServiceMock); // Exemplo de retorno do método ResetPasswordAsync

        // Act
        var result = await _authController.ResetPasswordAsync(It.IsAny<PasswordResetDto>()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(AuthAppServiceMock, result.Value); // Verifica se o valor retornado é o esperado
    }

    [Fact]
    public async Task RemoveUser_WhenCalled_ReturnsOkResultAsync()
    {
        // Arrange
        var userMock = _fixture.Build<OperationResult<ViewUserDto>>()
                       .With(x => x.StatusCode, StatusCodes.Status200OK) // Remova o valor padrão do AutoFixture para StatusCode 
                       .Without(x => x.Content) // Remova o valor padrão do AutoFixture para Content
                       .Do(x => x.Content = new ViewUserDto { AccountStatus = AccountStatus.Active }) // Inicialize e configure Content diretamente
                       .Create();

        _authAppServiceMock.Setup(service => service.RemoveAsync(It.IsAny<string>()))
            .ReturnsAsync(userMock);

        // Act
        var result = await _authController.RemoveUserAsync(It.IsAny<string>());

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var operationResult = okResult.Value as OperationResult<ViewUserDto>;
        Assert.NotNull(operationResult);
        Assert.Equal(StatusCodes.Status200OK, operationResult.StatusCode);
    }

    [Fact]
    public async Task UpdateMatch_WhenCalled_ReturnsOkResultWithUserAsync()
    {
        // Arrange
        var userModel = _fixture.Create<OperationResult<ViewUserDto>>();

        _authAppServiceMock.Setup(service => service.UpdateAsync(It.IsAny<string>(), It.IsAny<UpdateUserDto>()))
            .ReturnsAsync(userModel);

        // Act
        var result = await _authController.UpdateMatchAsync(It.IsAny<string>(), It.IsAny<UpdateUserDto>());

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var operationResult = okResult.Value as OperationResult<ViewUserDto>;
        Assert.NotNull(operationResult);
    }
}
