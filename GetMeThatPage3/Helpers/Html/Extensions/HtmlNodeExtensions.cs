using HtmlAgilityPack;

namespace GetMeThatPage3.Helpers.Html.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static bool IsLink(this HtmlNode htmlNode)
        {
            if (htmlNode.Name == "link")
                return true;
            return false;
        }
        public static bool IsAnchor(this HtmlNode htmlNode)
        {
            if (htmlNode.Name == "a")
                return true;
            return false;
        }
        public static bool IsImage(this HtmlNode htmlNode)
        {
            if (htmlNode.Name == "img")
                return true;
            return false;
        }
        public static bool IsScript(this HtmlNode htmlNode)
        {
            if (htmlNode.Name == "script")
                return true;
            return false;
        }
        public static bool HasHrefAttribute(this HtmlNode htmlNode)
        {
            if (htmlNode.IsAnchor() || htmlNode.IsLink())
                return true;
            return false;
        }
        public static bool HasSrcAttribute(this HtmlNode htmlNode)
        {
            if (htmlNode.IsImage() || htmlNode.IsScript())
                return true;
            return false;
        }
        public static string? GetHTMLNodeAttributeValue(this HtmlNode htmlNode)
        {
            string? url = null;
            if (htmlNode.HasHrefAttribute())
            {
                url = htmlNode.Attributes["href"].Value;
            }
            else if (htmlNode.HasSrcAttribute())
            {
                url = htmlNode.Attributes["src"].Value;
            }
            return url;
        }
    }
}
