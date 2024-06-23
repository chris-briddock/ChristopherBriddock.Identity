namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

[TestFixture]
public class JsonWebTokenProviderTests
{
    private readonly string _email = "christopherbriddock@gmail.com";
    private readonly string _jwtSecret = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}GLq74z*:&gB^zmhx*HsrB6GYj%K}G";
    private readonly string _issuer = "https://auth.example.com";
    private readonly string _audience = "https://api.example.com";
    private readonly string _expires = 120.ToString();
    private readonly string _subject = "John Doe";

    [Test]
    public async Task TryCreateTokenAsync_ShouldCreateToken_WhenValidParametersAreProvided()
    {
        // Arrange
        var mockProvider = new JsonWebTokenProviderMock();
        mockProvider.Setup(p => p.TryCreateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new JwtResult { Success = true, Token = "mockToken", Error = null });

        // Act
        var result = await mockProvider.Object.TryCreateTokenAsync(_email, _jwtSecret, _issuer, _audience, _expires, _subject);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Token, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        });

    }

    [Test]
    public async Task TryValidateTokenAsync_ShouldValidateToken_WhenValidTokenIsProvided()
    {
        // Arrange
        var mockProvider = new JsonWebTokenProviderMock();
        var validToken = "mockValidToken";
        mockProvider.Setup(p => p.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new JwtResult { Success = true, Token = validToken, Error = null });

        // Act
        var result = await mockProvider.Object.TryValidateTokenAsync(validToken, _jwtSecret, _issuer, _audience);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.Token, Is.EqualTo(validToken));
        });

    }

    [Test]
    public async Task TryValidateTokenAsync_ShouldFail_WhenInvalidTokenIsProvided()
    {
        // Arrange
        var mockProvider = new JsonWebTokenProviderMock();
        var invalidToken = "invalidToken";
        mockProvider.Setup(p => p.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new JwtResult { Success = false, Token = null!, Error = "Invalid token" });

        // Act
        var result = await mockProvider.Object.TryValidateTokenAsync(invalidToken, _jwtSecret, _issuer, _audience);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.Not.Empty);
        });

    }
}