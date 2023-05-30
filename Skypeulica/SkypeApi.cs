using Skypeulica.Utils;
using Skypeulica.Utils.Models;

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

        public static SkypeToken GetAllContacts(string myUserID, string token)
        {
            //resp = self.skype.conn("GET", "{0}/users/{1}".format(SkypeConnection.API_CONTACTS, self.skype.userId),
            //                      params={ "delta": "", "reason": "default"},
            //                  auth = SkypeConnection.Auth.SkypeToken).json()

            //  headers["Authorization"] = "skype_token {0}".format(self.tokens["skype"])

            var data = new SkypeGetContactsJsonRequestModel();
            var response = RestUtils.PostData(EndPoints.API_CONTACTS, $"{EndPoints.API_CONTACTS_USERS}/{myUserID}", data, new() { { "Authorization", $"skype_token {token}" } });

            return ResponseParsers.GetSkypeTokenFromResponse(response);
        }

    }
}
