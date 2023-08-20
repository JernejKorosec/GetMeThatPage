using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.FileSystemOperations.FileSystemHelpers;
using GetMeThatPage2.Helpers.WebOperations.Url;
using GetMeThatPage2.Helpers.WebOperations.Css;
using GetMeThatPage2.Helpers.WebOperations.Html;

namespace GetMeThatPage2.Helpers.WebOperations.Download
{
    public class Downloader
    {
        private string rootUrl { get; set; }
        private string appDirectory { get; set; }
        public Downloader(string _appDirectory, string _rootUrl)
        {
            rootUrl = _rootUrl;
            appDirectory = _appDirectory;
        }
        #region added two async functions
        public async Task saveHTMLDocumentResourcesAsync(HtmlNodeCollection resourceNodes)
        {
            List<string> savedResourcePath = new List<string>();
            List<string> cssResourcePath = new List<string>();
            List<Task> resourceTasks = new List<Task>();

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
            await CSS.RenameCSSResources(CSSFiles); //FIXME: Collection has changed
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
                    //await DownloadAndSaveFile(absoluteFileWebUri.AbsoluteUri, absoluteFilePath);
                    await FileDownloader.DownloadAndSaveFile(absoluteFileWebUri.AbsoluteUri, absoluteFilePath);
                }
            }
        }

        #endregion
        
        
        #region added two async functions 2
        public async Task<List<ResourceFile>> SaveAllResources(List<ResourceFile> resources)
        {
            List<Task<ResourceFile>> resourceTasks = new List<Task<ResourceFile>>();
            foreach (ResourceFile resource in resources)
            {
                Task<ResourceFile> resourceTask = SaveSingleResource(resource);
                resourceTasks.Add(resourceTask);
            }
            ResourceFile[] updatedResourcesArray = await Task.WhenAll(resourceTasks);
            List<ResourceFile> updatedResourcesList = updatedResourcesArray.ToList();
            return updatedResourcesList;
        }
        public async Task<ResourceFile> SaveSingleResource(ResourceFile resourceFile)
        {
            string? absoluteFilePath = resourceFile.AbsoluteFilePath;
            string? directoryPath = Path.GetDirectoryName(absoluteFilePath);
            string? fileRelativeUrl = resourceFile.RelativeFilePath;
            if (!string.IsNullOrWhiteSpace(fileRelativeUrl) && !string.IsNullOrEmpty(directoryPath))
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                if (!File.Exists(absoluteFilePath)) {
                    resourceFile.isSaved = await FileDownloader.DownloadAndSaveFile2(resourceFile.AbsoluteUriFilePath, absoluteFilePath);
                }
                else
                {
                    resourceFile.isSaved = true;
                }
            }
            return resourceFile;
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
        {
            List<HtmlNode> HtmlNodeList = nodes.ToList();
            HtmlNodeList = nodes.GroupBy(node => node.OuterHtml)
                                   .Select(group => group.First())
                                   .ToList();
        }
        public static void RemoveDuplicateAnchorNodes(HtmlNodeCollection nodes)
        {
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