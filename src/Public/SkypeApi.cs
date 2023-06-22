namespace SkSharp;

public class SkypeApi
{
    private LoginTokens _loginTokens;
    private readonly SkypeService _skypeService;
    private readonly FileCacheService _fileCacheService;
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

    internal async Task<LoginTokens> GetTokens(bool ignoreCache = false)
    {
        if (_isCacheFileRead == false && ignoreCache == false)
        {
            _loginTokens = await _fileCacheService.ReadCacheFile(_cacheFilePath) ?? new LoginTokens();
            _isCacheFileRead = true;

            if (_loginTokens.TokenExpirationDate != default && _loginTokens.TokenExpirationDate > DateTime.UtcNow)
            {
                return _loginTokens;
            }
        }

        var securityToken = await _skypeService.GetSecurityToken(_username, _password);
        var (skypeToken, expiresIn) = await _skypeService.GetSkypeToken(securityToken);
        var (registrationToken, baseUrl, endpointId) = await _skypeService.GetRegistrationToken(skypeToken);
        var userId = await _skypeService.GetUsername(skypeToken);

        var loginTokens = new LoginTokens
        {
            UserId = userId,
            SkypeToken = skypeToken,
            RegistrationToken = registrationToken,
            BaseUrl = baseUrl,
            TokenExpirationDate = DateTime.UtcNow.AddSeconds(expiresIn),
            EndpointId = endpointId
        };

        await _fileCacheService.WriteCacheFile(_cacheFilePath, loginTokens);
        return loginTokens;
    }

    internal async Task<LoginTokens> ReRegister()
    {
        var tokens = await GetTokens();
        var (registrationToken, baseUrl, endpointId) = await _skypeService.GetRegistrationToken(tokens.SkypeToken);
        tokens.RegistrationToken = registrationToken;
        tokens.BaseUrl = baseUrl;
        tokens.EndpointId = endpointId;
        await _fileCacheService.WriteCacheFile(_cacheFilePath, tokens);
        return tokens;
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
