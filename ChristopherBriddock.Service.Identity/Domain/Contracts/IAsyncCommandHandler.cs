namespace Domain.Contracts;

public interface IAsyncCommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand request);
}

public interface IAsyncCommandHandler<TResult>
{
    Task<TResult> HandleAsync();
}
