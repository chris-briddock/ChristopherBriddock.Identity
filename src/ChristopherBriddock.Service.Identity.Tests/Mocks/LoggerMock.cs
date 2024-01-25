using Microsoft.Extensions.Logging;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class LoggerMock<T> : Mock<ILogger<T>>, IMockBase<LoggerMock<T>> where T : class
{
    public LoggerMock<T> Mock()
    {
        return this;
    }
}
