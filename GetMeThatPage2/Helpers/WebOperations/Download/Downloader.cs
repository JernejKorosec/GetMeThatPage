using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.FileSystemOperations.FileSystemHelpers;
using System.ComponentModel.DataAnnotations;
using GetMeThatPage2.Helpers.WebOperations.Url;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Concurrent;
using GetMeThatPage2.Helpers.WebOperations.Css;
using System.Xml.Linq;

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

            return;
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

        #region added two async functions
        public async Task saveHTMLDocumentResourcesAsync(HtmlNodeCollection resourceNodes)
        {
            List<string> savedResourcePath = new List<string>();
            List<string> cssResourcePath = new List<string>();
            List<Task> resourceTasks = new List<Task>();


            //HtmlNodeExtensions.RemoveDuplicateNodes(resourceNodes);
            
            HtmlNodeExtensions.RemoveDuplicateAnchorNodes(resourceNodes);


            // Save all resources of HTML File concurrently
            foreach (HtmlNode? resource in resourceNodes)
            {
                Task resourceTask = Task.Run(async () =>
                {
                    string relativePath = resource.ReturnRelativePath();
                    await SaveResourceAsync(relativePath);
                    savedResourcePath.Add(relativePath);
                    if (resource.IsCss()) cssResourcePath.Add(relativePath);
                });

                resourceTasks.Add(resourceTask);
            }

            await Task.WhenAll(resourceTasks);

            // Parse all the CSS concurrently
            List<CSS> CSSFiles = CSS.GetCSSFiles(cssResourcePath, rootUrl, appDirectory);
            await CSS.RenameCSSResources(CSSFiles);
        }
        public async Task SaveResourceAsync(string fileRelativeUrl)
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
                {
                    await DownloadAndSaveFile(absoluteFileWebUri.AbsoluteUri, absoluteFilePath);
                }
            }
        }
        #endregion
    }

    public static class HtmlNodeExtensions
    {
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
        
        //FIXME: Remove from here
        public static void RemoveDuplicateNodes(HtmlNodeCollection nodes)
        //public static HtmlNodeCollection RemoveDuplicateNodes(HtmlNodeCollection nodes)
        {
            List<HtmlNode> HtmlNodeList = nodes.ToList();

            HtmlNodeList = nodes.GroupBy(node => node.OuterHtml)
                                   .Select(group => group.First())
                                   .ToList();



            //return new HtmlNodeCollection();
            // Create a new HtmlNodeCollection using the constructor that accepts IEnumerable<HtmlNode>
            //return uniqueNodes;
        }
        public static void RemoveDuplicateAnchorNodes(HtmlNodeCollection nodes)
        {
            int stopmehere = 0;
            //IEnumerable<HtmlAttribute> attributes = nodes.SelectMany(node => node.Attributes.Where(attr => attr.Name.Equals("a")));

            List<bool> something = nodes.Select(node => node.Name.ToLower().Equals("a")).ToList();  

            var distinctNodes = nodes.Distinct(new HtmlNodeEqualityComparer()).ToList();
            nodes.Clear();
            foreach (var node in distinctNodes)
            {
                nodes.Add(node);
            }
        }
    }
    public class HtmlNodeEqualityComparer : IEqualityComparer<HtmlNode>
    {
        public bool Equals(HtmlNode x, HtmlNode y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.OuterHtml == y.OuterHtml;
        }
        public int GetHashCode(HtmlNode obj)
        {
            return obj.OuterHtml.GetHashCode();
        }
    }

}
