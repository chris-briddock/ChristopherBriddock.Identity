using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

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
        Task.Delay(TimeSpan.FromSeconds(15)).Wait();
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
        base.ConfigureClient(client);
    }
}