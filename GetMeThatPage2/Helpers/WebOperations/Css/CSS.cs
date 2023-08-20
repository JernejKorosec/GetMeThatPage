using GetMeThatPage2.Helpers.WebOperations.Url;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace GetMeThatPage2.Helpers.WebOperations.Css
{
    public class CSS
    {
        public CSS() { }
        public string? LocalRootFolder { get; set; }
        public string? RelativelocalPath { get; set; }
        public string? FileRelativeRemotePath { get; set; }
        public string? AbsoluteLocalPath { get; set; }
        public string? FileAbsoluteRemotePath { get; set; }
        public string? WebPageHost { get; set; }
        public string? FileName { get; set; }
        public List<CssUrlResource> UrlResources { get; set; }
        public static String NormalizeTheFileName(String filename)
        {
            int percentIndex = filename.IndexOf('%');
            if (percentIndex != -1)
                return filename.Substring(0, percentIndex);
            else
                return filename;
        }
    }
    public static class CSSExtensions
    {
        public static bool IsCss(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("link"))
            {
                HtmlAttribute htmlAttr = node.Attributes["href"];
                if (htmlAttr != null)
                    if (!htmlAttr.Value.HasSchema())
                        return RelativePathContainsCSSFile(htmlAttr.Value);
            }
            return false;
        }
        // Optimization: proper would be to check  rel="stylesheet" type="text/css"
        public static bool RelativePathContainsCSSFile(string path)
        {
            return Path.GetExtension(path).ToLower().Equals(".css");
        }
    }
}