using System.Diagnostics.CodeAnalysis;

namespace ChristopherBriddock.Service.Identity.Exceptions;

/// <summary>
/// Exception class for errors related to the JWT secret being null or empty.
/// </summary>
[ExcludeFromCodeCoverage]
public class JwtSecretNullOrEmptyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JwtSecretNullOrEmptyException"/> class.
    /// </summary>
    public JwtSecretNullOrEmptyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtSecretNullOrEmptyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    public JwtSecretNullOrEmptyException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtSecretNullOrEmptyException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public JwtSecretNullOrEmptyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}