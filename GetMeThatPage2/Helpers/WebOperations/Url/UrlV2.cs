using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage2.Helpers.WebOperations.Url
{
    // C#'s expression-bodied property syntax
    // Read only properties
    public class Schema
    {
        public static string Http => "http://";
        public static string Https => "https://";
    }
    public class UrlV2
    {
        public string ParsePageRoot { get; set; }       // Original hardcoded value
        public string Schema { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public string LocalPath { get; set; }
        public string Filename { get; set; }
        public UrlV2(string parsePageRoot)
        {
            ParsePageRoot = parsePageRoot;
        }

        public void parse(string url)
        {

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


        public static Uri ConvertToUri(string url, string baseUri, string relativePath, string filename)
        {
            // Creating an empty URI using an empty string
            Uri emptyUri = new Uri("");
            Console.WriteLine(emptyUri.ToString()); // Output: ""

            // Creating a relative URI
            //Uri relativeUri = new Uri("/path/page?query=123", UriKind.Relative);
            Uri relativeUri = new Uri("/path/page?query=123", UriKind.Relative);
            Console.WriteLine(relativeUri.ToString()); // Output: /path/page?query=123

            // Concatenate base URI and relative URI to create a new URI
            //Uri concatenatedUri = new Uri(baseUri, relativeUri);

            //Console.WriteLine(concatenatedUri.ToString()); // Output: https://www.example.com/path/page?query=123

            return relativeUri;
        }
    }
}