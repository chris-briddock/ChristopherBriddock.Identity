using Microsoft.Extensions.Logging;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class LoggerMock<T> : MockBase<ILogger<T>> where T : class
{
    public override ILogger<T> Mock()
    {
        return Substitute.For<ILogger<T>>();
    }
}
