namespace SkSharp;

public class SkypeApi
{
    private LoginTokens _loginTokens;
    private SkypeService _skypeService;
    private FileCacheService _fileCacheService;
    private string _username;
    private string _password;
    private string _cacheFilePath;
    private bool _isCacheFileRead = false;

    public SkypeApi()
    {
        _loginTokens = new LoginTokens();
        _skypeService = new SkypeService();
        _fileCacheService = new FileCacheService();
        _cacheFilePath = string.Empty;
        _password = string.Empty;
        _username = string.Empty;
    }

    internal async Task<LoginTokens> GetTokens()
    {
        if(_isCacheFileRead == false)
        {
            _loginTokens = await _fileCacheService.ReadCacheFile(_cacheFilePath) ?? new LoginTokens();
            _isCacheFileRead = true;
        }

        if (_loginTokens.TokenExpirationDate != default && _loginTokens.TokenExpirationDate > DateTime.UtcNow)
        {
            return _loginTokens;
        }

        var securityToken = await _skypeService.GetSecurityToken(_username, _password);
        var skypeToken = await _skypeService.GetSkypeToken(securityToken);
        var (registrationToken, location) = await _skypeService.GetRegistrationToken(skypeToken.Skypetoken);
        var userId = await _skypeService.GetUsername(skypeToken.Skypetoken);

        var loginTokens = new LoginTokens
        {
            UserId = userId,
            SkypeToken = skypeToken.Skypetoken,
            RegistrationToken = registrationToken,
            Location = location,
            TokenExpirationDate = DateTime.UtcNow.AddSeconds(skypeToken.ExpiresIn)
        };

        await _fileCacheService.WriteCacheFile(_cacheFilePath, loginTokens);
        return loginTokens;
    }

    public async Task<LoggedInSkypeApi> Login(string username, string password, string cacheFilePath)
    {
        _cacheFilePath = cacheFilePath;
        _username = username;
        _password = password;
        _loginTokens = await GetTokens();

        return new LoggedInSkypeApi(this);
    }
}
