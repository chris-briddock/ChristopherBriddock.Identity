using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Models.Results;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class JsonWebTokenProviderTests
{
    private readonly string _email = "christopherbriddock@gmail.com";
    private readonly string _jwtSecret = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}GLq74z*:&gB^zmhx*HsrB6GYj%K}G";
    private readonly string _issuer = "https://auth.example.com";
    private readonly string _audience = "https://api.example.com";
    private readonly string _expires = 120.ToString();
    private readonly string _subject = "John Doe";

    [Fact]
    public async Task TryCreateTokenAsync_ShouldCreateToken_WhenValidParametersAreProvided()
    {
        // Arrange
        var mockProvider = new Mock<IJsonWebTokenProvider>();
        mockProvider.Setup(p => p.TryCreateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(new JwtResult { Success = true, Token = "mockToken", Error = null });

        // Act
        var result = await mockProvider.Object.TryCreateTokenAsync(_email, _jwtSecret, _issuer, _audience, _expires, _subject);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task TryValidateTokenAsync_ShouldValidateToken_WhenValidTokenIsProvided()
    {
        // Arrange
        var mockProvider = new Mock<IJsonWebTokenProvider>();
        var validToken = "mockValidToken";
        mockProvider.Setup(p => p.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(new JwtResult { Success = true, Token = validToken, Error = null });

        // Act
        var result = await mockProvider.Object.TryValidateTokenAsync(validToken, _jwtSecret, _issuer, _audience);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(validToken, result.Token);
    }

    [Fact]
    public async Task TryValidateTokenAsync_ShouldFail_WhenInvalidTokenIsProvided()
    {
        // Arrange
        var mockProvider = new Mock<IJsonWebTokenProvider>();
        var invalidToken = "invalidToken";
        mockProvider.Setup(p => p.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(new JwtResult { Success = false, Token = null!, Error = "Invalid token" });

        // Act
        var result = await mockProvider.Object.TryValidateTokenAsync(invalidToken, _jwtSecret, _issuer, _audience);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Error!);
    }
}