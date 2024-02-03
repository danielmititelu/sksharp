using SkSharp.PublicHosted;
using SkSharp.Services;
using SkSharp.Utils;

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
            var displayName = await _skSharpChat.GetDisplayName();
            var chatName = "Builds";

            await _skSharpChat.SendRichTextMessageAsync(chatName, $"{SkypeMessageHelpers.Emote("finger")} {SkypeMessageHelpers.Italic("Reporting for duty!")} {SkypeMessageHelpers.Emote("checkmark")}{SkypeMessageHelpers.Emote("checkmark")}{SkypeMessageHelpers.Emote("checkmark")}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _skSharpChat.PoolMessagesAsync();
                foreach (var message in messages)
                {
                    if (message.MessageType.Equals("RichText/Media_GenericFile"))
                    {
                        var localFileDetails = await message.DownloadAttachementAsync(@"Downloads\", new ProgressReporter(chatName, message.GetAttachedFileName(), _skSharpChat));
                        if (!localFileDetails.HasError)
                        {
                            await _skSharpChat.SendMessageAsync(chatName, $"Successfully downloaded file {localFileDetails.Name}, size {localFileDetails.Size / 1024 / 1024} MB, location {localFileDetails.LocalPath}");
                        }
                        else
                        {
                            await _skSharpChat.SendMessageAsync(chatName, $"Failed to downloaded file {localFileDetails.Name}, size {localFileDetails.Size / 1024 / 1024} MB");
                        }
                    }
                    else if ((message.Sender != myUserId && message.Sender != displayName)
                             && !message.MessageType.Contains("Typing"))
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