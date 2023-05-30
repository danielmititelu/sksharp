namespace Skypeulica.Utils.Models
{
    internal class Client
    {
        public string os { get; set; } = "Windows";
        public string osVer { get; set; } = "10";
        public string proc { get; set; } = "x86";
        public string lcid { get; set; } = "en-US";
        public string deviceType { get; set; } = "1";
        public string country { get; set; } = "US";
        public string clientName { get; set; } = "skype4life";
        public string clientVer { get; set; } = "1418/9.99.0.999//skype4life";

        public override string ToString()
        {
            return $"os={os}; osVer={osVer}; proc={proc}; lcid={lcid}; deviceType={deviceType}; country={country}; clientName={clientName}; clientVer={clientVer}";
        }
    }
}
