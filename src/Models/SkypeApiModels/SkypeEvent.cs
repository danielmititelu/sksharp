namespace SkSharp.Models.SkypeApiModels;


public class SkypeEvent
{
    public List<EventMessage> EventMessages { get; set; } = new();
}

public class EventMessage
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Resourcetype { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Resourcelink { get; set; } = string.Empty;
    public Resource Resource { get; set; } = new();
}

public class Resource
{
    public string Ackrequired { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Clientmessageid { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Messagetype { get; set; } = string.Empty;
    public string Counterpartymessageid { get; set; } = string.Empty;
    public string Imdisplayname { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Composetime { get; set; }
    public string Origincontextid { get; set; } = string.Empty;
    public DateTime Originalarrivaltime { get; set; }
    public string Threadtopic { get; set; } = string.Empty;
    public string Contenttype { get; set; } = string.Empty;
    public string Mlsepoch { get; set; } = string.Empty;
    public string Conversationlink { get; set; } = string.Empty;
    public bool Isactive { get; set; }
    public string Id { get; set; } = string.Empty;
}
