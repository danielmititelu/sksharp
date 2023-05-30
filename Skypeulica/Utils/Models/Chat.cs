﻿namespace Skypeulica.Utils.Models
{
    public class Chat
    {
        private long _lastRoomMessageTreated;
        private string _location;
        private string _userName;
        private string _registrationToken;

        private Conversation _conversation;

        public Chat(string location, Conversation conversation, string userName, string registrationToken)
        {
            _location = location;
            _conversation = conversation;
            _userName = userName;
            _registrationToken = registrationToken;

            var destinationTimezoneId = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            _lastRoomMessageTreated = TimeUtils.GetUnixTimestamp(DateTime.UtcNow);
        }

        public void SendMessage(string content)
        {
            SkypeApi.SendMessage(_location, _conversation.id, _userName, _registrationToken, content);
        }

        public List<ReceivedMessageData> GetRecentMessages()
        {
            var receivedMessages = SkypeApi.GetRecentMessages(_location, _conversation.id, _registrationToken, _lastRoomMessageTreated);
            if (receivedMessages.messages.Count > 0)
            {
                _lastRoomMessageTreated = TimeUtils.GetUnixTimestamp(DateTime.UtcNow);
                // _lastRoomMessageTreated = receivedMessages.messages.Max(elem => elem.composetime);
            }
            return receivedMessages.messages;
        }

        public void GetEvents()
        {
            SkypeApi.GetEvents(_location, _conversation.id, _registrationToken);
        }
        
    }
}
