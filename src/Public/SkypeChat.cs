using System.Net;

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
        var watch = new System.Diagnostics.Stopwatch();
        _ = Task.Run(async () =>
        {
            while (true)
            {
                watch.Restart();
                Console.WriteLine("Polling messages");
                await PollMessages();
                watch.Stop();
                Console.WriteLine($"Polling messages done, took {watch.ElapsedMilliseconds}ms");
            }
        }).ContinueWith((t) =>
        {
            if (t.IsFaulted)
            {
                if (t.Exception != null)
                {
                    Console.WriteLine("Polling stopped with error: {0}", t.Exception);
                }
                else
                {
                    Console.WriteLine("Polling stopped with error");
                }
            }
            Console.WriteLine("Polling stopped");
        });
    }

    private async Task PollMessages()
    {
        var tokens = await _skypeApi.GetTokens();
        if (_isSubscribed == false)
        {
            await SubscribeToChat(tokens);
            _isSubscribed = true;
        }

        var messageResponse = await _skypeService.GetMessageEvents(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            tokens.EndpointId
        );

        if (messageResponse.StatusCode == HttpStatusCode.NotFound)
        {
            await SubscribeToChat(tokens, true);
            messageResponse = await _skypeService.GetMessageEvents(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                tokens.EndpointId
            );
        }

        if (messageResponse.StatusCode != HttpStatusCode.RequestTimeout && messageResponse.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("Failed to get messages");
            throw new Exception("Failed to get messages");
        }

        if (messageResponse.StatusCode != HttpStatusCode.RequestTimeout)
        {
            foreach (var eventMessage in messageResponse.Content.EventMessages)
            {
                OnMessage?.Invoke(new SkypeMessage
                {
                    MessageType = eventMessage.Resource.Messagetype,
                    Message = eventMessage.Resource.Content,
                    Sender = eventMessage.Resource.Imdisplayname
                });
            }
        }
    }

    private async Task SubscribeToChat(LoginTokens tokens, bool regenerateRegistrationToken = false)
    {
        if (regenerateRegistrationToken)
        {
            tokens = await _skypeApi.RegenerateRegistrationToken();
        }

        var subscribeResponse = await _skypeService.Subscribe(
            tokens.BaseUrl,
            tokens.RegistrationToken,
            tokens.EndpointId
        );

        if (subscribeResponse.StatusCode == HttpStatusCode.NotFound)
        {
            await _skypeApi.RegenerateRegistrationToken();
            subscribeResponse = await _skypeService.Subscribe(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                tokens.EndpointId
            );
        }

        if (subscribeResponse.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Failed to subscribe to chat");
        }
    }
}