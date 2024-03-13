
using ChristopherBriddock.Service.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class WebApplicationFactoryCustom : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql;
    public WebApplicationFactoryCustom()
    {
        _postgreSql = new PostgreSqlBuilder()
                    .WithImage("postgres:latest")
                    .WithDatabase("ChristopherBriddock.Service.Identity")
                    .WithUsername("postgres")
                    .WithPassword("pass123")
                    .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => {
            var descriptor = services.
            SingleOrDefault(sd => sd.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(s => {
                s.UseNpgsql(_postgreSql.GetConnectionString());
            });
        });
        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _postgreSql.StartAsync();
    }
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSql.StopAsync();
    }
}