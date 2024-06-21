using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> :  WebApplicationFactory<TProgram> where TProgram : class
{
    public MsSqlContainer _msSqlContainer = new MsSqlBuilder()
                                                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                                .WithWaitStrategy(Wait.ForUnixContainer())
                                                .WithAutoRemove(true)
                                                .Build();
                                                    
    public void StartTestContainer() 
    {
        _msSqlContainer.StartAsync().Wait();
        Task.Delay(TimeSpan.FromSeconds(30)).Wait();
    }
    public void StopTestContainer() 
    {
        _msSqlContainer.StopAsync().Wait();
        _msSqlContainer.DisposeAsync().AsTask().Wait();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _msSqlContainer.GetConnectionString();

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:Default", _msSqlContainer.GetConnectionString())
            ]).Build();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        try
        {
                // Get the current directory and navigate to the desired directory
                string currentDirectory = Directory.GetCurrentDirectory();
                string desiredDirectory = Path.Combine(
                currentDirectory, 
                "..", "..", "..", "..", 
                "ChristopherBriddock.Service.Identity"
            );

                // Ensure the path is properly formatted
                desiredDirectory = Path.GetFullPath(desiredDirectory);
                // Create a new process info
                ProcessStartInfo psi = new()
                {
                    FileName = "dotnet",
                    Arguments = "user-jwts create --output json --audience http://localhost",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true, 
                    WorkingDirectory = desiredDirectory
                };

                // Create and start the process
                using Process process = new() { StartInfo = psi };
                process.Start();

                // Read the standard output
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                JObject jwtJson = JObject.Parse(output);
                string token = jwtJson["Token"]!.ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Optionally, read and handle standard error
            string error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }
        catch (Exception)
        {
            throw;
        }

        base.ConfigureClient(client);
    }
}