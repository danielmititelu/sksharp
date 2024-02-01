namespace SkSharp.Models.Public
{
    public class FileDetails
    {
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public int Size { get; set; }

        public bool HasError { get; set; }
    }
}
