namespace ChristopherBriddock.Service.Identity.Exceptions;


/// <summary>
/// Exception class for errors related to creating JSON Web Tokens (JWT).
/// </summary>
public class CreateJwtException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJwtException"/> class.
    /// </summary>
    public CreateJwtException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJwtException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    public CreateJwtException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJwtException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    /// <param name="inner">The inner exception that caused this exception.</param>
    public CreateJwtException(string message, Exception inner)
        : base(message, inner)
    {
    }
}