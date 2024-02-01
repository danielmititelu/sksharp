using System.Xml.Serialization;

namespace SkSharp.Models.SkypeApiModels
{
    [XmlRoot(ElementName = "a")]
    public class A
    {

        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "OriginalName")]
    public class OriginalName
    {

        [XmlAttribute(AttributeName = "v")]
        public string V { get; set; }
    }

    [XmlRoot(ElementName = "FileSize")]
    public class FileSize
    {

        [XmlAttribute(AttributeName = "v")]
        public int V { get; set; }
    }

    [XmlRoot(ElementName = "URIObject")]
    public class URIObject
    {

        [XmlElement(ElementName = "a")]
        public A A { get; set; }

        [XmlElement(ElementName = "OriginalName")]
        public OriginalName OriginalName { get; set; }

        [XmlElement(ElementName = "FileSize")]
        public FileSize FileSize { get; set; }

        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }

        [XmlAttribute(AttributeName = "url_thumbnail")]
        public string UrlThumbnail { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "doc_id")]
        public string DocId { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
