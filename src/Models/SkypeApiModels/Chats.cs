namespace SkSharp;

public class Chats
{
    public List<Conversation> Conversations { get; set; } = new();
    public ChatsMetadata Metadata { get; set; } = new();
}

public class Conversation
{
    public string TargetLink { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object Version { get; set; } = string.Empty;
    public ChatProperties Properties { get; set; } = new();
    public LastMessage LastMessage { get; set; } = new();
    public string Messages { get; set; } = string.Empty;
    public ThreadProperties ThreadProperties { get; set; } = new();
}

public class LastMessage
{
    public string Id { get; set; } = string.Empty;
    public DateTime OriginalArrivaltime { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime ComposeTime { get; set; }
    public string ClientMessageId { get; set; } = string.Empty;
    public string ConversationLink { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string OrigincontextId { get; set; } = string.Empty;
}

public class ChatsMetadata
{
    public int TotalCount { get; set; }
    public string ForwardLink { get; set; } = string.Empty;
    public string SyncState { get; set; } = string.Empty;
}

public class ChatProperties
{
    public string ConversationStatusProperties { get; set; } = string.Empty;
    public string IsEmptyConversation { get; set; } = string.Empty;
    public string OnetooneThreadId { get; set; } = string.Empty;
    public string ConsumptionHorizon { get; set; } = string.Empty;
    public string ConversationisBlocked { get; set; } = string.Empty;
    public string ConversationStatus { get; set; } = string.Empty;
    public string ConsumptionHorizonPublished { get; set; } = string.Empty;
    public DateTime? Lastimreceivedtime { get; set; }
    public string IsFollowed { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Archived { get; set; } = string.Empty;
}

public class ThreadProperties
{
    public string Lastjoinat { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Membercount { get; set; } = string.Empty;
    public string Members { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Joiningenabled { get; set; } = string.Empty;
}
