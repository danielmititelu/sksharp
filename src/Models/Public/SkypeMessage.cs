using SkSharp.Models.Public;
using SkSharp.Services;

namespace SkSharp;

public class SkypeMessage
{
    private readonly SkypePipe _skype;
    private readonly SkypeService _skypeService;

    public required string MessageType { get; set; }
    public required string Message { get; set; }
    public required string Sender { get; set; }
    public string ThreadTopic { get; set; }

    public SkypeMessage(SkypePipe skypePipe,
                        SkypeService skypeService)
    {
        _skype = skypePipe;
        _skypeService = skypeService;
    }

    public async Task<FileDetails> DownloadAttachementAsync(string saveDirectory, IProgress<float> progress = null)
    {
        var tokens = await _skype.GetTokensAsync();
        return await _skypeService.DownloadMessageAttachement(tokens.SkypeToken, this, saveDirectory, progress);
    }
}