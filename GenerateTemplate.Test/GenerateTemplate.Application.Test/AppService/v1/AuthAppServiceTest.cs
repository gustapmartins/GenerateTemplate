using AutoFixture;
using AutoMapper;
using GenerateTemplate.Application.AppServices.v1;
using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Enum;
using GenerateTemplate.Domain.Interface.Services.v1;
using Moq;

namespace GenerateTemplate.Application.Test.AppService.v1;

public class AuthAppServiceTest
{
    private readonly Fixture _fixture;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthAppService _authAppService;

    public AuthAppServiceTest()
    {
        _fixture = new Fixture();
        _mapperMock = new Mock<IMapper>();
        _authServiceMock = new Mock<IAuthService>();
        _authAppService = new AuthAppService(_mapperMock.Object, _authServiceMock.Object);
    }

    [Fact]
    [Trait("AuthAppService", "Autenticação dentro da aplicação")]
    public async Task Login_ReturnsToken_SucessAsync()
    {
        // Arrange
        var users = _fixture.Create<OperationResult<string>>();

        _mapperMock.Setup(mapper => mapper.Map<UserModel>(It.IsAny<UserModel>()))
            .Returns(_fixture.Create<UserModel>());

        _authServiceMock.Setup(service => service.Login(It.IsAny<UserModel>()))
            .ReturnsAsync(users);

        // Act
        var result = await _authAppService.LoginAsync(It.IsAny<LoginDto>());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users.Content, result.Content);

        _mapperMock.Verify(dao => dao.Map<UserModel>(It.IsAny<UserModel>()), Times.Once);
        _authServiceMock.Verify(dao => dao.Login(It.IsAny<UserModel>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Criação do usuário dentro da aplicação")]
    public async Task CreateAsync_ReturnsNewUser_SucessAsync()
    {
        // Arrange
        _fixture.Customize<CreateUserDto>(composer => composer
           .With(r => r.Username, "ValidUsername")
           .With(r => r.Email, "valid@example.com")
           .With(r => r.Password, "Valid1234")
           .With(r => r.ConfirmPassword, "Valid1234")
           .With(r => r.Cpf, "123.456.789-09")
           .With(r => r.Role, Role.Admin)); // Adjust Role to a valid enum value

        var createUserDto = _fixture.Create<CreateUserDto>(); // Generate the DTO instance
        var mapperCreatedUserDto = new OperationResult<CreateUserDto> { Content = createUserDto }; // Simulate the response from service

        _mapperMock.Setup(mapper => mapper.Map<UserModel>(It.IsAny<CreateUserDto>()))
            .Returns(_fixture.Create<UserModel>());

        _authServiceMock.Setup(service => service.CreateAsync(It.IsAny<UserModel>()))
            .ReturnsAsync(_fixture.Create<OperationResult<UserModel>>());

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<CreateUserDto>>(It.IsAny<OperationResult<UserModel>>()))
            .Returns(mapperCreatedUserDto);

        // Act
        var result = await _authAppService.CreateAsync(It.IsAny<CreateUserDto>());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mapperCreatedUserDto.Content, result.Content);

        _authServiceMock.Verify(dao => dao.CreateAsync(It.IsAny<UserModel>()), Times.Once);
        _mapperMock.Verify(dao => dao.Map<UserModel>(It.IsAny<CreateUserDto>()), Times.Once);
        _mapperMock.Verify(dao => dao.Map<OperationResult<CreateUserDto>>(It.IsAny<OperationResult<UserModel>>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Recuperar todos os usuarios com base no valor da paginação informada")]
    public async Task GetAlltAsyncAsync_ReturnsUsers_SucessAsync()
    {
        // Arrange
        var mapperGetAsync = _fixture.Create<OperationResult<IEnumerable<ViewUserDto>>>(); // Simulate the response from service

        _authServiceMock.Setup(service => service.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<OperationResult<IEnumerable<UserModel>>>());

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<IEnumerable<ViewUserDto>>>(It.IsAny<OperationResult<IEnumerable<UserModel>>>()))
            .Returns(mapperGetAsync);

        // Act
        var result = await _authAppService.GetAsync(It.IsAny<int>(), It.IsAny<int>());

        Assert.NotNull(result);
        Assert.Equal(3, result.Content.Count());

        _authServiceMock.Verify(dao => dao.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Recuperar o usuario com base no id dele")]
    public async Task GetIdAsyncAsync_ReturnUser_SucessAsync()
    {
        // Arrange
        var mapperGetAllAsync = _fixture.Create<OperationResult<ViewUserDto>>(); // Simulate the response from service

        _authServiceMock.Setup(service => service.GetIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<OperationResult<UserModel>>());

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<ViewUserDto>>(It.IsAny<OperationResult<UserModel>>()))
            .Returns(mapperGetAllAsync);

        // Act
        var result = await _authAppService.GetIdAsync(It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Equal(mapperGetAllAsync.Content, result.Content);

        _authServiceMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Recuperar o usuario com base no id dele e bloquear a conta")]
    public async Task RemoveAsync_RemoveUser_SucessAsync()
    {
        // Arrange
        var mapperRemoveAsync = _fixture.Create<OperationResult<ViewUserDto>>(); // Simulate the response from service

        _authServiceMock.Setup(service => service.RemoveAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<OperationResult<UserModel>>());

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<ViewUserDto>>(It.IsAny<OperationResult<UserModel>>()))
            .Returns(mapperRemoveAsync);

        // Act
        var result = await _authAppService.RemoveAsync(It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Equal(mapperRemoveAsync.Content, result.Content);

        _authServiceMock.Verify(dao => dao.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Recuperar o usuario com base no id dele e deve alterar os dados da conta dele")]
    public async Task UpdateAsync_UpdateUser_SucessAsync()
    {
        // Arrange
        var mapperRemoveAsync = _fixture.Create<OperationResult<ViewUserDto>>(); // Simulate the response from service

        _mapperMock.Setup(mapper => mapper.Map<UserModel>(It.IsAny<UpdateUserDto>()))
          .Returns(_fixture.Create<UserModel>());

        _authServiceMock.Setup(service => service.UpdateAsync(It.IsAny<string>(), It.IsAny<UserModel>()))
            .ReturnsAsync(_fixture.Create<OperationResult<UserModel>>());

        _mapperMock.Setup(mapper => mapper.Map<OperationResult<ViewUserDto>>(It.IsAny<OperationResult<UserModel>>()))
            .Returns(mapperRemoveAsync);

        // Act
        var result = await _authAppService.RemoveAsync(It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Equal(mapperRemoveAsync.Content, result.Content);


        _authServiceMock.Verify(dao => dao.RemoveAsync(It.IsAny<string>()), Times.Once);
        _mapperMock.Verify(dao => dao.Map<OperationResult<ViewUserDto>>(It.IsAny<OperationResult<UserModel>>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Codigo enviado no email para redefinição de senha")]
    public async Task ForgetPasswordAsync_ReturnsCodeEmail_SucessAsync()
    {
        // Arrange
        var mapperForgetPassword = _fixture.Create<OperationResult<string>>(); // Simulate the response from service

        _authServiceMock.Setup(service => service.ForgetPassword(It.IsAny<string>()))
            .ReturnsAsync(mapperForgetPassword);

        // Act
        var result = await _authAppService.ForgetPasswordAsync(It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Equal(mapperForgetPassword.Content, result.Content);

        _authServiceMock.Verify(dao => dao.ForgetPassword(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Deve resetar a senha do usuario")]
    public void VerificationPasswordOTP_ResetPasswordUser_SucessAsync()
    {
        // Arrange
        var mapperForgetPassword = _fixture.Create<OperationResult<string>>(); // Simulate the response from service

        _authServiceMock.Setup(service => service.VerificationPasswordOTP(It.IsAny<string>()))
            .Returns(mapperForgetPassword);

        // Act
        var result = _authAppService.VerificationPasswordOTP(It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Equal(mapperForgetPassword.Content, result.Content);

        _authServiceMock.Verify(dao => dao.VerificationPasswordOTP(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AuthAppService", "Deve resetar a senha do usuario")]
    public async Task ResetPasswordAsync_ResetPasswordUser_SucessAsync()
    {
        // Arrange
        var mapperForgetPassword = _fixture.Create<OperationResult<string>>(); // Simulate the response from service

        _mapperMock.Setup(mapper => mapper.Map<PasswordReset>(It.IsAny<PasswordResetDto>()))
            .Returns(_fixture.Create<PasswordReset>());

        _authServiceMock.Setup(service => service.ResetPassword(It.IsAny<PasswordReset>()))
            .ReturnsAsync(mapperForgetPassword);

        // Act
        var result = await _authAppService.ResetPasswordAsync(It.IsAny<PasswordResetDto>());

        Assert.NotNull(result);
        Assert.Equal(mapperForgetPassword.Content, result.Content);

        _authServiceMock.Verify(dao => dao.ResetPassword(It.IsAny<PasswordReset>()), Times.Once);
    }
}
