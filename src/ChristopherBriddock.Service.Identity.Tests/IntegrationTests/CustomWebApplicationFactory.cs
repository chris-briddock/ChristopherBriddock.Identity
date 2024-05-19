using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(s => {
            // Remove the existing Identity.Application authentication scheme
            var identitySchemeDescriptors = s
                .Where(d => d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>)).ToList();

            // Add the custom test-specific authentication scheme
            s.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });        
    }

    protected override void ConfigureClient(HttpClient client)
    {
        var token = Environment.GetEnvironmentVariable("JWT");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        base.ConfigureClient(client);
    }
}