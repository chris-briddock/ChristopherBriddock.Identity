namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TestFixture<TProgram> : IDisposable where TProgram : class
{
    public CustomWebApplicationFactory<TProgram> WebApplicationFactory { get; set; }
    public string AccessToken { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        WebApplicationFactory = new CustomWebApplicationFactory<TProgram>();
        WebApplicationFactory.StartTestContainer();
        AccessToken = AuthorizeAsync().GetAwaiter().GetResult();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        WebApplicationFactory.StopTestContainer();
        Dispose();
    }

    public void Dispose()
    {
        WebApplicationFactory.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task<string> AuthorizeAsync()
    {
        HttpClient client = WebApplicationFactory.CreateClient();

        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "authorizeTest@default.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            RememberMe = true
        };
        // drops cookie upon successful auth to authenticate against /token endpoint.
        await client.PostAsJsonAsync("/authorize", authorizeRequest);
        HttpResponseMessage responseMessage = await client.GetAsync("/token?token_type=resource_owner");
        responseMessage.EnsureSuccessStatusCode();

        string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
        TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        return tokenResponse.AccessToken;
    }

    public HttpClient CreateAuthenticatedClient(Action<IServiceCollection> configureServices = null!)
    {
        var client = WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                configureServices?.Invoke(services);
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false    
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        return client;
    }
}