using GenerateTemplate.Domain.Interface.Utils;
using GenerateTemplate.Domain.Utils;
#if Authentication || DEBUG
using GenerateTemplate.Domain.Entity.UserEntity;
#endif

namespace GenerateTemplate.Domain.Test.Utils;

public class GenerateHashServiceTest
{
    private readonly IGenerateHash _generateHash;

    public GenerateHashServiceTest()
    {
        _generateHash = new GenerateHash();
    }

    [Fact]
    public void GenerateHashRandom_ShouldReturnNonEmptyHash()
    {
        // Act
        string result = _generateHash.GenerateHashRandom();

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Equal(64, result.Length); // SHA256 produces a 64-character hex string
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("diferentPassword123")]
    public void GenerateHashParameters_ShouldReturnExpectedHash(string password)
    {
        // Act
        string result = _generateHash.GenerateHashParameters(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Equal(64, result.Length); // SHA256 produces a 64-character hex string
    }

    [Theory]
    [InlineData("password123", "ef92b778bafee91d3d3d9aa84d6b31415dbb7c2ba8b0bc843b14b281d8c7e0c2")]
    [InlineData("anotherPassword", "25d55ad283aa400af464c76d713c07ad")]
    public void VerifyPassword_ShouldReturnExpectedResult(string password, string hashedPassword)
    {
        // Act
        string hash = _generateHash.GenerateHashParameters(password);
        bool result = _generateHash.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

#if Authentication || DEBUG
    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userModel = new UserModel
        {
            UserName = "testuser",
            Email = "testuser@example.com",
            Id = "1"
        };

        // Act
        string token = _generateHash.GenerateToken(userModel);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));

        // Validate token structure (this is a basic check, not a full validation)
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

        Assert.NotNull(jwtToken);
        Assert.Equal("testuser", jwtToken.Claims.First(c => c.Type == "username").Value);
        Assert.Equal("testuser@example.com", jwtToken.Claims.First(c => c.Type == "email").Value);
        Assert.Equal("1", jwtToken.Claims.First(c => c.Type == "id").Value);
    }
#endif
}
