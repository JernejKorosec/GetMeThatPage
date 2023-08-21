using GetMeThatPage3.Helpers.Url.Extensions;
using HtmlAgilityPack;

namespace GetMeThatPage3.Helpers.Css.Extensions
{
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
        public static bool RelativePathContainsCSSFile(string path)
        {
            return Path.GetExtension(path).ToLower().Equals(".css");
        }
        public static String NormalizeTheFileName(String filename)
        {
            int percentIndex = filename.IndexOf('%');
            if (percentIndex != -1)
                return filename.Substring(0, percentIndex);
            else
                return filename;
        }
    }
}