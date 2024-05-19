namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TestFixture<TProgram> : IDisposable where TProgram : class
{
    public CustomWebApplicationFactory<TProgram> WebApplicationFactory { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        WebApplicationFactory = new CustomWebApplicationFactory<TProgram>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Dispose();
    }

    public void Dispose()
    {
        
        WebApplicationFactory.Dispose();
    }
}
