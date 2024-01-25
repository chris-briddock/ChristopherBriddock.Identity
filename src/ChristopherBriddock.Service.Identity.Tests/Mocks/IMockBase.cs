namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal interface IMockBase<T> where T : class
{
    public abstract T Mock();
}
