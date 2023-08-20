using HtmlAgilityPack;
using GetMeThatPage2.Helpers.WebOperations.Download;
using GetMeThatPage2.Helpers.WebOperations.Url;
using System.Text.RegularExpressions;
using GetMeThatPage2.Helpers.WebOperations.Css;
using GetMeThatPage2.Helpers.WebOperations.ResourceFiles;

namespace GetMeThatPage2
{
    internal class Program
    {
        private static string rootUrl = "http://books.toscrape.com/";
        private static string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static async Task Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            List<ResourceFile> resources = new List<ResourceFile>();
            ResourceFile.WebRoot = rootUrl;
            ResourceFile.AppRoot = appDirectory;
            
            await GetHTMLLinks(rootUrl, resources);                             // Dobi vse linke od spletne strani in jih shrani v listo
            Downloader downloader = new Downloader(appDirectory, rootUrl);      // Nastavi zacetno pot za download in spletno stran s katere downloada
            resources = await downloader.SaveAllResources(resources);           // shrani s spleta vse linke ki so v listi na disk
            resources = await GetCSSLinksAsync(resources);                      // odpre cssje in shrani vse linke iz cssja

            TimeSpan executionTime = DateTime.Now - startTime; // Record end time
            Console.WriteLine($"Execution Time: {(int)executionTime.TotalMinutes} minutes, {(int)executionTime.Seconds} seconds, {executionTime.Milliseconds} milliseconds");
            Console.ReadKey();
            // Execution Time: 0 minutes, 1 seconds, 435 milliseconds
        }
        public static async Task<List<ResourceFile>> GetCSSLinksAsync(List<ResourceFile> resources)
        {
            foreach (ResourceFile resource in resources)
            {
                if (resource.Extension!=null && resource.AbsoluteFilePath!=null && resource.RelativePath != null)
                if (resource.Extension.ToLower().Equals(".css"))
                {
                    if (resource.isSaved) // Če je css na disku ga lahko odpremo
                    {
                        CSS css = new CSS();
                        css.FileName = resource.Filename;
                        css.AbsoluteLocalPath = resource.AbsoluteFilePath; // absolutna pot datoteke na disku
                        css.RelativelocalPath = resource.RelativePath;      // mapa v kateri se css nahaja, nalaganje fontov bo relativno na to pot
                        css.WebPageHost = UrlStringExtensions.GetBaseUrl(resource.AbsoluteUriPath);
                            
                        if (File.Exists(css.AbsoluteLocalPath))
                        {
                            css.UrlResources = new List<CssUrlResource>();
                            string cssContent = File.ReadAllText(css.AbsoluteLocalPath);
                            string pattern = @"url\((['""]?)(?!https?://)(?!data:)([^)]+)\1\)";
                            MatchCollection matches = Regex.Matches(cssContent, pattern);
                            foreach (Match match in matches)
                            {
                                if (match.Groups.Count >= 3)
                                {
                                    // Setting Remote Resource Location
                                    CssUrlResource cssUrlResource = new CssUrlResource();
                                    cssUrlResource.FileRelativeRemotePath = match.Groups[2].Value;
                                    cssUrlResource.RemoteFileName = Path.GetFileName(match.Groups[2].Value);
                                    Uri baseUriObject = new Uri(css.WebPageHost);
                                    Uri combinedUri = new Uri(baseUriObject, css.RelativelocalPath);
                                    Uri modifiedUri = new Uri(combinedUri, combinedUri.AbsolutePath.EndsWith("/") ? combinedUri.AbsolutePath : combinedUri.AbsolutePath + "/");
                                    Uri finalUri = new Uri(modifiedUri, cssUrlResource.FileRelativeRemotePath);
                                    cssUrlResource.FileAbsoluteRemotePath = finalUri;
                                    String path1 = ResourceFile.ScrapeRoot.Replace('/', '\\').TrimEnd('\\');
                                    String path2 = finalUri.AbsolutePath.Replace('/', '\\').TrimStart('\\');
                                    cssUrlResource.FileAbsoluteLocalPath = Path.Combine(path1, path2);
                                    css.UrlResources.Add(cssUrlResource);
                                    // Create directories if they dont exist
                                    if (!Directory.Exists(cssUrlResource.FileAbsoluteLocalPath))
                                    {
                                        String? dirToCreate = Path.GetDirectoryName(cssUrlResource.FileAbsoluteLocalPath);
                                        if (dirToCreate != null)
                                            if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
                                    }
                                    cssUrlResource.RenamedFileAbsoluteLocalPath = CSS.NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                                    if (!File.Exists(cssUrlResource.FileAbsoluteLocalPath))
                                    {
                                        String temp1 = cssUrlResource.FileAbsoluteRemotePath.ToString();
                                        String temp2 = CSS.NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                                        await FileDownloader.DownloadAndSaveFile(temp1, temp2);
                                    }
                                }
                            }
                            foreach (CssUrlResource urlResource in css.UrlResources)
                            {
                                String oldAString = urlResource.RemoteFileName;
                                String newString = Path.GetFileName(urlResource.RenamedFileAbsoluteLocalPath);
                                if ((!string.IsNullOrEmpty(oldAString) && !string.IsNullOrEmpty(newString)))
                                    cssContent = cssContent.Replace(oldAString, newString);
                            }
                            File.WriteAllText(css.AbsoluteLocalPath, cssContent);
                        }
                    }
                }
            }
            return resources;
        }
        public static async Task GetHTMLLinks(string rootUrl, List<ResourceFile> resources)
        {
            HtmlDocument document = await FileDownloader.DownloadHTMLDocument(rootUrl);
            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            List<HtmlNode> htmlNodeList = allDescendants.ToList();

            List<ResourceFile> newResources = new List<ResourceFile>();
            List<ResourceFile> uniqueResources;
            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory); // Todo 2 konstruktorja za vhodni pogoj ko je lsita resourceov prazna
                if (!htmlNode.GetRelativeUri().HasSchema())
                    newResources.Add(r);
            }
            lock (resources)
            {
                resources.AddRange(newResources);
                uniqueResources = resources.RemoveDuplicateValues();
                resources.Clear();
                resources.AddRange(uniqueResources);
            }

        }
    }
}