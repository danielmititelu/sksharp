using SkSharp.Services;
using System;

namespace SkSharp.Models.Public
{
    public class SentSkypeMessage
    {
        private readonly SkypePipe _skype;
        private readonly SkypeService _skypeService;

        public required string ChatName { get; set; }
        public required string ChatID { get; set; }
        public required string Message { get; set; }
        public required string Location { get; set; }
        public required string OriginalArriveTime { get; set; }

        public SentSkypeMessage(SkypePipe skypePipe,
                            SkypeService skypeService)
        {
            _skype = skypePipe;
            _skypeService = skypeService;
        }

        public async Task<SentSkypeMessage> EditMessageAsync(string message)
        {
            var tokens = await _skype.GetTokensAsync();
            var response = await _skypeService.EditMessageAsync(
                Location,
                tokens.RegistrationToken,
                tokens.DisplayName,
                message,
                OriginalArriveTime
            );

            Message = message;

            return this;
        }
    }
}
