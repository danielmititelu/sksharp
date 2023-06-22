namespace SkSharp;

public class SkypeChat
{
    private readonly SkypeService _skypeService;
    private readonly LoggedInSkypeApi _loggedInSkypeApi;
    private readonly string _chatId;
    private readonly SkypeApi _skypeApi;
    private bool _isSubscribed = false;

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
        if (_isSubscribed == false)
        {
            var subscribeResponse = await _skypeService.Subscribe(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                tokens.EndpointId
            );
            if (subscribeResponse.StatusCode != System.Net.HttpStatusCode.Created)
            {
                tokens = await _skypeApi.ReRegister();
                subscribeResponse = await _skypeService.Subscribe(
                    tokens.BaseUrl,
                    tokens.RegistrationToken,
                    tokens.EndpointId
                );

                if (subscribeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to subscribe to chat");
                }
            }
            _isSubscribed = true;
        }

        Console.WriteLine("start pooling...");
        var messageResponse = await _skypeService.GetMessageEvents(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            tokens.EndpointId
        );

        if (messageResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception("Failed to get messages");
        }

        foreach (var eventMessage in messageResponse.Content.EventMessages)
        {
            Console.WriteLine("inside library"+eventMessage.Resource.Content);
            OnMessage?.Invoke(new SkypeMessage
            {
                MessageType = eventMessage.Resource.Messagetype,
                Message = eventMessage.Resource.Content,
                Sender = eventMessage.Resource.Imdisplayname
            });
        }
    }
}