using ChristopherBriddock.Service.Identity.Models.Results;
using ChristopherBriddock.Service.Identity.Providers;
using System.Net.Http.Headers;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests
{
    [TestFixture]
    public class RefreshEndpointTests
    {
        private TestFixture<Program> _fixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fixture = new TestFixture<Program>();
            _fixture.OneTimeSetUp();
        }

        [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _fixture.OneTimeTearDown();
        _fixture.Dispose();
        _fixture = null!;
    }

        [Test]
        public async Task RefreshEndpoint_Returns401_WhenInvalidTokenIsUsed()
        {
            using var client = _fixture.WebApplicationFactory.CreateClient();

            var accessToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhdGVzdHlAdGVzdGluZy5jb20iLCJqdGkiOiIzNzM1ZTQwOS04MzIzLTRiN2MtYjczZC00ZWZkNWRmMmRjZWUiLCJpYXQiOiIxNzA2NDA4MjQ0IiwiZW1haWwiOiJhdGVzdHlAdGVzdGluZy5jb20iLCJleHAiOjE3MDY2MjQyNDQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0IiwiYXVkIjoiYXRlc3R5QHRlc3RpbmcuY29tIn0.YeOVALkBLkdxPgHg5EpV54tuQYgwwGPC - VitAe1Wubc";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var sut = await client.PostAsync("/refresh", null);

            Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task RefreshEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
        {
            JsonWebTokenProviderMock jsonWebTokenProviderMock = new();

            RefreshRequest refreshRequest = new() { RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs" };

            jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>())).ThrowsAsync(new Exception());

            using var sutClient = _fixture.CreateAuthenticatedClient(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
            });

            using var sut = await sutClient.PostAsJsonAsync("/refresh", refreshRequest);

            Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task RefreshEndpoint_Returns302Found_WhenRefreshIsSuccessful()
        {
            var jsonWebTokenProviderMock = new JsonWebTokenProviderMock();

            RefreshRequest refreshRequest = new() { RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs" };

            jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>())).ReturnsAsync(new JwtResult(){
                                                                            Success = true
                                                                        });


            using var refreshClient = _fixture.CreateAuthenticatedClient(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
            });

            using var sut = await refreshClient.PostAsJsonAsync("/refresh", refreshRequest); 

            Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Found));
        }

        [Test]
        public async Task RefreshEndpoint_Returns401Unauthorized_WhenTokenValidationFails()
        {
            RefreshRequest refreshRequest = new() { RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs" };

            var jsonWebTokenProviderMock = new JsonWebTokenProviderMock().Mock();

            JwtResult jwtResult = new()
            {
                Success = false
            };

            jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>())).ReturnsAsync(jwtResult);

            using var refreshClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
            {
                s.ConfigureTestServices(s =>
                {
                    s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
                });
            }).CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            using var sut = await refreshClient.PostAsJsonAsync("/refresh", refreshRequest);

            Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}