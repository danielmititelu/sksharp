namespace SkSharp;

public class SkypeChat
{
    private readonly SkypeService _skypeService;
    private readonly LoggedInSkypeApi _loggedInSkypeApi;
    private readonly string _chatId;
    private readonly SkypeApi _skypeApi;

    public delegate void MessageHandler(SkypeMessage message);
    public event MessageHandler OnMessage;

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

    public void StartPolling()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await PollMessages();
            }
        });
    }

    private async Task PollMessages()
    {
        var tokens = await _skypeApi.GetTokens();
        await _skypeService.Subscribe(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            tokens.EndpointId
        );
        var message = await _skypeService.GetMessageEvents(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            tokens.EndpointId
        );
        OnMessage?.Invoke(new SkypeMessage
        {
            Message = message,
            Sender = "Gildrobica"
        });
    }
}