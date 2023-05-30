using Newtonsoft.Json;
using Skypeulica.Utils.Models;

namespace Skypeulica
{
    public class Skype
    {
        private const string CACHE_FILE_NAME = ".dat";

        private bool _usingCachedData = false;
        private string _secToken;
        private SkypeToken _skypeToken;
        private UserDetails _userDetails;
        private SkypeRegistrationDetails _registrationToken;

        private Chats _chats;

        public void Login(string username, string password)
        {
            if (File.Exists(CACHE_FILE_NAME))
            {
                var content = File.ReadAllText(CACHE_FILE_NAME);
                var cachedData = JsonConvert.DeserializeObject<CachedData>(content);
                if (cachedData.ExpiryDate > DateTime.Now)
                {
                    _userDetails = new() { username = cachedData.UserName };
                    _registrationToken = new() { RegistrationToken = cachedData.RegistrationToken, RawLocation = cachedData.RawLocation, Location = cachedData.Location };
                    _skypeToken = new() { expiresIn = (int)(cachedData.ExpiryDate - DateTime.Now).TotalSeconds };
                    _usingCachedData = true;
                }
                else
                {
                    ConnectToSkype(username, password);
                }
            }
            else
            {
                ConnectToSkype(username, password);
            }
        }

        public Chat GetRoomByName(string roomName)
        {
            LoadChats();
            var conv = _chats.conversations.FirstOrDefault(elem => elem.threadProperties?.topic.Equals(roomName, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (conv is null)
            {
                throw new Exception("Room not found");
            }

            return new Chat(_registrationToken.RawLocation, conv, _userDetails.username, _registrationToken.RegistrationToken);
        }

        public string GetUsername()
        {
            return _userDetails.username;
        }

        private void LoadChats()
        {
            _chats = SkypeApi.GetAllChats(_registrationToken.RegistrationToken);
        }

        private void ConnectToSkype(string username, string password)
        {
            _secToken = SkypeApi.GetSecToken(username, password);
            _skypeToken = SkypeApi.GetSkypeToken(_secToken);

            _userDetails = SkypeApi.GetUserDetails(_skypeToken.Token);
            _registrationToken = SkypeApi.GetRegistrationToken(_skypeToken.Token);
            CacheData();
        }

        private void CacheData()
        {
            File.WriteAllText(CACHE_FILE_NAME, JsonConvert.SerializeObject(new CachedData()
            {
                ExpiryDate = _skypeToken.ExpireDate,
                RawLocation = _registrationToken.RawLocation,
                Location = _registrationToken.Location,
                RegistrationToken = _registrationToken.RegistrationToken,
                UserName = _userDetails.username
            }));
        }

    }
}
