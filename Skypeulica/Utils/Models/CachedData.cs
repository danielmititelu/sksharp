namespace Skypeulica.Utils.Models
{
    public class CachedData
    {
        //Register token and expiry date
        public string RegistrationToken { get; set; }
        public DateTime ExpiryDate { get; set; }

        //Cloud location
        public string RawLocation { get; set; }
        public string Location { get; set; }

        //User details
        public string UserName { get; set; }
    }
}
