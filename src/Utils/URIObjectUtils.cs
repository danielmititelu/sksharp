using SkSharp.Models.Internal;
using SkSharp.Models.SkypeApiModels;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SkSharp.Utils
{
    internal static class URIObjectUtils
    {
        private const string API_ASM = "https://api.asm.skype.com/v1/objects";
        private const string API_ASM_LOCAL = "https://{0}1-api.asm.skype.com/v1/objects";

        internal enum ContentType
        {
            None,
            File,
            Image,
            Audio,
            Video,
        }

        internal static URIFileDetails ToURIFile(this SkypeMessage message, ContentType type)
        {
            URIObject messageURIObject;

            var serializer = new XmlSerializer(typeof(URIObject));
            using (var reader = new StringReader(message.Message))
            {
                messageURIObject = serializer.Deserialize(reader) as URIObject;
            }

            return messageURIObject.GetDetails(type);
        }

        internal static URIFileDetails GetDetails(this URIObject uriObject, ContentType type)
        {
            var match = Regex.Match(uriObject.Uri, $"{Regex.Escape(API_ASM)}/0-([a-z0-9]+)-");

            var urAsm = uriObject.Uri;

            if (match.Success)
            {
                var prefix = string.Format(API_ASM_LOCAL, match.Groups[1].Value);
                urAsm = uriObject.Uri.Replace(API_ASM, prefix);
            }

            var urlContent = $"{urAsm}/views/{type.GetContentPath()}";

            return new URIFileDetails
            {
                DownloadLink = urlContent,
                OriginalName = uriObject.OriginalName.V,
                Size = uriObject.FileSize.V
            };
        }

        private static string GetContentPath(this ContentType type)
        {
            return type switch
            {
                ContentType.File => "original",
                ContentType.Image => "imgpsh_fullsize",
                ContentType.Audio => "audio",
                ContentType.Video => "video",
                _ => throw new ArgumentException("Unknown type"),
            };
        }

    }
}
