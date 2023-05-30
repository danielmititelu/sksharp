namespace Skypeulica.Utils.Models
{
    internal class SkypeGetAllConversationsJsonRequestModel
    {
        public string startTime { get; set; } = "0";
        public string view { get; set; } = "supportsExtendedHistory|msnp24Equivalent";
        public string targetType { get; set; } = "Passport|Skype|Lync|Thread|Agent|ShortCircuit|PSTN|Flxt|NotificationStream|ModernBots|secureThreads|InviteFree";
    }
}
