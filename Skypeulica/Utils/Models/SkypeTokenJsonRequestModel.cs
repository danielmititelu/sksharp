namespace Skypeulica.Utils.Models
{
    internal class SkypeTokenJsonRequestModel
    {
        public string partner { get; set; } = "999";
        public string access_token { get; set; } = "";
        public string scopes { get; set; } = "client";

        public SkypeTokenJsonRequestModel(string secToken)
        {
            access_token = secToken;
        }
    }
}
