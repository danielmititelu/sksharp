using Newtonsoft.Json;
using Skypeulica.Utils.Models;
using System;

namespace Skypeulica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configContent = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(configContent);

            var skype = new Skype();
            skype.Login(config.User, config.Password);

            var room = skype.GetRoomByName(config.BuildGroupChatName);
            // room.GetEvents();
            room.SendMessage("Ping pong protocol has started");
            var botUsername = skype.GetUsername();
            Task.Run(() =>
            {
                do
                {
                    var messages = room.GetRecentMessages();
                    foreach (var message in messages)
                    {
                        if (!message.from.Contains(botUsername) && message.content.Contains("ping"))
                        {
                            room.SendMessage($"pong to : {message.content}");
                        }
                    }

                    Thread.Sleep(5000);
                } while (true);
            });


            Console.ReadLine();

            return;

            // login
            //var secToken = SkypeApi.GetSecToken(config.User, config.Password);
            //var token = SkypeApi.GetSkypeToken(secToken);

            var userID = "live:.cid.6f340b045df93b21";
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVFODQ4MjE0Qzc3MDczQUU1QzJCREU1Q0NENTQ0ODlEREYyQzRDODQiLCJ4NXQiOiJYb1NDRk1kd2M2NWNLOTVjelZSSW5kOHNUSVEiLCJ0eXAiOiJKV1QifQ.eyJpYXQiOjE2ODU0NDMyNjAsImV4cCI6MTY4NTUyOTY1OSwic2t5cGVpZCI6ImxpdmU6LmNpZC42ZjM0MGIwNDVkZjkzYjIxIiwic2NwIjo5NTYsImNzaSI6IjE2ODU0NDMyNTkiLCJjaWQiOiI2ZjM0MGIwNDVkZjkzYjIxIiwiYWF0IjoxNjg1NDQzMjU5fQ.TTmxuDZUPuDBqbZX0ohRkBPjA8q90z_f5Rdm1E1ij1VlGNzD7mNPKUxIwBLcepQKoTpkUKjhasfs83uEKbWfkGuK_qzY-pyKiDNO3mWuVXlbwxS-0mzllhaaChe_Wdnms8UNgpXggIS4qtQrpKFqGx9LMthYdXItBdqYsykdvK5G0Gz6pcsND75VhcSvkB5SGsqoMab2pAaEkJ8IXDwws8ffyZHvCYL48lT9W8QmmTJJtoJtjGLpe95F_OTFHi5H6m916Hq9JJSlNtgGdbv5k2zcAhi0dwiLZU2vnR3z8OkaQz85VNsMghgoPvbQY71BI_529qxGB5b30NA-x07zUg";

            // User id and registration token

            //var userDetails = SkypeApi.GetUserDetails(token);
            //var registrationToken = SkypeApi.GetRegistrationToken(token.Token);
            //var rawLocation = registrationToken.RawLocation;

            var username = "live:.cid.6f340b045df93b21";
            var rawLocation = "https://azeus1-client-s.gateway.messenger.live.com/v1/";
            var location = "https://azeus1-client-s.gateway.messenger.live.com/v1/users/ME/endpoints/%7B43531b21-c225-4ecb-8474-eed356332c0c%7D";
            var registrationToken = "registrationToken=U2lnbmF0dXJlOjI6Mjg6QVFRQUFBQlNyVTZMYTZyQmlOcjFibHRBc1N3NDtWZXJzaW9uOjY6MToxO0lzc3VlVGltZTo0OjE5OjUyNDk4OTY0NDcyMjA3MTM0MzE7RXAuSWRUeXBlOjc6MTo4O0VwLklkOjI6MjY6bGl2ZTouY2lkLjZmMzQwYjA0NWRmOTNiMjE7RXAuRXBpZDo1OjM2OjQzNTMxYjIxLWMyMjUtNGVjYi04NDc0LWVlZDM1NjMzMmMwYztFcC5Mb2dpblRpbWU6NzoxOjA7RXAuQXV0aFRpbWU6NDoxOTo1MjQ5ODk2NDQ3MjIwNTU3MjQ5O0VwLkF1dGhUeXBlOjc6MjoxNTtFcC5FeHBUaW1lOjQ6MTk6NTI0OTg5NzI4MzAxNzM4NzkwNDtFcC5Ta3lwZVRva2VuOjI6Njg4OmV5SmhiR2NpT2lKU1V6STFOaUlzSW10cFpDSTZJalZGT0RRNE1qRTBRemMzTURjelFVVTFRekpDUkVVMVEwTkVOVFEwT0RsRVJFWXlRelJET0RRaUxDSjROWFFpT2lKWWIxTkRSazFrZDJNMk5XTkxPVFZqZWxaU1NXNWtPSE5VU1ZFaUxDSjBlWEFpT2lKS1YxUWlmUS5leUpwWVhRaU9qRTJPRFUwTkRNeU5qQXNJbVY0Y0NJNk1UWTROVFV5T1RZMU9Td2ljMnQ1Y0dWcFpDSTZJbXhwZG1VNkxtTnBaQzQyWmpNME1HSXdORFZrWmprellqSXhJaXdpYzJOd0lqbzVOVFlzSW1OemFTSTZJakUyT0RVME5ETXlOVGtpTENKamFXUWlPaUkyWmpNME1HSXdORFZrWmprellqSXhJaXdpWVdGMElqb3hOamcxTkRRek1qVTVmUS5UVG14dURaVVB1REJxYlpYMG9oUmtCUGpBOHE5MHpfZjVSZG0xRTFpajFWbEdOekQ3bU5QS1V4SXdCTGNlcFFLb1Rwa1VLamhhc2ZzODN1RUtiV2ZrR3VLX3F6WS1weUtpRE5PM21XdVZYbGJ3eFMtMG16bGxoYWFDaGVfV2RubXM4VU5ncFhnZ0lTNHF0UXJwS0ZxR3g5TE10aFlkWEl0QmRxWXN5a2R2SzVHMEd6NnBjc05ENzVWaGNTdmtCNVNHc3FvTWFiMnBBYUVrSjhJWER3d3M4ZmZ5Wkh2Q1lMNDhsVDlXOFFtbVRKSnRvSnRqR0xwZTk1Rl9PVEZIaTVINm05MTZIcTlKSlNsTnRnR2RidjVrMnpjQWhpMGR3aUxaVTJ2blIzejhPa2FRejg1Vk5zTWdoZ29QdmJRWTcxQklfNTI5cXhHQjViMzBOQS14MDd6VWc7VXNyLk5ldE1hc2s6MTE6MToyO1Vzci5YZnJDbnQ6NjoxOjA7VXNyLlJkcmN0RmxnOjI6MDo7VXNyLkV4cElkOjk6MTowO1Vzci5FeHBJZExhc3RMb2c6NDoxOjA7VXNlci5BdGhDdHh0OjI6NDQ0OkNsTnJlWEJsVkc5clpXNGFiR2wyWlRvdVkybGtMalptTXpRd1lqQTBOV1JtT1ROaU1qRUJBMVZwWXhReEx6RXZNREF3TVNBeE1qb3dNRG93TUNCQlRReE9iM1JUY0dWamFXWnBaV1FoTy9sZEJBczBid0FBQUFBQUFFQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQWFiR2wyWlRvdVkybGtMalptTXpRd1lqQTBOV1JtT1ROaU1qRUFBQUFBQUFBQUFBQUhUbTlUWTI5eVpRQUFBQUFFQUFBQUFBQUFBQUFBQUFBaE8vbGRCQXMwYndBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQVJwc2FYWmxPaTVqYVdRdU5tWXpOREJpTURRMVpHWTVNMkl5TVFBQUFBQUF1OUoxWkFjQUFBQUlTV1JsYm5ScGRIa09TV1JsYm5ScGRIbFZjR1JoZEdVSVEyOXVkR0ZqZEhNT1EyOXVkR0ZqZEhOVmNHUmhkR1VJUTI5dGJXVnlZMlVOUTI5dGJYVnVhV05oZEdsdmJoVkRiMjF0ZFc1cFkyRjBhVzl1VW1WaFpFOXViSGtBQUE9PTs=; expires=1685529659; endpointId={43531b21-c225-4ecb-8474-eed356332c0c}";


            //var getAllChats = SkypeApi.GetAllChats(registrationToken);
            //var groupChat = getAllChats.conversations.FirstOrDefault(elem => elem.threadProperties?.topic.Equals(config.BuildGroupChatName, StringComparison.InvariantCultureIgnoreCase) ?? false);
            //var groupChatId = groupChat.id;
            //var groupChatTargetLink = groupChat.targetLink;

            var groupChatId = "19:e6c8c69c3f11451da074814165deea9c@thread.skype";
            var groupChatTargetLink = "https://azeus1-client-s.gateway.messenger.live.com/v1/threads/19:e6c8c69c3f11451da074814165deea9c@thread.skype";

            //SkypeApi.SendMessage(rawLocation, groupChatId, username, registrationToken, "ma duc sa ma pis");



            //var contacts = SkypeApi.GetAllContacts(token.skypeid, token.Token);


            // 
        }
    }
}