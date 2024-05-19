namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public interface IMockBase<T> where T : class
{
    public abstract T Mock();
}
