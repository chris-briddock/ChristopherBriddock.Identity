using ChristopherBriddock.Service.Identity.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

[TestFixture]
public class ConfigureJwtBearerOptionsTests
{
    private JwtBearerOptions _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new JwtBearerOptions();
    }

    [Test]
    public void Configure_Should_Set_Issuer_Correctly()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" }
            }).Build();

        var jwtOptionsConfigurer = new ConfigureJwtBearerOptions(configurationBuilder);

        jwtOptionsConfigurer.Configure(_sut);

        Assert.That(_sut.TokenValidationParameters.ValidateIssuer, Is.True);
        Assert.That(_sut.TokenValidationParameters.ValidIssuer, Is.EqualTo("https://localhost"));
    }

    [Test]
    public void Configure_Should_Set_Secret_Correctly()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" }
            }).Build();

        var jwtOptionsConfigurer = new ConfigureJwtBearerOptions(configurationBuilder);

        jwtOptionsConfigurer.Configure(_sut);
        var symmetricKey = (SymmetricSecurityKey)_sut.TokenValidationParameters.IssuerSigningKey;

        Assert.That(_sut.TokenValidationParameters.IssuerSigningKey, Is.Not.Null);
        Assert.That(Encoding.UTF8.GetString(symmetricKey.Key), Is.EqualTo(configurationBuilder.GetSection("Jwt:Secret").Value));
    }

    [Test]
    public void Configure_Should_Set_Audience_Correctly()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "chris@chris.com" }
            }).Build();

        var jwtOptionsConfigurer = new ConfigureJwtBearerOptions(configurationBuilder);

        jwtOptionsConfigurer.Configure(_sut);

        Assert.That(_sut.TokenValidationParameters.ValidAudience, Is.Not.Null);
        Assert.That(_sut.TokenValidationParameters.ValidAudience, Is.EqualTo(configurationBuilder.GetSection("Jwt:Audience").Value));
    }
}