namespace SkSharp;

public class SkypeChat
{
    private SkypeService _skypeService;
    private LoggedInSkypeApi _loggedInSkypeApi;
    private string _chatId;
    private SkypeApi _skypeApi;
    internal SkypeChat(string chatId, LoggedInSkypeApi loggedInSkypeApi, SkypeService skypeService)
    {
        _chatId = chatId;
        _loggedInSkypeApi = loggedInSkypeApi;
        _skypeService = skypeService;
        _skypeApi = _loggedInSkypeApi._skypeApi;
    }

    public async Task SendMessage(string message)
    {
        var tokens = await _skypeApi.GetTokens();
        await _skypeService.SendMessage(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            _chatId,
            message
        );
    }
}