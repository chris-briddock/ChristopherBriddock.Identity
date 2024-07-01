using Domain.Contracts;
namespace Application.Results;

/// <summary>
/// Represents the result of a registration attempt.
/// </summary>
public class RegisterResult : ResultBase<RegisterResult>
{
    private static readonly RegisterResult _exists = new() { UserAlreadyExists = true };

    /// <summary>
    /// Gets a result indicating that the user already exists.
    /// </summary>
    public static RegisterResult UserExists => _exists;

    /// <summary>
    /// Gets a value indicating whether the user already exists.
    /// </summary>
    public bool UserAlreadyExists { get; private set; }
}