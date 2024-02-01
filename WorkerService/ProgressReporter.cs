using SkSharp.Models.Public;
using SkSharp.PublicHosted;

namespace WorkerService
{
    internal class ProgressReporter : IProgress<float>
    {
        private SkSharpChat _skSharpChat;
        private string _chatName;

        private readonly object _lock = new object();
        private int _lastWritenValue = -1;

        internal ProgressReporter(string chatName, SkSharpChat skSharpChat)
        {
            _skSharpChat = skSharpChat;
            _chatName = chatName;
        }

        public void Report(float percent)
        {
            var intVal = (int)(percent * 100);
            lock (_lock)
            {
                if (intVal % 10 == 0 && _lastWritenValue != intVal)
                {
                    _lastWritenValue = intVal;
                    _ = _skSharpChat.SendMessageAsync(_chatName, $"Progress: {intVal}");
                }
            }
        }
    }
}
