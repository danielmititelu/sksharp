using SkSharp;

var credentials = File.ReadAllText("../credentials.txt").Split('\n');
var username = credentials[0];
var password = credentials[1];
var groupName = credentials[2];

var chat = new SkypeApi();
var loggedInSkypeApi = await chat.Login(username, password, "tokenCacheFile.json");
var user = loggedInSkypeApi.GetUserId();
Console.WriteLine($"Logged in as {user}");
// var room = await loggedInSkypeApi.GetRoomByName(groupName);



