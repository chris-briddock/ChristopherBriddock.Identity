namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Defines a contract for providing methods to generate secure random codes.
/// </summary>
public interface ICodeProvider 
{
    /// <summary>
    /// Generates a random authorization code.
    /// </summary>
    /// <param name="length">The length of the generated code (default is 4).</param>
    /// <returns>A random authorization code.</returns>
    public string Create(int length = 4);
}