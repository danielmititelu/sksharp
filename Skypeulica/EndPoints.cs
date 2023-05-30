namespace Skypeulica
{
    public static class EndPoints
    {
        public const string API_MSACC = "https://login.live.com";
        public const string API_MSACC_LOGIN = "/RST.srf";


        public const string API_EDGE = "https://edge.skype.com/rps/v1/rps/skypetoken";


        public const string API_CONTACTS = "https://contacts.skype.com/contacts/v2";
        public const string API_CONTACTS_USERS = "/users";

        public const string API_USER = "https://api.skype.com";
        public const string API_USER_GET_ID = "/users/self/profile";



        public const string API_MSGSHOST = "https://client-s.gateway.messenger.live.com/v1";
        public const string API_MSGSHOST_CONVERSATIONS = "/users/ME/conversations";
        public const string API_MSGSHOST_ENDPOINTS = "/users/ME/endpoints";
    }
}
