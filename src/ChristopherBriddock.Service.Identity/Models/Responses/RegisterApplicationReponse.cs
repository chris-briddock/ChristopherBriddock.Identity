namespace ChristopherBriddock.Service.Identity.Models.Responses;

public sealed class RegisterApplicationResponse
{
    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!; 
}