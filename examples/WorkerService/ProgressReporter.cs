using SkSharp.Models.Public;
using SkSharp.PublicHosted;

namespace WorkerService
{
    internal class ProgressReporter : IProgress<float>
    {
        private SkSharpChat _skSharpChat;
        private string _chatName;
        private string _fileName;

        private readonly object _lock = new object();
        private readonly SemaphoreSlim _collectionsLock = new(1, 1);
        private int _lastWritenValue = -1;

        private SentSkypeMessage _message;

        internal ProgressReporter(string chatName, string fileName, SkSharpChat skSharpChat)
        {
            _skSharpChat = skSharpChat;
            _chatName = chatName;
            _fileName = fileName;
        }

        public async void Report(float percent)
        {
            var intVal = (int)(percent * 100);

            var shouldSend = false;
            lock (_lock)
            {
                shouldSend = intVal % 10 == 0 && _lastWritenValue != intVal;
                _lastWritenValue = intVal;
            }

            if (shouldSend)
            {
                _collectionsLock.Wait();
                try
                {
                    if (_message == null)
                    {
                        _message = await _skSharpChat.SendMessageAsync(_chatName, $"Downloading {_fileName} progress: {intVal}");
                    }
                    else
                    {
                        await _message.EditMessageAsync($"Downloading {_fileName} progress: {intVal}");
                    }
                }
                finally
                {
                    _collectionsLock.Release();
                }
            }
        }
    }
}
