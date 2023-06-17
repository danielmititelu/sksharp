using System.Text.Json;

namespace SkSharp;

public class SkypeApi
{
    private SkypeService _skypeService;

    public SkypeApi()
    {
        _skypeService = new SkypeService();
    }

    public async Task<LoggedInSkypeApi> Login(string username, string password, string cacheFilePath)
    {
        if (File.Exists(cacheFilePath)) {
            var cacheFile = JsonSerializer.Deserialize<LoginTokens>(File.ReadAllText(cacheFilePath));
            if (cacheFile != null && cacheFile.TokenExpirationDate > DateTime.UtcNow) {
                return new LoggedInSkypeApi(cacheFile);
            }
        }

        var securityToken = await _skypeService.GetSecurityToken(username, password);
        var skypeToken = await _skypeService.GetSkypeToken(securityToken);
        var (registrationToken, location) = await _skypeService.GetRegistrationToken(skypeToken.Skypetoken);
        var userId = await _skypeService.GetUsername(skypeToken.Skypetoken);

        var loginTokens = new LoginTokens {
            UserId = userId,
            SkypeToken = skypeToken.Skypetoken,
            RegistrationToken = registrationToken,
            Location = location,
            TokenExpirationDate = DateTime.UtcNow.AddSeconds(skypeToken.ExpiresIn)
        };
        
        File.WriteAllText(cacheFilePath, JsonSerializer.Serialize(loginTokens));

        return new LoggedInSkypeApi(loginTokens);
    }
}
