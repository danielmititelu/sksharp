using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skypeulica.Utils;
using Skypeulica.Utils.Models;
using System;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Skypeulica
{
    public static class SkypeApi
    {

        public static string GetSecToken(string username, string password)
        {
            var envelope = TemplateBuilder.BuildLoginRequestTemplate(username, password);
            var response = RestUtils.PostJson(EndPoints.API_MSACC, EndPoints.API_MSACC_LOGIN, envelope);

            return ResponseParsers.GetSecTokenFromResponse(response);
        }

        public static SkypeToken GetSkypeToken(string secToken)
        {
            var data = new SkypeTokenJsonRequestModel(secToken);
            var response = RestUtils.PostData(EndPoints.API_EDGE, string.Empty, data);

            return ResponseParsers.GetSkypeTokenFromResponse(response);
        }

        public static UserDetails GetUserDetails(string skypeToken)
        {
            var response = RestUtils.GetData(EndPoints.API_USER, EndPoints.API_USER_GET_ID, new() { { "X-SkypeToken", skypeToken } });
            return JsonConvert.DeserializeObject<UserDetails>(response); ;
        }

        public static SkypeRegistrationDetails GetRegistrationToken(string skypeToken)
        {
            var secs = DateTime.Now.Ticks;
            var hash = Mac256Utils.GetMac256Hash(secs.ToString());
            var headerds = new Dictionary<string, string>{
                { "LockAndKey", $"appId=msmsgs@msnmsgr.com; time={secs}; lockAndKeyResponse={hash}" },
                { "Authentication", $"skypetoken={skypeToken}" },
                { "BehaviorOverride", "redirectAs404"}
            };
            var json = "{\"endpointFeatures\": \"Agent\"}";

            var response = RestUtils.PostDataResponse(EndPoints.API_MSGSHOST, EndPoints.API_MSGSHOST_ENDPOINTS, json, headerds);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var locationHeader = response.Headers.First(elem => elem.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase));
                if (locationHeader is null)
                {
                    throw new Exception("Location not found");
                }
                var location = locationHeader.Value.ToString().Replace(EndPoints.API_MSGSHOST_ENDPOINTS, "");

                var locationResponse = RestUtils.PostDataResponse(location, EndPoints.API_MSGSHOST_ENDPOINTS, json, headerds);

                var regToken = new SkypeRegistrationDetails();
                regToken.RawLocation = location;
                regToken.Location = locationResponse.Headers.First(elem => elem.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase)).Value.ToString();
                regToken.RegistrationToken = locationResponse.Headers.First(elem => elem.Name.Equals("Set-RegistrationToken", StringComparison.InvariantCultureIgnoreCase)).Value.ToString();
                return regToken;
            }
            else
            {
                throw new Exception("Could not determine how to get the registration token in this case");
            }
        }

        public static Chats GetAllChats(string token)
        {
            var parameters = new Dictionary<string, string>() { { "startime", "0" }, { "view", "supportsExtendedHistory|msnp24Equivalent" }, { "targetType", "Passport|Skype|Lync|Thread|Agent|ShortCircuit|PSTN|Flxt|NotificationStream|ModernBots|secureThreads|InviteFree" } };
            var response = RestUtils.GetData(EndPoints.API_MSGSHOST, EndPoints.API_MSGSHOST_CONVERSATIONS, new() { { "RegistrationToken", token } }, parameters);

            return JsonConvert.DeserializeObject<Chats>(response);
        }

        public static Chats SendMessage(string msgsHost, string threadId, string username, string registrationToken, string content)
        {
            var msg = new SendMessageData();
            msg.clientmessageid = (DateTime.Now.Ticks / 1000).ToString();
            msg.content = content;
            msg.imdisplayname = username;

            var client = new Client();

            var response = RestUtils.PostData(msgsHost, $"/users/ME/conversations/{threadId}/messages", msg, new() { { "RegistrationToken", registrationToken }, { "ClientInfo", client.ToString() } });

            return JsonConvert.DeserializeObject<Chats>(response);
        }

        public static RecentMessagesData GetRecentMessages(string msgsHost, string threadId, string registrationToken, long startTime)
        {
            var headers = new Dictionary<string, string>() {
                { "RegistrationToken", registrationToken },
                { "BehaviorOverride", "redirectAs404" },
                { "Sec-Fetch-Dest", "empty" },
                { "Sec-Fetch-Mode", "cors"},
                { "Sec-Fetch-Site", "cross-site"}
            };

            var parameters = new Dictionary<string, string>() {
                { "startTime", startTime.ToString() },
                { "view", "supportsExtendedHistory|msnp24Equivalent|supportsMessageProperties" },
                { "pageSize", "30" }
            };

            var response = RestUtils.GetData(msgsHost, $"/users/ME/conversations/{threadId}/messages", headers, parameters);
            return JsonConvert.DeserializeObject<RecentMessagesData>(response);
        }

        public static void GetEvents(string msgsHost, string chatId, string token) 
        {
            var response = RestUtils.PostDataResponse(msgsHost, $"/users/ME/endpoints/{chatId}/subscriptions/0/poll", null, new() { { "RegistrationToken", token } });
            Console.WriteLine(response);
        }

        //public static SkypeToken GetAllContacts(string myUserID, string token)
        //{
        //    //resp = self.skype.conn("GET", "{0}/users/{1}".format(SkypeConnection.API_CONTACTS, self.skype.userId),
        //    //                      params={ "delta": "", "reason": "default"},
        //    //                  auth = SkypeConnection.Auth.SkypeToken).json()

        //    //  headers["Authorization"] = "skype_token {0}".format(self.tokens["skype"])

        //    var data = new SkypeGetContactsJsonRequestModel();
        //    //var response = RestUtils.GetData(EndPoints.API_CONTACTS, $"{EndPoints.API_CONTACTS_USERS}/{myUserID}", data, new() { { "Authorization", $"skype_token {token}" } });

        //    return ResponseParsers.GetSkypeTokenFromResponse(response);
        //}

    }
}
