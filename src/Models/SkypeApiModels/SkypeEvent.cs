namespace SkSharp.Models.SkypeApiModels;


public class SkypeEvent
{
    public List<EventMessage> EventMessages { get; set; }
}

public class EventMessage
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Resourcetype { get; set; }
    public DateTime Time { get; set; }
    public string Resourcelink { get; set; }
    public Resource Resource { get; set; }
}

public class Resource
{
    public string Ackrequired { get; set; }
    public string Type { get; set; }
    public string From { get; set; }
    public string Clientmessageid { get; set; }
    public string Version { get; set; }
    public string Messagetype { get; set; }
    public string Counterpartymessageid { get; set; }
    public string Imdisplayname { get; set; }
    public string Content { get; set; }
    public DateTime Composetime { get; set; }
    public string Origincontextid { get; set; }
    public DateTime Originalarrivaltime { get; set; }
    public string Threadtopic { get; set; }
    public string Contenttype { get; set; }
    public string Mlsepoch { get; set; }
    public string Conversationlink { get; set; }
    public bool Isactive { get; set; }
    public string Id { get; set; }
}
