﻿using ChristopherBriddock.Service.Identity.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class ConfigureJwtBearerOptionsTests
{
    public JwtBearerOptions _sut;
    public ConfigureJwtBearerOptionsTests()
    {
        _sut = new JwtBearerOptions();
    }
    [Fact]
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

        Assert.True(_sut.TokenValidationParameters.ValidateIssuer);
        Assert.Equal("https://localhost", _sut.TokenValidationParameters.ValidIssuer);
    }
    [Fact]
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

        Assert.NotNull(_sut.TokenValidationParameters.IssuerSigningKey);

        Assert.Equal(configurationBuilder.GetSection("Jwt:Secret").Value, Encoding.UTF8.GetString(symmetricKey.Key));
    }
    [Fact]
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
        Assert.NotNull(_sut.TokenValidationParameters.ValidAudience);
        Assert.Equal(configurationBuilder.GetSection("Jwt:Audience").Value, _sut.TokenValidationParameters.ValidAudience);
    }


}
