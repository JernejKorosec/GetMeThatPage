using GetMeThatPage3.Helpers.Url;
using HtmlAgilityPack;

namespace GetMeThatPage3.Helpers.Url.Extensions
{
    public static class UrlStringExtensions
    {
        public static bool HasSchema(this string url)
        {
            bool startsWithSchema = false;

            if (string.IsNullOrEmpty(url))
                return false;

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
        public static string? ExtractDomain(this string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                string host = uri.Host;
                return host;
            }

            return null;
        }
        public static string? GetFileName(this string? url)
        {

            string? filename;
            string? result = Path.GetFileName(url);

            if (result == null)
                return null;
            else if (string.IsNullOrEmpty(result))
                return string.Empty;
            else
                filename = result;
            return filename;
        }
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
                return string.Empty;
            }
            return extension;
        }
        public static string? GetRelativeUri(this HtmlNode htmlNode)
        {
            string? path = null;

            switch (htmlNode.Name)
            {
                case "a":
                    path = htmlNode.Attributes["href"].Value;
                    break;
                case "link":
                    path = htmlNode.Attributes["href"].Value;
                    break;
                case "img":
                    path = htmlNode.Attributes["src"].Value;
                    break;
                case "script":
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
            return input.Replace("/", "\\");
        }
        public static string GetBaseUrl(string url)
        {
            Uri uri = new Uri(url);
            string baseUrl = $"{uri.Scheme}://{uri.Host}/";
            return baseUrl;
        }
    }
}