using SkSharp.Models.SkypeApiModels;
using SkSharp.Services;

namespace SkSharp.Utils
{
    public static class SkypeMessageHelpers
    {
        private static SkypeEmotes _hardcodedEmotes = new SkypeEmotes()
        {
            items = new List<Item>()
            {
                new Item() { id = "2714_heavycheckmark", shortcuts = new List<string>() { "(heavycheckmark)" } },
                new Item() { id = "274c_crossmark", shortcuts = new List<string>() { "(crossmark)" } },
                new Item() { id = "porgsurprised", shortcuts = new List<string>() { "(porgsurprised)" } },
                new Item() { id = "1f528_hammer", shortcuts = new List<string>() { "(hammer)" } }
            }
        };

        private static SkypeEmotes _emotes;

        private static SkypeEmotes GetSkypeEmotes()
        {
            return _emotes = _emotes ?? SkypePlublicService.GetAllEmotes().Result;
        }

        public static string Bold(string s)
        {
            return $"<b raw_pre=\"*\" raw_post=\"*\">{s}</b>";
        }

        public static string Italic(string s)
        {
            return $"<i raw_pre=\"_\" raw_post=\"_\">{s}</i>";
        }

        public static string Strike(string s)
        {
            return $"<s raw_pre=\"~\" raw_post=\"~\">{s}</s>";
        }

        public static string Mono(string s)
        {
            return $"<pre raw_pre=\"{{code}}\" raw_post=\"{{code}}\">{s}</pre>";
        }

        public static string Colour(string s, string colour)
        {
            return $"<font color=\"{colour}\">{s}</font>";
        }

        public static string Link(string url, string display = null)
        {
            return $"<a href=\"{url}\">{display ?? url}</a>";
        }

        public static string Emote(string shortcut, SkypeEmotes extraEmotes = null)
        {
            foreach (var emote in _hardcodedEmotes.items)
            {
                if (shortcut == emote.id)
                    return $"<ss type=\"{shortcut}\">{emote.shortcuts[0]}</ss>";
                else if (emote.shortcuts.Contains(shortcut))
                    return $"<ss type=\"{emote.id}\">{shortcut}</ss>";
            }
            foreach (var emote in GetSkypeEmotes().items)
            {
                if (shortcut == emote.id)
                    return $"<ss type=\"{shortcut}\">{emote.shortcuts[0]}</ss>";
                else if (emote.shortcuts.Contains(shortcut))
                    return $"<ss type=\"{emote.id}\">{shortcut}</ss>";
            }
            if (extraEmotes != null)
            {
                foreach (var emote in extraEmotes.items)
                {
                    if (shortcut == emote.id)
                        return $"<ss type=\"{shortcut}\">{emote.shortcuts[0]}</ss>";
                    else if (emote.shortcuts.Contains(shortcut))
                        return $"<ss type=\"{emote.id}\">{shortcut}</ss>";
                }
            }
            return shortcut;
        }

        //public static string Mention(SkypeUser user)
        //{
        //    return $"<at id=\"8:{user.id}\">{user.name}</at>";
        //}

        public static string MentionAll => "<at id=\"*\">all</at>";

        //public static string Quote(SkypeUser user, SkypeChat chat, DateTime timestamp, string content)
        //{
        //    var chatId = chat.id.Split(':')[0] == "19" ? chat.id : NoPrefix(chat.id);
        //    var unixTime = (int)(timestamp - new DateTime(1970, 1, 1)).TotalSeconds;
        //    var legacyTime = timestamp.ToString(timestamp.Date == DateTime.Today ? "H:mm:ss" : "dd/MM/yyyy H:mm:ss");
        //    return $"<quote author=\"{user.id}\" authorname=\"{user.name}\" conversation=\"{chatId}\" timestamp=\"{unixTime}\"><legacyquote>[{unixTime}] {user.name}: </legacyquote>{content}<legacyquote>\n\n<<< </legacyquote></quote>";
        //}

        public static string UriObject(string content, string type, string url, string thumb = null, string title = null, string desc = null, params (string key, string value)[] values)
        {
            var titleTag = title != null ? $"<Title>Title: {title}</Title>" : "<Title/>";
            var descTag = desc != null ? $"<Description>Description: {desc}</Description>" : "<Description/>";
            var thumbAttr = thumb != null ? $" url_thumbnail=\"{thumb}\"" : "";
            var valTags = string.Join("", values.Select(v => $"<{v.key} v=\"{v.value}\"/>"));
            return $"<URIObject type=\"{type}\" uri=\"{url}\"{thumbAttr}>{titleTag}{descTag}{valTags}{content}</URIObject>";
        }

        private static string NoPrefix(string chatId)
        {
            // Implement the logic for removing the prefix as needed
            // This is a placeholder method, adjust based on actual requirements
            return chatId;
        }
    }
}
