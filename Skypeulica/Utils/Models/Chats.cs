namespace Skypeulica.Utils.Models
{
    public class Chats
    {
        public List<Conversation> conversations { get; set; }
        public ChatsMetadata _metadata { get; set; }
    }

    public class Conversation
    {
        public string targetLink { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public object version { get; set; }
        public ChatProperties properties { get; set; }
        public LastMessage lastMessage { get; set; }
        public string messages { get; set; }
        public ThreadProperties threadProperties { get; set; }
    }

    public class LastMessage
    {
        public string id { get; set; }
        public DateTime originalarrivaltime { get; set; }
        public string messagetype { get; set; }
        public string version { get; set; }
        public DateTime composetime { get; set; }
        public string clientmessageid { get; set; }
        public string conversationLink { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public string conversationid { get; set; }
        public string from { get; set; }
        public string origincontextid { get; set; }
    }

    public class ChatsMetadata
    {
        public int totalCount { get; set; }
        public string forwardLink { get; set; }
        public string syncState { get; set; }
    }

    public class ChatProperties
    {
        public string conversationstatusproperties { get; set; }
        public string isemptyconversation { get; set; }
        public string onetoonethreadid { get; set; }
        public string consumptionhorizon { get; set; }
        public string conversationisblocked { get; set; }
        public string conversationstatus { get; set; }
        public string consumptionhorizonpublished { get; set; }
        public DateTime? lastimreceivedtime { get; set; }
        public string isfollowed { get; set; }
        public string color { get; set; }
        public string archived { get; set; }
    }

    public class ThreadProperties
    {
        public string lastjoinat { get; set; }
        public string topic { get; set; }
        public string membercount { get; set; }
        public string members { get; set; }
        public string version { get; set; }
        public string joiningenabled { get; set; }
    }


}
