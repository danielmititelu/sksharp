using SkSharp.Models.Public;
using SkSharp.Models.SkypeApiModels;
using SkSharp.Utils;
using System.Net;
using System.Xml.Linq;

namespace SkSharp;

public class SkypeService
{
    private readonly RestService _restService;

    public SkypeService()
    {
        _restService = new RestService();
    }

    internal async Task<string> GetSecurityToken(string username, string password)
    {
        var body = @$"<Envelope xmlns='http://schemas.xmlsoap.org/soap/envelope/'
            xmlns:wsse='http://schemas.xmlsoap.org/ws/2003/06/secext'
            xmlns:wsp='http://schemas.xmlsoap.org/ws/2002/12/policy'
            xmlns:wsa='http://schemas.xmlsoap.org/ws/2004/03/addressing'
            xmlns:wst='http://schemas.xmlsoap.org/ws/2004/04/trust'
            xmlns:ps='http://schemas.microsoft.com/Passport/SoapServices/PPCRL'>
            <Header>
                <wsse:Security>
                    <wsse:UsernameToken Id='user'>
                        <wsse:Username>{username}</wsse:Username>
                        <wsse:Password>{password}</wsse:Password>
                    </wsse:UsernameToken>
                </wsse:Security>
            </Header>
            <Body>
                <ps:RequestMultipleSecurityTokens Id='RSTS'>
                    <wst:RequestSecurityToken Id='RST0'>
                        <wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>
                        <wsp:AppliesTo>
                            <wsa:EndpointReference>
                                <wsa:Address>wl.skype.com</wsa:Address>
                            </wsa:EndpointReference>
                        </wsp:AppliesTo>
                        <wsse:PolicyReference URI='MBI_SSL'></wsse:PolicyReference>
                    </wst:RequestSecurityToken>
                </ps:RequestMultipleSecurityTokens>
            </Body>
            </Envelope>";

        var responseContent = await _restService.PostXml("https://login.live.com/RST.srf", body);
        var xmlResponse = XElement.Parse(responseContent);
        var securityToken = xmlResponse.Descendants().First(x => x.Name.LocalName == "BinarySecurityToken").Value;
        if (string.IsNullOrEmpty(securityToken))
        {
            throw new Exception("Could not get security token");
        }

        return securityToken;
    }

    internal async Task<(string, int)> GetSkypeToken(string securityToken)
    {
        var body = new
        {
            partner = "999",
            access_token = securityToken,
            scopes = "client"
        };

        var response = await _restService.PostJson<SkypeTokenResponse>("https://edge.skype.com/rps/v1/rps/skypetoken", body);
        return (response.Skypetoken, response.ExpiresIn);
    }

    internal async Task<(string registrationToken, string location, string endpointId)> GetRegistrationToken(string skypeToken)
    {
        var baseUrl = "https://client-s.gateway.messenger.live.com";
        var currentTicks = DateTime.Now.Ticks;
        var hash = Mac256Utils.GetMac256Hash(currentTicks.ToString());
        var headers = new Dictionary<string, string>{
                { "LockAndKey", $"appId=msmsgs@msnmsgr.com; time={currentTicks}; lockAndKeyResponse={hash}" },
                { "Authentication", $"skypetoken={skypeToken}" },
                { "BehaviorOverride", "redirectAs404"}
        };

        var body = new
        {
            endpointFeatures = "Agent"
        };

        var response = await _restService.PostJson($"{baseUrl}/v1/users/ME/endpoints", body, headers);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var location = response.Headers.FirstOrDefault(x => x.Key == "Location").Value?.ToString();
            if (string.IsNullOrEmpty(location))
            {
                throw new Exception("Could not get registration token");
            }
            response = await _restService.PostJson(location, body, headers);
            baseUrl = new Uri(location).GetLeftPart(UriPartial.Authority);
        }

        var registrationTokenHeader = response.Headers.FirstOrDefault(x => x.Key == "Set-RegistrationToken").Value?.ToString();
        if (string.IsNullOrEmpty(registrationTokenHeader))
        {
            throw new Exception("Could not get registration token");
        }
        var (registrationToken, expires, endpointId) = TokenUtils.ParseRegistrationTokenHeader(registrationTokenHeader);

        if (string.IsNullOrEmpty(registrationTokenHeader))
        {
            throw new Exception("Could not get registration token");
        }
        return (registrationToken, baseUrl, endpointId);
    }

    internal async Task<(string UserId, string DisplayName)> GetUsername(string skypeToken)
    {
        var headers = new Dictionary<string, string>{
                { "X-SkypeToken", skypeToken }
        };
        var userDetails = await _restService.Get<UserDetails>("https://api.skype.com/users/self/profile", headers);
        return (userDetails.Username, $"{userDetails.Firstname} {userDetails.Lastname}");
    }

    internal async Task<FileDetails> DownloadMessageAttachement(string skypeToken, SkypeMessage message, string savePath, IProgress<float> progress = null)
    {
        var headers = new Dictionary<string, string>{
            { "Authorization", $"skype_token {skypeToken}" },
            { "Accept-Encoding", "gzip, deflate, br" }
        };

        var uriFile = message.ToURIFile(URIObjectUtils.ContentType.File);

        var localPath = Path.Combine(savePath, uriFile.OriginalName);

        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        using (var file = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await _restService.DownloadAsync(uriFile.DownloadLink, headers, file, progress);
        }

        return new FileDetails
        {
            Name = uriFile.OriginalName,
            LocalPath = localPath,
            Size = uriFile.Size
        };
    }

    internal async Task<Chats> GetChats(string registrationToken, string baseUrl)
    {
        var queryParameters = new Dictionary<string, string>() {
            { "startime", "0" },
            { "view", "supportsExtendedHistory|msnp24Equivalent" },
            { "targetType", "Passport|Skype|Lync|Thread|Agent|ShortCircuit|PSTN|Flxt|NotificationStream|ModernBots|secureThreads|InviteFree" }
        };
        var headers = new Dictionary<string, string>{
                { "RegistrationToken", registrationToken }
        };

        return await _restService.Get<Chats>($"{baseUrl}/v1/users/ME/conversations", headers, queryParameters);
    }

    internal async Task<RestResponse> EditMessageAsync(string location, string registrationToken,
                                                       string displayName,
                                                       string message, string originalArriveTime, string messageType = "Text")
    {
        //var realContent = $"{message}<e_m a=\"live:.cid.6f340b045df93b21\" ts_ms=\"{originalArriveTime}\" ts=\"{originalArriveTime.Substring(0, originalArriveTime.Length - 4)}\" t=\"61\"></e_m>";

        var body = new
        {
            content = message,
            messagetype = messageType,
            contenttype = "text",
            editid = location.Split('/').Last(),
            imdisplayname = displayName
        };

        var headers = new Dictionary<string, string> {
                { "RegistrationToken", registrationToken },
                { "ClientInfo", "os=Windows; osVer=10; proc=x86; lcid=en-US; deviceType=1; country=US; clientName=skype4life; clientVer=1418/9.99.0.999//skype4life" }
        };
        var response = await _restService.PutJson(location, body, headers);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Could not send message, got {response.StatusCode}");
        }

        return response;
    }

    internal async Task<RestResponse> SendMessageAsync(string baseUrl, string registrationToken, string chatId,
                                                       string displayName,
                                                       string message, string messageType = "Text")
    {

        var body = new
        {
            clientmessageid = MessageUtils.NewClientMessageId(),
            //ComposeTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
            content = message,
            messagetype = messageType,
            contenttype = "text",
            imdisplayname = displayName,
        };

        var headers = new Dictionary<string, string> {
                { "RegistrationToken", registrationToken },
                { "ClientInfo", "os=Windows; osVer=10; proc=x86; lcid=en-US; deviceType=1; country=US; clientName=skype4life; clientVer=1418/9.99.0.999//skype4life" }
        };
        var response = await _restService.PostJson($"{baseUrl}/v1/users/ME/conversations/{chatId}/messages", body, headers);
        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception($"Could not send message, got {response.StatusCode}");
        }

        return response;
    }

    internal async Task<RestResponse> Subscribe(string baseUrl, string registrationToken, string endpointId)
    {
        var body = new
        {
            interestedResources = new string[] {
                "/v1/users/ME/conversations/ALL/messages",
            },
            channelType = "httpLongPoll",
            conversationType = 2047
        };

        var headers = new Dictionary<string, string>{
            { "RegistrationToken", registrationToken }
        };
        return await _restService.PostJson($"{baseUrl}/v1/users/ME/endpoints/{endpointId}/subscriptions", body, headers);
    }

    internal async Task<RestResponse<SkypeEvent>> GetMessageEvents(string baseUrl, string registrationToken, string endpointId)
    {
        var headers = new Dictionary<string, string>{
            { "RegistrationToken", registrationToken }
        };
        var endpoint = WebUtility.UrlEncode(endpointId);

        try
        {
            return await _restService.Post<SkypeEvent>($"{baseUrl}/v1/users/ME/endpoints/{endpoint}/subscriptions/0/poll", headers);
        }
        catch (TaskCanceledException)
        {
            return new RestResponse<SkypeEvent>
            {
                StatusCode = HttpStatusCode.RequestTimeout
            };
        }
    }
}