namespace SkSharp.Models.SkypeApiModels
{
    public class SkypeEmotes
    {
        public string packsRoot { get; set; }
        public string emoticonsRoot { get; set; }
        public string itemsRoot { get; set; }
        public List<Pack> packs { get; set; }
        public List<Item> items { get; set; }
    }

    public class Default
    {
        public int firstFrame { get; set; }
        public int framesCount { get; set; }
        public object framesCountOptimized { get; set; }
        public object playBack { get; set; }
        public int fps { get; set; }
    }

    public class Facebook
    {
        public List<string> shortcuts { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string type { get; set; }
        public List<string> shortcuts { get; set; }
        public bool visible { get; set; }
        public bool useInSms { get; set; }
        public Media media { get; set; }
        public string etag { get; set; }
        public string description { get; set; }
        public List<object> keywords { get; set; }
        public Facebook facebook { get; set; }
    }

    public class Media
    {
        public Default @default { get; set; }
    }

    public class Pack
    {
        public string id { get; set; }
        public string etag { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string copyright { get; set; }
        public bool isHidden { get; set; }
        public string price { get; set; }
        public object expiry { get; set; }
        public List<string> items { get; set; }
    }
}
