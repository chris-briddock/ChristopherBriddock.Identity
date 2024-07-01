using Microsoft.AspNetCore.Identity;

namespace Application.Results;

/// <summary>
/// Represents a base class for returning a result.
/// </summary>
/// <typeparam name="TResult">The result class type</typeparam>
public abstract class ResultBase<TResult> where TResult : ResultBase<TResult>, new()
{
    private static readonly TResult _success = new() { Succeeded = true };
    private static readonly TResult _failed = new() { Succeeded = false };
    private List<IdentityError> _errors = [];

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static TResult Success => _success;

    /// <summary>
    /// Creates a failed result with the specified errors.
    /// </summary>
    /// <param name="errors">The errors that occurred during the attempt.</param>
    /// <returns>A failed result containing the specified errors.</returns>
    public static TResult Failed(params IdentityError[] errors)
    {
        var result = _failed;

        if (errors is not null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Succeeded { get; protected set; } = false;

    /// <summary>
    /// Gets the errors that occurred during the operation.
    /// </summary>
    public IList<string> Errors => _errors.Select(e => e.Description).ToList();
}