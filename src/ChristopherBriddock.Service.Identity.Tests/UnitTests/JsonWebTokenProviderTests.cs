using ChristopherBriddock.Service.Identity.Providers;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class JsonWebTokenProviderTests
{
    private readonly JsonWebTokenProvider _sut;
    private readonly string _email = "christopherbriddock@gmail.com";
    private readonly string _jwtSecret = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}GLq74z*:&gB^zmhx*HsrB6GYj%K}G";
    private readonly string _issuer = "https://auth.example.com";
    private readonly string _audience = "https://api.example.com";
    private readonly string _expires = 120.ToString();
    private readonly string _subject = "John Doe";

    public JsonWebTokenProviderTests()
    {
        // Arrange

        _sut = new JsonWebTokenProvider();
    }

    [Fact]
    public async Task TryCreateTokenAsync_ShouldCreateToken_WhenValidParametersAreProvided()
    {
        // Act
        var result = await _sut.TryCreateTokenAsync(_email,
                                                    _jwtSecret,
                                                    _issuer,
                                                    _audience,
                                                    _expires,
                                                    _subject);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.Null(result.Error);
    }
    [Fact]
    public async Task TryValidateTokenAsync_ShouldValidateToken_WhenValidTokenIsProvided()
    {
        // Arrange
        var jwtResult = await _sut.TryCreateTokenAsync(_email,
                                                      _jwtSecret,
                                                      _issuer,
                                                      _audience,
                                                      _expires,
                                                      _subject);
        var validToken = jwtResult.Token;

        // Act
        var result = await _sut.TryValidateTokenAsync(validToken,
                                                     _jwtSecret,
                                                     _issuer,
                                                     _audience);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(validToken, result.Token);
    }
    [Fact]
    public async Task TryValidateTokenAsync_ShouldFail_WhenInvalidTokenIsProvided()
    {
        // Arrange
        var invalidToken = "invalidToken";

        // Act
        var result = await _sut.TryValidateTokenAsync(invalidToken, _jwtSecret, _issuer, _audience);

        // Assert
        Assert.False(result.Success);
    }
}
