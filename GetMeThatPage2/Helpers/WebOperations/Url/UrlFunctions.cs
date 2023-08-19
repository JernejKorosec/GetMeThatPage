using HtmlAgilityPack;
using System.IO;
using System.Xml.Linq;

namespace GetMeThatPage2.Helpers.WebOperations.Url
{
    // C#'s expression-bodied property syntax
    // Read only properties
    public class Schema
    {
        public static string Http => "http://";
        public static string Https => "https://";
    }
    public class UrlFunctions
    {
        public string ParsePageRoot { get; set; }       // Original hardcoded value
        public string Schema { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public string LocalPath { get; set; }
        public string Filename { get; set; }
        public UrlFunctions(string parsePageRoot)
        {
            ParsePageRoot = parsePageRoot;
        }
    }
    public static class UrlStringExtensions
    {
        public static bool HasSchema(this string url) // referenca na this
        {
            bool startsWithSchema = false;

            if (string.IsNullOrEmpty(url))
                return false;

            //if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            //    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return true;

            if (url.StartsWith(Schema.Http, StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith(Schema.Https, StringComparison.OrdinalIgnoreCase)) return true;

            return startsWithSchema;
        }
        public static string RemoveSchema(this string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.HasSchema())
                {
                    if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                        return url.Substring("http://".Length);
                    else if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        return url.Substring("https://".Length);
                }
            }
            return url;
        }
        public static string AddSchema(this string url, Schema schema)
        {
            return schema.Equals(Schema.Http) ? Schema.Http + url : Schema.Https + url;
        }
        public static string ExtractDomain(this string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                string host = uri.Host;
                return host;
            }

            return null;
        }
        /// <summary>
        /// Gets Filename from Relative Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>filename</returns>
        public static string? GetFileName(this string? url)
        {
            
            String? filename = null;
            string? result = Path.GetFileName(url);

            if(result == null){
                // path is null
                return null;

            }
            else if (string.IsNullOrEmpty(result)) {
                // It is directory not file
                return String.Empty;
            }
            else
            {
                filename = result;
            }
            return filename;
        }
         /// <summary>
         /// Get Filename Extension from path
         /// </summary>
         /// <param name="path">url path</param>
         /// <returns> - extension if path is valid 
         /// null if path is null , 
         /// Empty String if path doesnt have extension</returns>
        public static string? GetExtensionFromFilename(this string? path)
        {
            string? extension = Path.GetExtension(path);
            if (extension == null)
            {
                // path is null
                return null;
            }
            else if (string.IsNullOrEmpty(extension))
            {
                // path doesnt have extension
                return String.Empty;
            }
            return extension;
        }
        public static string? GetRelativeUri(this HtmlNode htmlNode)
        {
            string? path = null;

            switch (htmlNode.Name)
            {
                case ("a"):
                    path = htmlNode.Attributes["href"].Value;
                    break;
                case ("link"):
                    path = htmlNode.Attributes["href"].Value;
                    break;
                case ("img"):
                    path = htmlNode.Attributes["src"].Value;
                    break;
                case ("script"):
                    path = htmlNode.Attributes["src"].Value;
                    break;

                default:
                    break;
            }
            return path;
        }
        public static string ReplaceBackslashesWithForwardslashes(this string input)
        {
            return input.Replace("\\", "/");
        }
        public static string ReplaceForwardslashesWithBackslashes(this string input)
        {
            return input.Replace("/","\\");
        }
    }
}