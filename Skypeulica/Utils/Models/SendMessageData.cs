using Newtonsoft.Json;

namespace Skypeulica.Utils.Models
{
    internal class SendMessageData
    {
        public string contenttype { get; set; } = "text";
        public string messagetype { get; set; } = "Text";
        public string content { get; set; }

        public string imdisplayname { get; set; }
        public string clientmessageid { get; set; }

        [JsonProperty("Has-Mentions")]
        public bool hasMentions { get; set; } = false;

    }
}
