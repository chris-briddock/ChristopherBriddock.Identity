namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public MsSqlContainer _msSqlContainer = new MsSqlBuilder()
                                                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                                .WithWaitStrategy(Wait.ForUnixContainer())
                                                .WithAutoRemove(true)
                                                .Build();

    public IContainer _redisCacheContainer = new ContainerBuilder()
                                             .WithImage("redis:latest")
                                             .WithWaitStrategy(Wait.ForUnixContainer())
                                             .WithPortBinding(5002, true)
                                             .WithAutoRemove(true)
                                             .Build();
    public IContainer _loggingContainer = new ContainerBuilder()
                                             .WithImage("datalust/seq:latest")
                                             .WithWaitStrategy(Wait.ForUnixContainer())
                                             .WithEnvironment("ACCEPT_EULA", "Y")
                                             .WithPortBinding(5431, true)
                                             .WithAutoRemove(true)
                                             .Build();

    public IContainer _messagingContainer = new ContainerBuilder()
                                            .WithImage("rabbitmq:latest")
                                            .WithWaitStrategy(Wait.ForUnixContainer())
                                            .WithPortBinding(5672, false)
                                            .Build();


    public void StartTestContainer()
    {
        _msSqlContainer.StartAsync().Wait();
        _redisCacheContainer.StartAsync().Wait();
        _loggingContainer.StartAsync().Wait();
        _messagingContainer.StartAsync().Wait();
        Task.Delay(TimeSpan.FromSeconds(60)).Wait();
    }
    public void StopTestContainer()
    {
        _msSqlContainer.StopAsync().Wait();
        _msSqlContainer.DisposeAsync().AsTask().Wait();
        _loggingContainer.StopAsync().Wait();
        _loggingContainer.DisposeAsync().AsTask().Wait();
        _redisCacheContainer.StopAsync().Wait();
        _redisCacheContainer.DisposeAsync().AsTask().Wait();
        _messagingContainer.StopAsync().Wait();
        _messagingContainer.DisposeAsync().AsTask().Wait();
        Task.Delay(TimeSpan.FromSeconds(30)).Wait();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _msSqlContainer.GetConnectionString();

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:Default", _msSqlContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:Redis", $"localhost:{_redisCacheContainer.GetMappedPublicPort(5002)}"),
                new KeyValuePair<string, string?>("Serilog:Using:0", "Serilog.Sinks.Console"),
                new KeyValuePair<string, string?>("Serilog:Using:1", "Serilog.Sinks.Seq"),
                new KeyValuePair<string, string?>("Serilog:MinimumLevel:Default", "Information"),
                new KeyValuePair<string, string?>("Serilog:MinimumLevel:Override:Microsoft", "Information"),
                new KeyValuePair<string, string?>("Serilog:WriteTo:0:Name", "Console"),
                new KeyValuePair<string, string?>("Serilog:WriteTo:1:Name", "Seq"),
                new KeyValuePair<string, string?>("Serilog:WriteTo:1:Args:serverUrl", $"http://localhost:{_loggingContainer.GetMappedPublicPort(5431)}"),
                new KeyValuePair<string, string?>("Serilog:Enrich:0", "FromLogContext"),
                new KeyValuePair<string, string?>("Serilog:Enrich:1", "WithMachineName"),
                new KeyValuePair<string, string?>("Serilog:Enrich:2", "WithThreadId")
            ]).Build();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
    }
}