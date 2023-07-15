using SkSharp.PublicHosted;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SkSharpChat _skSharpChat;

        public Worker(ILogger<Worker> logger, SkSharpChat skSharpChat)
        {
            _logger = logger;
            _skSharpChat = skSharpChat;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var myUserId = await _skSharpChat.GetUserIdAsync();
            var chatName = "Builds";
            await _skSharpChat.SendMessageAsync(chatName, "Reporting for duty!");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _skSharpChat.PoolMessagesAsync();
                foreach (var message in messages)
                {
                    if (message.Sender != myUserId && !message.MessageType.Contains("Typing"))
                    {
                        var messageToSend = $"You said {message.Message}";
                        await _skSharpChat.SendMessageAsync(chatName, messageToSend);
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}