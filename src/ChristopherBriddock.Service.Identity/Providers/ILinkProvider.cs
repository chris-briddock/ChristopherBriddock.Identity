namespace ChristopherBriddock.Service.Identity.Providers;

public interface ILinkProvider
{
    public string GetUri(HttpContext context, string endpointName, RouteValueDictionary routeValues);
}