using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(s => {
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
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        base.ConfigureClient(client);
    }
}