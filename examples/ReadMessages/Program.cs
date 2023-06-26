using SkSharp;

var credentials = File.ReadAllText("../credentials.txt").Split('\n');
var username = credentials[0];
var password = credentials[1];
var groupName = credentials[2];

Console.WriteLine($"Starting program");

var skypeApi = await new SkypeApi().Login(username, password, "tokenCacheFile.json");
var chatRoom = await skypeApi.GetChatRoomByName(groupName);
chatRoom.OnMessage += (message) =>
{
    if (message.MessageType.Contains("Typing"))
    {
        Console.WriteLine($"[{message.Sender}] is typing...");
    }
    else
    {

        Console.WriteLine($"[{message.Sender}] {message.Message}");
    }
};

chatRoom.StartPolling();

Console.ReadLine();