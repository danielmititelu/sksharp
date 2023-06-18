using SkSharp;

var credentials = File.ReadAllText("../credentials.txt").Split('\n');
var username = credentials[0];
var password = credentials[1];
var groupName = credentials[2];

var skypeApi = await new SkypeApi().Login(username, password, "tokenCacheFile.json");
var chatRoom = await skypeApi.GetChatRoomByName(groupName);
await chatRoom.SendMessage("Hello from SkSharp!");
