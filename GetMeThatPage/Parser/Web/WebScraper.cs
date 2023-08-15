using GetMeThatPage.Resources;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace GetMeThatPage.Parser.Web
{
    public class WebScraper
    {
        private const String hardcodedWebPageUrl = "http://books.toscrape.com/";
        private static string hardcodedSavePath = AppDomain.CurrentDomain.BaseDirectory;
        private string savePath;
        private string webPageUrl;
        private static WebScraper? _instance;
        static readonly HttpClient client = new HttpClient();
        public string SavePath
        {
            get { return savePath; }
            set { savePath = value; }
        }
        public string WebPageUrl
        {
            get { return webPageUrl; }
            set { webPageUrl = value; }
        }
        public static WebScraper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WebScraper();
                return _instance;
            }
        }
        public WebScraper()
        {
            webPageUrl = hardcodedWebPageUrl;
            savePath = hardcodedSavePath;
        }
        public WebScraper WebAddress(string webAddr = WebScraper.hardcodedWebPageUrl)
        {
            webPageUrl = webAddr;
            return this;
        }
        internal WebScraper SaveDirectory(string baseDirectory)
        {
            savePath = baseDirectory;
            return Instance;
        }
        internal WebScraper Scrape()
        {
            Functions.CopyWebPageDataToDirectories(this).Wait();
            return Instance;
        }
        public static void Traverse()
        {
            // TODO
        }
        public static HtmlDocument? SaveHTMLDocument(String webPageRootFolder, String urlRoot)
        {
            Uri absoluteUri = new Uri(urlRoot);
            HtmlWeb web = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 }; // Use the appropriate encoding 
            HtmlDocument? doc = web.Load(urlRoot);
            String fileName = Path.Combine(webPageRootFolder, Path.GetFileName(absoluteUri.LocalPath));

            // If users enters web page with realitev path as suffix
            if (absoluteUri.LocalPath.StartsWith("/"))
            {
                // Saving first WebPage or other
                fileName = Path.Combine(fileName, "index.html");
                doc.Save(fileName);
            }
            else
            {
                // TODO: Here we parse the whole URL root:
                // - Create folders if they dont exists
                // - Create traversal ??
                doc.Save(fileName);
            }

            return doc;
        }
        internal static Dictionary<string, List<string>> GetLinksDictionary(HtmlDocument doc)
        {
            var linksUrls = new Dictionary<string, List<string>>
            {
                { "img", new List<string>() },
                { "script", new List<string>() },
                { "link", new List<string>() },
                { "a", new List<string>() },
                { "css", new List<string>() }
            };
            HtmlNodeCollection allNodes = doc.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");

            foreach (var node in allNodes)
            {
                var srcAttribute = node.Attributes["src"];
                var hrefAttribute = node.Attributes["href"];
                if (srcAttribute != null)
                    if (!srcAttribute.Value.StartsWith("http://") && !srcAttribute.Value.StartsWith("https://"))
                        linksUrls[node.Name.ToLower()].Add(srcAttribute.Value);
                if (hrefAttribute != null)
                {
                    if (!hrefAttribute.Value.StartsWith("http://") && !hrefAttribute.Value.StartsWith("https://"))
                    {
                        linksUrls[node.Name.ToLower()].Add(hrefAttribute.Value);
                        //if (hrefAttribute.Value.ToString().ToLower().Contains("css"))
                        if (hrefAttribute.Value.ToString().ToLower().Contains("css"))
                            linksUrls["css"].Add(hrefAttribute.Value);
                    }
                }
            }
            return linksUrls;
        }
        internal static void SaveHTMLDocumentResources(Dictionary<string, List<string>> linksUrls, String urlRoot, string webPageRootFolder)
        {
            try
            {
                foreach (KeyValuePair<String, List<String>> kvp in linksUrls)
                {
                    List<String> resourcesList = kvp.Value;
                    if (kvp.Key.ToLower() != "css")
                        foreach (string resourceUrl in resourcesList)
                        {
                            String? localResourcePath = Path.Combine(webPageRootFolder, resourceUrl);
                            String webUri = Path.Combine(urlRoot, resourceUrl);
                            if (!string.IsNullOrEmpty(localResourcePath))
                            {
                                String? directoryPath = Path.GetDirectoryName(localResourcePath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    if (directoryPath != null)
                                        Directory.CreateDirectory(directoryPath);
                                }
                                // wait the task to finish, update for http 2.0 later
                                if (!System.IO.File.Exists(localResourcePath))
                                    DownloadAndSaveFiles(webUri, localResourcePath).Wait();
                            }
                        }
                }
            }
            catch (Exception ex) { }
        }
        public static async Task DownloadAndSaveFiles(string imageUrl, String filename)
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            await Task.CompletedTask;
        }
        internal static List<CSS> GetCSSFiles(Dictionary<string, List<string>> linksUrls, string urlRoot, string webPageRootFolder)
        {
            List<CSS> CSSList = new List<CSS>();
            foreach (String cssRelativePath in linksUrls["css"])
            {
                CSS css = new CSS();
                css.RelativelocalPath = cssRelativePath;
                css.LocalRootFolder = webPageRootFolder;
                css.AbsoluteLocalPath = Path.Combine(webPageRootFolder, cssRelativePath);
                css.FileName = Path.GetFileName(cssRelativePath);
                css.WebPageHost = urlRoot;
                if (System.IO.File.Exists(css.AbsoluteLocalPath)) GetURLPathsFromCSS(css);
                CSSList.Add(css);
            }
            return CSSList;
        }
        private static void GetURLPathsFromCSS(CSS css)
        {
            css.urlResources = new List<CssUrlResource>();
            try
            {
                string cssContent = System.IO.File.ReadAllText(css.AbsoluteLocalPath);
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
                        String remoteMiddlePath = Path.GetDirectoryName(css.RelativelocalPath);
                        Uri uri = new Uri(Path.Combine(css.WebPageHost, remoteMiddlePath, cssUrlResource.FileRelativeRemotePath));
                        cssUrlResource.FileAbsoluteRemotePath = uri;

                        // Setting Local Resource Location
                        String localStart = css.LocalRootFolder;
                        String middlePath = Path.GetDirectoryName(css.RelativelocalPath);
                        String temp1 = Path.Combine(localStart, middlePath);
                        String temp2 = cssUrlResource.FileRelativeRemotePath;  // "../fonts/glyphicons-halflings-regular.eot"
                        String temp3 = Path.Combine(temp1, temp2);
                        cssUrlResource.FileAbsoluteLocalPath = Path.GetFullPath(temp3);
                        css.urlResources.Add(cssUrlResource);

                        // Create directories if they dont exist
                        if (!Directory.Exists(cssUrlResource.FileAbsoluteLocalPath))
                        {
                            String? dirToCreate = Path.GetDirectoryName(cssUrlResource.FileAbsoluteLocalPath);
                            if (dirToCreate != null)
                            {
                                if (!Directory.Exists(dirToCreate))
                                    Directory.CreateDirectory(dirToCreate);
                            }
                        }
                        cssUrlResource.RenamedFileAbsoluteLocalPath = NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                        if (!System.IO.File.Exists(cssUrlResource.FileAbsoluteLocalPath))
                            WebScraper.DownloadAndSaveFiles(cssUrlResource.FileAbsoluteRemotePath.ToString(), NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath)).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static String NormalizeTheFileName(String filename)
        {
            int percentIndex = filename.IndexOf('%');
            if (percentIndex != -1)
                return filename.Substring(0, percentIndex);
            else
                return filename;
        }
        public static async Task RenameCSSResources(List<CSS> CSSFiles)
        {
            foreach (CSS css in CSSFiles)
            {
                String cssFilePath = css.AbsoluteLocalPath;
                string cssContent = System.IO.File.ReadAllText(cssFilePath);
                if (css.urlResources.Count > 0) Console.WriteLine($"cssFilePath:{cssFilePath}, countCSSResaource:{css.urlResources.Count}");
                foreach (CssUrlResource urlResource in css.urlResources)
                {
                    String oldAString = urlResource.RemoteFileName;
                    String newString = Path.GetFileName(urlResource.RenamedFileAbsoluteLocalPath);
                    if ((!string.IsNullOrEmpty(oldAString) && !string.IsNullOrEmpty(newString)))
                        cssContent = cssContent.Replace(oldAString, newString);
                }
                System.IO.File.WriteAllText(cssFilePath, cssContent);
            }
            await Task.CompletedTask;
        }
    }
}