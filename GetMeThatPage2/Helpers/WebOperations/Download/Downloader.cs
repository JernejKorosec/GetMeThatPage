using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.FileSystemOperations.FileSystemHelpers;
using System.ComponentModel.DataAnnotations;
using static GetMeThatPage2.Helpers.WebOperations.Url.UrlStringExtensions;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Concurrent;
using GetMeThatPage2.Helpers.WebOperations.Css;

namespace GetMeThatPage2.Helpers.WebOperations.Download
{
    public class Downloader
    {
        private string rootUrl { get; set; }
        private string appDirectory { get; set; }
        //private ConcurrentBag<CSS> cssFilesBag = new ConcurrentBag<CSS>();

        public Downloader(string _appDirectory, string _rootUrl)
        {
            rootUrl = _rootUrl;
            appDirectory = _appDirectory;
        }
        public async Task saveHTMLDocumentImages(IEnumerable<HtmlNode> imageNodes)
        {
            foreach (var imageNode in imageNodes)
            {
                string fileRelativeUrl = imageNode.GetAttributeValue("src", "");

                Uri combinedUri = getWebFileAbsolutePath(rootUrl, fileRelativeUrl);

                string absoluteFilePath = getFileAbsolutePath(appDirectory, rootUrl, fileRelativeUrl);

                string? directoryPath = Path.GetDirectoryName(absoluteFilePath);

                if (!string.IsNullOrWhiteSpace(fileRelativeUrl) && !string.IsNullOrEmpty(directoryPath))
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                        Console.WriteLine("Directory created: " + directoryPath);
                    }
                    if (!File.Exists(absoluteFilePath))
                        DownloadAndSaveFile(combinedUri.AbsoluteUri, absoluteFilePath).Wait();
                }
            }
        }
        public async Task saveHTMLDocumentResources(HtmlNodeCollection resourceNodes)
        {
            List<string> savedResourcePath = new List<string>();
            List<string> cssResourcePath = new List<string>();
            // Save all resources of HTML File
            foreach (HtmlNode? resource in resourceNodes) { 
                SaveResource(resource.ReturnRelativePath());
                savedResourcePath.Add(resource.ReturnRelativePath());
                if (resource.IsCss()) cssResourcePath.Add(resource.ReturnRelativePath());
            }

            // Parse all the CSS 
            List<CSS> CSSFiles = CSS.GetCSSFiles(cssResourcePath, rootUrl, appDirectory);
            CSS.RenameCSSResources(CSSFiles).Wait();

        }
        public void SaveResource(string fileRelativeUrl)
        {
            Uri absoluteFileWebUri = getWebFileAbsolutePath(rootUrl, fileRelativeUrl);
            string absoluteFilePath = getFileAbsolutePath(appDirectory, rootUrl, fileRelativeUrl);
            string? directoryPath = Path.GetDirectoryName(absoluteFilePath);
            if (!string.IsNullOrWhiteSpace(fileRelativeUrl) && !string.IsNullOrEmpty(directoryPath))
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine("Directory created: " + directoryPath);
                }
                if (!File.Exists(absoluteFilePath))
                    DownloadAndSaveFile(absoluteFileWebUri.AbsoluteUri, absoluteFilePath).Wait();
            }
        }
        public async Task saveHTMLDocumentImagesAsync(IEnumerable<HtmlNode> imageNodes)
        {


        }
    }

    public static class HtmlNodeExtensions
    {
        // Todo For other pages
        /*
        <img>: Specifies the source URL for an image.
        <script>: Specifies the source URL for an external script.
        <iframe>: Specifies the source URL for an embedded frame.
        <audio>: Specifies the source URL for an audio file.
        <video>: Specifies the source URL for a video file.
        <source>: Used inside <audio> or<video> to specify alternative media sources.
        */
        public static bool IsImage(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("img"))
                return node.Attributes["src"] != null;
            return false;
        }
        public static bool IsScript(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("script"))
                return node.Attributes["src"] != null;
            return false;
        }
        public static bool IsAnchor(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("a"))
                return node.Attributes["href"] != null;
            return false;
        }
        public static bool IsLink(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("link"))
                return node.Attributes["href"] != null;
            return false;
        }
        public static string ReturnRelativePath(this HtmlNode node)
        {
            if (node.IsImage() || node.IsScript())
            {
                HtmlAttribute htmlAttr = node.Attributes["src"];
                if (htmlAttr != null)
                    if (!htmlAttr.Value.HasSchema())
                    {
                        return htmlAttr.Value;
                    }
            }
            else if (node.IsAnchor() || node.IsLink())
            {
                HtmlAttribute htmlAttr = node.Attributes["href"];
                if (htmlAttr != null)
                    if (!htmlAttr.Value.HasSchema())
                    {
                        return htmlAttr.Value;
                    }
            }
            return "";
        }
    }

    // inside html
    //  <link rel="stylesheet" type="text/css" href="static/oscar/css/styles.css" />
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
