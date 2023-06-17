using System.Net;

internal class RestResponse {
    public string Content { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}