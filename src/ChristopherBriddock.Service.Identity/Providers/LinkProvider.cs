using System.Text;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Gets the URL provided by the <see cref="HttpContext"/>
/// </summary>
public class LinkProvider  : ILinkProvider
{
    /// <inheritdoc/>
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
    private string BuildQueryParameters(RouteValueDictionary routeValues)
    {
        StringBuilder sb = new();
            
        var queryString = sb.Append(string.Join("&", routeValues.Select(kvp => $"{kvp.Key}={kvp.Value}"))).ToString();
        return string.IsNullOrEmpty(queryString) ? string.Empty : $"?{queryString}";
    }
}
