using SkSharp.Models.SkypeApiModels;

namespace SkSharp.Services
{

    public static class SkypePlublicService
    {
        private const string API_CONFIG = "https://a.config.skype.com/config/v1";
        private static readonly RestService _restService;

        static SkypePlublicService()
        {
            _restService = new RestService();
        }

        public static async Task<SkypeEmotes> GetAllEmotes()
        {
            var pResponse = await _restService.Get<PersonalizeResponse>($"{API_CONFIG}/Skype/0_0.0.0.0/SkypePersonalization");
            return await _restService.Get<SkypeEmotes>(pResponse.pes_config);
        }
    }
}
