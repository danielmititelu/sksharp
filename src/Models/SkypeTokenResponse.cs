namespace SkSharp;

public class SkypeTokenResponse
{
    public string Skypetoken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string Skypeid { get; set; } = string.Empty;
    public string Signinname { get; set; } = string.Empty;
    public string Anid { get; set; } = string.Empty;
    public bool IsBusinessTenant { get; set; }
    public Status Status { get; set; } = new();
}

public class Status
{
    public int Code { get; set; }
    public string Text { get; set; } = string.Empty;
}
