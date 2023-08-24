namespace SkSharp;

public class SkypeMessage
{
    public required string MessageType { get; set; }
    public required string Message { get; set; }
    public required string Sender { get; set; }
    public string ThreadTopic { get; set; }
}