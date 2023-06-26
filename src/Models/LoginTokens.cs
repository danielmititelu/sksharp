namespace SkSharp;

internal class LoginTokens
{
    public string SkypeToken { get; set; } = string.Empty;
    public string RegistrationToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime TokenExpirationDate { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string EndpointId { get; set; } = string.Empty;
}
