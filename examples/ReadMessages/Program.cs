using SkSharp;

var credentials = File.ReadAllText("../credentials.txt").Split('\n');
var username = credentials[0];
var password = credentials[1];
var groupName = credentials[2];

var skypeApi = await new SkypeApi().Login(username, password, "tokenCacheFile.json");
var chatRoom = await skypeApi.GetChatRoomByName(groupName);

// chatRoom.onMessage += (message) => {
//     Console.WriteLine($"[{message.Sender.DisplayName}] {message.Body}");
// };