namespace Skypeulica.Utils.Models
{
    public class RecentMessagesData
    {

        public List<ReceivedMessageData> messages { get; set; }
        public ReceivedMessagesMetadata _metadata { get; set; }
    }

    public class ReceivedMessageData
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

    public class ReceivedMessagesMetadata
    {
        public string syncState { get; set; }
        public string backwardLink { get; set; }
        public long lastCompleteSegmentStartTime { get; set; }
        public long lastCompleteSegmentEndTime { get; set; }
    }


}
