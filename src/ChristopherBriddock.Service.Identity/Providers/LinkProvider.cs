using System.Text;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Gets the URL provided by the <see cref="HttpContext"/>
/// </summary>
public class LinkProvider  : ILinkProvider
{
    /// <summary>
    /// Gets a built string from <see cref="HttpContext"/> and provided <see cref="RouteValueDictionary"/>
    /// </summary>
    /// <param name="context">The http context</param>
    /// <param name="endpointName">The endpoint to generate this uri for.</param>
    /// <param name="routeValues"></param>
    /// <returns>The uri string.</returns>
    public string GetUri(HttpContext context, string endpointName, RouteValueDictionary routeValues)
    {
        string queryParameters = BuildQueryParameters(routeValues);

        return $"{context.Request.Scheme}://{context.Request.Host}/{endpointName}{queryParameters}";
    }

    /// <summary>
    /// Builds up a query string.
    /// </summary>
    /// <param name="routeValues">The route values for the uri.</param>
    /// <returns>A built string.</returns>
    public string BuildQueryParameters(RouteValueDictionary routeValues)
    {
        StringBuilder sb = new();
        string queryString;
        foreach (var parameter in routeValues.Values)
        {
            sb.Append(string.Join("&", routeValues.Select(kvp => $"{kvp.Key}={kvp.Value}")));
        }
        queryString = sb.ToString();
        return string.IsNullOrEmpty(queryString) ? string.Empty : $"?{queryString}";
    }
}
