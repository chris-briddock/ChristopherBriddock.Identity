namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal abstract class MockBase<T> where T : class
{
    public abstract T Mock();
}
