using AutoFixture;
using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Enum;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Domain.Interface.Services;
using GenerateTemplate.Domain.Interface.Services.v1;
using GenerateTemplate.Domain.Interface.Utils;
using GenerateTemplate.Domain.Services.v1;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Moq;

namespace GenerateTemplate.Domain.Test.Services.v1;

public class AuthServiceTest
{
    private readonly Fixture _fixture;
    private readonly Mock<IAuthDao> _authDaoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContext;
    private readonly Mock<IGetClientIdToken> _getClientIdToken;
    private readonly Mock<IMemoryCacheService> _memoryCacheServiceMock;
    private readonly Mock<IGenerateHash> _generateHashMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly IAuthService _authServiceMock;
    private readonly Mock<INotificationBase> _notificationBase;

    public AuthServiceTest()
    {
        _fixture = new Fixture();
        _authDaoMock = new Mock<IAuthDao>();
        _emailServiceMock = new Mock<IEmailService>();
        _memoryCacheServiceMock = new Mock<IMemoryCacheService>();
        _generateHashMock = new Mock<IGenerateHash>();
        _getClientIdToken = new Mock<IGetClientIdToken>(); // Initialize the mock
        _httpContext = new Mock<IHttpContextAccessor>(); // Initialize the mock
        _redisServiceMock = new Mock<IRedisService>();
        _notificationBase = new Mock<INotificationBase>();

        _authServiceMock = new AuthService(
            _authDaoMock.Object,
            _generateHashMock.Object,
            _emailServiceMock.Object,
            _memoryCacheServiceMock.Object,
            _httpContext.Object, // Pass the initialized mock
            _getClientIdToken.Object, // Pass the initialized mock
            _redisServiceMock.Object,
            _notificationBase.Object
        );
    }

    [Fact]
    public async Task GetAsync_WhenAuthExist_ReturnsAuthListAsync()
    {
        // Arrange
        List<UserModel> userList = _fixture.CreateMany<UserModel>(2).ToList();
        UserModel userModel = _fixture.Create<UserModel>();

        _authDaoMock.Setup(dao => dao.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FilterDefinition<UserModel>>())).ReturnsAsync(userList);

        //Act
        var result = await _authServiceMock.GetAllAsync(1, 10);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Content.Count());
        Assert.Collection(result.Content,
                item => Assert.Equal(userList[0].Id, item.Id),
                item => Assert.Equal(userList[1].Id, item.Id)
            );
        _authDaoMock.Verify(dao => dao.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FilterDefinition<UserModel>>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenNoAuthsExist_ThrowsExceptionAsync()
    {
        // Arrange
        _authDaoMock.Setup(dao => dao.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), null))
                   .ReturnsAsync(Enumerable.Empty<UserModel>());

        // Act and Assert
        var result = await _authServiceMock.GetAllAsync(It.IsAny<int>(), It.IsAny<int>());

        Assert.NotNull(result);
        Assert.Empty(result.Content);
    }

    [Fact]
    public async Task GetIdAsync_WhenAuthExist_ReturnsAuthAsync()
    {
        // Arrange
        UserModel userMock = _fixture.Build<UserModel>()
                               .With(x => x.AccountStatus, AccountStatus.Active)
                               .Create();

        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        //Act
        var result = await _authServiceMock.GetIdAsync(It.IsAny<string>());

        //Assert
        Assert.NotNull(result);
        Assert.Equal(userMock, result.Content);
        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetIdAsync_WhenNotAuthExist_ThrowsExceptionAsync()
    {
        // Arrange
        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>()))!.ReturnsAsync(null as UserModel);

        // Act
        var result = await _authServiceMock.GetIdAsync(It.IsAny<string>());

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Content);
        Assert.Equal($"Usuário com id:  não existe.", result.Message);
        
        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenAuthExist_ReturnsAuthAsync()
    {

        // Arrange
        UserModel userMock = _fixture.Build<UserModel>()
                               .With(x => x.Id, string.Empty)
                               .With(x => x.AccountStatus, AccountStatus.Active)
                               .Create();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(It.IsAny<string>()))!.ReturnsAsync(null as UserModel);

        _authDaoMock.Setup(dao => dao.CreateAsync(It.IsAny<UserModel>()));

        //Act
        var createdUser = await _authServiceMock.CreateAsync(userMock);

        //Assert
        Assert.Equal(userMock, createdUser.Content);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
        _authDaoMock.Verify(dao => dao.CreateAsync(It.IsAny<UserModel>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenAuthExist_ThrowsExceptionAsync()
    {
        // Arrange
        UserModel userMock = _fixture.Create<UserModel>();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        //Act
        var createdUser = await _authServiceMock.CreateAsync(userMock);

        //Assert
        Assert.Equal($"Usuário com esse email: '{userMock.Email}', já existe.", createdUser.Message);
        Assert.False(createdUser.Status);
        Assert.Equal(StatusCodes.Status409Conflict, createdUser.StatusCode);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenLogin_ReturnTokenAsync()
    {
        //Arrange
        UserModel userMock = _fixture.Create<UserModel>();
        string token = _fixture.Create<string>();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        _generateHashMock.Setup(hash => hash.VerifyPassword(userMock.Password, userMock.Password)).Returns(true);

        _generateHashMock.Setup(hash => hash.GenerateToken(userMock)).Returns(token);

        //Act
        var result = await _authServiceMock.LoginAsync(userMock);

        //Assert
        Assert.Equal(token, result.Content);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
        _generateHashMock.Verify(hash => hash.VerifyPassword(userMock.Password, userMock.Password), Times.Once);
        _generateHashMock.Verify(hash => hash.GenerateToken(userMock), Times.Once);
    }

    [Fact]
    public async Task LoginFindEmailNotExistAsync_WhenLogin_ThrowsExceptionAsync()
    {
        //Arranges
        UserModel userMock = _fixture.Create<UserModel>();
        string token = _fixture.Create<string>();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(It.IsAny<string>()))!.ReturnsAsync(null as UserModel);

        //Act
        var result = await _authServiceMock.LoginAsync(userMock);

        //Assert
        Assert.Equal($"Este email: {userMock.Email} não existe.", result.Message);
        Assert.False(result.Status);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginVerifyPasswordAsync_WhenLogin_ThrowsExceptionAsync()
    {
        //Arrange
        UserModel userMock = _fixture.Create<UserModel>();
        string token = _fixture.Create<string>();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        _generateHashMock.Setup(hash => hash.VerifyPassword(userMock.Password, userMock.Password)).Returns(false);

        //Act
        var result = await _authServiceMock.LoginAsync(userMock);

        //Assert
        Assert.Equal($"Senha incorreta.", result.Message);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
        _generateHashMock.Verify(hash => hash.VerifyPassword(userMock.Password, userMock.Password), Times.Once);
    }

    [Fact]
    public async Task ForgetPassword_WhenEmailIsValid_SendsEmailAndStoresTokenInCacheAsync()
    {
        // Arrange
        string email = "test@example.com";
        int token = 879684;
        var user = _fixture.Create<UserModel>();

        _authDaoMock.Setup(dao => dao.FindEmailAsync(email)).ReturnsAsync(user);
        _generateHashMock.Setup(hash => hash.GenerateRandomNumber()).Returns(token);

        // Act
        var result = await _authServiceMock.ForgetPasswordAsync(email);

        // Assert
        Assert.Equal(token.ToString(), result.Content);
        _emailServiceMock.Verify(service => service.SendMailAsync(It.IsAny<string>(), email, It.IsAny<string>(), It.Is<string>(msg => msg.Contains(token.ToString()))), Times.Once);
        _memoryCacheServiceMock.Verify(cache => cache.AddToCache(token.ToString(), It.IsAny<UserModel>(), 5), Times.Once);
    }

    [Fact]
    public async Task ForgetPassword_WhenEmailIsInvalid_ThrowsExceptionAsync()
    {
        // Arrange
        string email = "invalid@example.com";

        _authDaoMock.Setup(dao => dao.FindEmailAsync(email))!.ReturnsAsync(null as UserModel);

        // Act and Assert
        var result = await _authServiceMock.ForgetPasswordAsync(email);

        Assert.Equal($"Este E-mail: {email} não é válido", result.Message);
        _authDaoMock.Verify(dao => dao.FindEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void VerificationPasswordOTP_WhenTokenEmail_TokenConfirmationSentemailAsync()
    {
        UserModel user = _fixture.Create<UserModel>();
        string code = "123456789codeGenerate";
        string tokenEmail = "8623405";

        _memoryCacheServiceMock.Setup(cache => cache.GetCache<UserModel>(code)).Returns(user);
        _generateHashMock.Setup(hash => hash.GenerateToken(It.IsAny<UserModel>())).Returns(tokenEmail);
        _memoryCacheServiceMock.Setup(hash => hash.RemoveFromCache<UserModel>(code));

        OperationResult<string> result = _authServiceMock.VerificationPasswordOTP(code);

        Assert.Equal(tokenEmail, result.Content);
        _memoryCacheServiceMock.Verify(cache => cache.GetCache<UserModel>(code), Times.Once);
        _generateHashMock.Verify(hash => hash.GenerateToken(It.IsAny<UserModel>()), Times.Once);
        _memoryCacheServiceMock.Verify(cache => cache.RemoveFromCache<UserModel>(code), Times.Never);
    }

    [Fact]
    public void VerificationPasswordOTP_WhenTokenEmail_ThrowsExceptionAsync()
    {
        _memoryCacheServiceMock.Setup(cache => cache.GetCache<UserModel>(It.IsAny<string>())).Returns(null as UserModel);

        // Act and Assert
        OperationResult<string> result = _authServiceMock.VerificationPasswordOTP(It.IsAny<string>());

        Assert.Equal($"Este token está expirado", result.Message);
        Assert.False(result.Status);
        Assert.Null(result.Content);
        _memoryCacheServiceMock.Verify(cache => cache.GetCache<UserModel>(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_WhenTokenIsValid_UpdatesPasswordAndRemovesTokenFromCacheAsync()
    {
        // Arrange
        string clientId = "validClientId";
        string newPassword = "newPassword";
        var passwordReset = new PasswordReset { Password = newPassword };
        var user = _fixture.Create<UserModel>();

        _getClientIdToken.Setup(token => token.GetClientIdFromToken(It.IsAny<HttpContext>())).Returns(clientId);
        _authDaoMock.Setup(dao => dao.GetIdAsync(clientId)).ReturnsAsync(user);
        _generateHashMock.Setup(hash => hash.GenerateHashParameters(newPassword)).Returns("hashedPassword");
        _authDaoMock.Setup(dao => dao.UpdateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).ReturnsAsync(user);

        // Act
        var result = await _authServiceMock.ResetPasswordAsync(passwordReset);

        // Assert
        Assert.Equal("Senha Redefinida", result.Message);
        _authDaoMock.Verify(dao => dao.UpdateAsync(user.Id, It.Is<Dictionary<string, object>>(dict => dict[nameof(passwordReset.Password)].Equals("hashedPassword"))), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_WhenTokenIsInvalid_ThrowsExceptionAsync()
    {
        // Arrange
        string clientId = "invalidClientId";
        string newPassword = "newPassword";
        var passwordReset = new PasswordReset { Password = newPassword };
        var user = _fixture.Create<UserModel>();

        _getClientIdToken.Setup(token => token.GetClientIdFromToken(It.IsAny<HttpContext>())).Returns(clientId);
        _authDaoMock.Setup(dao => dao.GetIdAsync(clientId))!.ReturnsAsync(null as UserModel);

        // Act and Assert
        var passwordResetResult = await _authServiceMock.ResetPasswordAsync(passwordReset);
        Assert.Equal($"Este clientId: {clientId} não é válido", passwordResetResult.Message);
        Assert.False(passwordResetResult.Status);

        _getClientIdToken.Verify(token => token.GetClientIdFromToken(It.IsAny<HttpContext>()), Times.Once);
        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenTeamsExist_ReturnsTeamAsync()
    {
        UserModel userMock = _fixture.Build<UserModel>()
                               .With(x => x.AccountStatus, AccountStatus.Active)
                               .Create();

        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        _authDaoMock.Setup(dao => dao.UpdateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
             .ReturnsAsync(userMock);

        //Act
        var removedTeam = await _authServiceMock.RemoveAsync(It.IsAny<string>());

        //Assert
        Assert.Equal(userMock, removedTeam.Content);
        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
        _authDaoMock.Verify(dao => dao.UpdateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenNotAuthExist_ThrowsExceptionAsync()
    {
        // Arrange
        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>()))!.ReturnsAsync(null as UserModel);

        //Act
        var removedTeam = await _authServiceMock.RemoveAsync(It.IsAny<string>());

        //Assert
        Assert.Equal($"Usuário com id: {It.IsAny<string>()} não existe.", removedTeam.Message);
        Assert.False(removedTeam.Status);

        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAuthExist_ReturnsAuthAsync()
    {
        // Arrange
        UserModel userMock = _fixture.Build<UserModel>()
                                .With(x => x.AccountStatus, AccountStatus.Active)
                                .Create();
        UserModel upadteUserMock = _fixture.Build<UserModel>()
                                .With(x => x.AccountStatus, AccountStatus.Blocked)
                                .Create(); ;

        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>())).ReturnsAsync(userMock);

        _authDaoMock.Setup(dao => dao.UpdateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(upadteUserMock);

        //Act
        var service = await _authServiceMock.UpdateAsync(It.IsAny<string>(), upadteUserMock);

        //Assert
        Assert.NotNull(service);
        Assert.Equal(upadteUserMock, service.Content);

        _authDaoMock.Verify(dao => dao.UpdateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotAuthExist_ThrowsExceptionAsync()
    {
        // Arrange
        _authDaoMock.Setup(dao => dao.GetIdAsync(It.IsAny<string>()))!.ReturnsAsync(null as UserModel);

        //Act
        var service = await _authServiceMock.UpdateAsync(It.IsAny<string>(), It.IsAny<UserModel>());

        //Assert
        Assert.Equal($"Usuário com id: {It.IsAny<string>()} não existe.", service.Message);
        Assert.False(service.Status);
        Assert.Null(service.Content);
        _authDaoMock.Verify(dao => dao.GetIdAsync(It.IsAny<string>()), Times.Once);
    }
}
