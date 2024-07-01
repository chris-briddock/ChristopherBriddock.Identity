using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Application.Results;

/// <summary>
/// Represents the result of a JWT (JSON Web Token) operation.
/// </summary>
public sealed class JwtResult : ResultBase<JwtResult>
{
    /// <summary>
    /// Gets or sets the JWT (JSON Web Token) string if the operation was successful.
    /// </summary>
    public string Token { get; set; } = default!;
}
