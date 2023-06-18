namespace SkSharp;

public class LoggedInSkypeApi
{
    private SkypeService _skypeService;
    internal SkypeApi _skypeApi;

    internal LoggedInSkypeApi(SkypeApi skypeApi)
    {
        _skypeService = new SkypeService();
        _skypeApi = skypeApi;
    }

    public async Task<SkypeChat> GetRoomByName(string roomName)
    {
        var tokens = await _skypeApi.GetTokens();
        var chats = await _skypeService.GetChats(tokens.RegistrationToken, tokens.Location);
        var chat = chats.Conversations.FirstOrDefault(r => r.ThreadProperties.Topic.Equals(roomName, StringComparison.InvariantCultureIgnoreCase));
        if (chat is null)
        {
            throw new Exception("Room not found");
        }

        return new SkypeChat(chat.Id, this, _skypeService);
    }
}