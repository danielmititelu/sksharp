﻿using Microsoft.Extensions.Logging;
using SkSharp.Services;
using System.Net;

namespace SkSharp.PublicHosted
{
    public class SkSharpChat
    {
        private readonly ILogger<SkSharpChat> _logger;
        private readonly SkypePipe _skype;
        private readonly SkypeService _skypeService;
        private Dictionary<string, string> _chatNameChatIdDict;
        private bool _isSubscribed;

        public SkSharpChat(ILogger<SkSharpChat> logger,
            SkypePipe skypePipe,
            SkypeService skypeService)
        {
            _logger = logger;
            _skype = skypePipe;
            _skypeService = skypeService;
            _chatNameChatIdDict = new Dictionary<string, string>();
            _isSubscribed = false;
        }

        public async Task<string> GetUserIdAsync()
        {
            var tokens = await _skype.GetTokensAsync();
            return tokens.UserId;
        }

        public async Task SendMessageAsync(string chatName, string message)
        {
            var tokens = await _skype.GetTokensAsync();
            var chatId = await GetChatRoomByNameAsync(tokens, chatName);
            await _skypeService.SendMessageAsync(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                chatId,
                message
            );
            _logger.LogInformation("Sent message:\"{message}\" to chat: {chatName}", message, chatName);
        }

        public async Task<List<SkypeMessage>> PoolMessagesAsync()
        {
            var tokens = await _skype.GetTokensAsync();
            if (_isSubscribed == false)
            {
                await SubscribeToChatAsync(tokens);
                _isSubscribed = true;
            }

            var messageResponse = await _skypeService.GetMessageEvents(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                tokens.EndpointId
            );

            if (messageResponse.StatusCode == HttpStatusCode.NotFound)
            {
                await SubscribeToChatAsync(tokens, true);
                messageResponse = await _skypeService.GetMessageEvents(
                    tokens.BaseUrl,
                    tokens.RegistrationToken,
                    tokens.EndpointId
                );
            }

            if (messageResponse.StatusCode != HttpStatusCode.RequestTimeout && messageResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("Failed to get messages");
                throw new Exception("Failed to get messages");
            }

            if (messageResponse.StatusCode == HttpStatusCode.RequestTimeout)
            {
                throw new Exception("Could not get messages, request timeout");
            }

            var skypeMessages = messageResponse.Content.EventMessages.Select(eventMessage => new SkypeMessage
            {
                MessageType = eventMessage.Resource.Messagetype,
                Message = eventMessage.Resource.Content,
                Sender = eventMessage.Resource.Imdisplayname
            });

            return skypeMessages.ToList();
        }

        private async Task<string> GetChatRoomByNameAsync(LoginTokens tokens, string chatName)
        {
            if (_chatNameChatIdDict.TryGetValue(chatName, out var chatId))
            {
                return chatId;
            }

            var chats = await _skypeService.GetChats(tokens.RegistrationToken, tokens.BaseUrl);
            var chat = chats.Conversations.FirstOrDefault(r => r.ThreadProperties.Topic.Equals(chatName, StringComparison.InvariantCultureIgnoreCase));
            if (chat is null)
            {
                throw new Exception("Room not found");
            }

            return chat.Id;
        }

        private async Task SubscribeToChatAsync(LoginTokens tokens, bool regenerateRegistrationToken = false)
        {
            if (regenerateRegistrationToken)
            {
                tokens = await _skype.RegenerateRegistrationToken();
            }

            var subscribeResponse = await _skypeService.Subscribe(
                tokens.BaseUrl,
                tokens.RegistrationToken,
                tokens.EndpointId
            );

            if (subscribeResponse.StatusCode == HttpStatusCode.NotFound)
            {
                await _skype.RegenerateRegistrationToken();
                subscribeResponse = await _skypeService.Subscribe(
                    tokens.BaseUrl,
                    tokens.RegistrationToken,
                    tokens.EndpointId
                );
            }

            if (subscribeResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception("Failed to subscribe to chat");
            }
        }
    }
}
