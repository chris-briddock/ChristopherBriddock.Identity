namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Gets the URL provided by the <see cref="HttpContext"/>
/// </summary>
public interface ILinkProvider
{
    /// <summary>
    /// Builds a string from <see cref="HttpContext"/> and the provided <see cref="RouteValueDictionary"/>
    /// </summary>
    /// <param name="context">The http context</param>
    /// <param name="endpointName">The endpoint to generate this uri for.</param>
    /// <param name="routeValues"></param>
    /// <returns>The uri string.</returns>
    public string GetUri(HttpContext context, string endpointName, RouteValueDictionary routeValues);
}