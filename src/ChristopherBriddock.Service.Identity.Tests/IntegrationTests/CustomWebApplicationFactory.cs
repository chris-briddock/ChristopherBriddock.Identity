using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
           var descriptors = services
            .Where(d =>
                d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>)).ToList();

            for (int i = 1; i < descriptors.Count; i++)
            {
                services.Remove(descriptors[i]);
            }

            // Add the test-specific authentication scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Add custom auth handler for the re-added Identity.Application scheme
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Identity.Application", options => { });

            // Configure the authorization policy
            services.AddAuthorizationBuilder()
                    .AddPolicy("UserRolePolicy", policy => policy.RequireRole("User"));
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs");
        base.ConfigureClient(client);
    }
}