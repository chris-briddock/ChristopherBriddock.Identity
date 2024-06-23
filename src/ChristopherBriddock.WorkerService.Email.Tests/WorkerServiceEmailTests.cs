namespace ChristopherBriddock.WorkerService.Email.Tests;

[TestFixture]
public class WorkerTests
{
    private Mock<ILogger<Worker>> _loggerMock;
    private Mock<IServiceProvider> _serviceProvider;
    private Mock<ISmtpClient> _smtpClientMock;
    private Mock<IConfiguration> _configurationMock;
    private Worker _worker;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<Worker>>();
        _serviceProvider = new Mock<IServiceProvider>();
        _smtpClientMock = new Mock<ISmtpClient>();
        _configurationMock = new Mock<IConfiguration>();

        _smtpClientMock.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);

        _configurationMock.SetupGet(c => c["Email:Server"]).Returns("localhost");
        _configurationMock.SetupGet(c => c["Email:Port"]).Returns("25");
        _configurationMock.SetupGet(c => c["Email:Credentials:EmailAddress"]).Returns("test@example.com");
        _configurationMock.SetupGet(c => c["Email:Credentials:Password"]).Returns("password");

        _serviceProvider.Setup(x => x.GetService(typeof(IConfiguration))!).Returns(_configurationMock.Object);
        _serviceProvider.Setup(x => x.GetService(typeof(ISmtpClient))!).Returns(_smtpClientMock.Object);
        _worker = new Worker(_loggerMock.Object, _serviceProvider.Object);
    }

    [Test]
    public async Task Consume_ConfirmEmail_SendsEmail()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            Type = EmailPublisherConstants.ConfirmEmail,
            EmailAddress = "recipient@example.com",
            Code = "https://example.com/confirm"
        };

        var consumeContextMock = new Mock<ConsumeContext<EmailMessage>>();
        consumeContextMock.Setup(c => c.Message).Returns(emailMessage);

        // Act
        await _worker.Consume(consumeContextMock.Object);

        // Assert
        _smtpClientMock.Verify(
            smtp => smtp.SendMailAsync(It.Is<MailMessage>(msg =>
                msg.To.Contains(new MailAddress("recipient@example.com")) &&
                msg.Subject == "Please confirm your email address." &&
                msg.Body.Contains($"https://example.com/confirm")
            )),
            Times.Once);
    }

    [Test]
    public async Task Consume_TwoFactorToken_SendsEmail()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            Type = EmailPublisherConstants.TwoFactorToken,
            EmailAddress = "recipient@example.com",
            Code = "123456"
        };

        var consumeContextMock = new Mock<ConsumeContext<EmailMessage>>();
        consumeContextMock.Setup(c => c.Message).Returns(emailMessage);

        // Act
        await _worker.Consume(consumeContextMock.Object);

        // Assert
        _smtpClientMock.Verify(
            smtp => smtp.SendMailAsync(It.Is<MailMessage>(msg =>
                msg.To.Contains(new MailAddress("recipient@example.com")) &&
                msg.Subject == "You requested a two factor code" &&
                msg.Body.Contains("Your two factor code is 123456")
            )),
            Times.Once);
    }

    [Test]
    public async Task Consume_ForgotPassword_SendsEmail()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            Type = EmailPublisherConstants.ForgotPassword,
            EmailAddress = "recipient@example.com",
            Code = "654321"
        };

        var consumeContextMock = new Mock<ConsumeContext<EmailMessage>>();
        consumeContextMock.Setup(c => c.Message).Returns(emailMessage);

        // Act
        await _worker.Consume(consumeContextMock.Object);

        // Assert
        _smtpClientMock.Verify(
            smtp => smtp.SendMailAsync(It.Is<MailMessage>(msg =>
                msg.To.Contains(new MailAddress("recipient@example.com")) &&
                msg.Subject == "Oh no! You silly goose, you forgot your password. You can reset it here." &&
                msg.Body.Contains("Your password reset code is <span class=\"font-bold text-indigo-800\">654321</span>")
            )),
            Times.Once);
    }
}