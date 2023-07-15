using System.Xml.Linq;
using System.Net;
using SkSharp.Utils;
using SkSharp.Models.SkypeApiModels;

namespace SkSharp;

public class SkypeService
{
    readonly RestService _restService;

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

    internal async Task<string> GetUsername(string skypeToken)
    {
        var headers = new Dictionary<string, string>{
                { "X-SkypeToken", skypeToken }
        };
        var userDetails = await _restService.Get<UserDetails>("https://api.skype.com/users/self/profile", headers);
        return userDetails.Username;
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

    internal async Task SendMessageAsync(string baseUrl, string registrationToken, string chatId, string message)
    {
        var body = new
        {
            Content = message,
            Messagetype = "Text",
            Contenttype = "text",
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