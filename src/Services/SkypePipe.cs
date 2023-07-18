using Microsoft.Extensions.Logging;
using SkSharp.PublicHosted;

namespace SkSharp.Services
{
    public class SkypePipe
    {
        private readonly SkypeService _skypeService;
        private readonly FileCacheService _fileCacheService;
        private readonly ILogger<SkypePipe> _logger;
        private readonly SkSharpOptions _options;

        private LoginTokens _loginTokens;
        private bool _isCacheFileRead = false;

        public SkypePipe(SkypeService skypeService,
            FileCacheService fileCacheService,
            SkSharpOptions options,
            ILogger<SkypePipe> logger)
        {
            _skypeService = skypeService;
            _fileCacheService = fileCacheService;
            _options = options;
            _logger = logger;
            _loginTokens = new LoginTokens();
        }

        internal async Task<LoginTokens> GetTokensAsync()
        {
            if (_isCacheFileRead == false)
            {
                _loginTokens = await _fileCacheService.ReadCacheFile(_options.CacheFilePath);
                _isCacheFileRead = true;
            }

            if (_loginTokens.TokenExpirationDate != default && _loginTokens.TokenExpirationDate > DateTime.UtcNow)
            {
                _logger.LogInformation("Using cached tokens");
                return _loginTokens;
            }

            var securityToken = await _skypeService.GetSecurityToken(_options.Username, _options.Password);
            var (skypeToken, expiresIn) = await _skypeService.GetSkypeToken(securityToken);
            var (registrationToken, baseUrl, endpointId) = await _skypeService.GetRegistrationToken(skypeToken);
            var userId = await _skypeService.GetUsername(skypeToken);

            _loginTokens = new LoginTokens
            {
                UserId = userId,
                SkypeToken = skypeToken,
                RegistrationToken = registrationToken,
                BaseUrl = baseUrl,
                TokenExpirationDate = DateTime.UtcNow.AddSeconds(expiresIn),
                EndpointId = endpointId
            };

            await _fileCacheService.WriteCacheFile(_options.CacheFilePath, _loginTokens);
            _logger.LogInformation("Using new tokens");
            return _loginTokens;
        }

        internal async Task<LoginTokens> RegenerateRegistrationToken()
        {
            var tokens = await GetTokensAsync();
            var (registrationToken, baseUrl, endpointId) = await _skypeService.GetRegistrationToken(tokens.SkypeToken);
            tokens.RegistrationToken = registrationToken;
            tokens.BaseUrl = baseUrl;
            tokens.EndpointId = endpointId;
            await _fileCacheService.WriteCacheFile(_options.CacheFilePath, tokens);
            return tokens;
        }
    }
}
