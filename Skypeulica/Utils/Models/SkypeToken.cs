namespace Skypeulica.Utils.Models
{
    public class SkypeToken
    {
        public DateTime ExpireDate => DateTime.Now.AddSeconds(expiresIn);
        public string Token => skypetoken;

        public string skypetoken { get; set; }
        public int expiresIn { get; set; }
        public string skypeid { get; set; }
        public string signinname { get; set; }
        public string anid { get; set; }
        public bool isBusinessTenant { get; set; }
        public Status status { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string text { get; set; }
    }
}
