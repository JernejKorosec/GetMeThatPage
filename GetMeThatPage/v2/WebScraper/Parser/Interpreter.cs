using GetMeThatPage.Parser;
using GetMeThatPage.Resources;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetMeThatPage.v2.WebScraper.Parser
{
    public class Interpreter
    {
        

        #region HTML Dcoument parsing functionality (Parses html for links, a href, src) Ant stored them in dictionary

        public static Dictionary<string, List<string>> GetLinksDictionary(HtmlDocument doc)
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
                        if (hrefAttribute.Value.ToString().ToLower().Contains("css")) linksUrls["css"].Add(hrefAttribute.Value);
                    }
                }
            }
            if (linksUrls.TryGetValue("a", out var aLinks)) linksUrls["a"] = aLinks.Distinct().ToList();
            return linksUrls;
        }
        #endregion
        #region Downloads resources from HTLM files (Images, js files, without css files)
        internal static void SaveHTMLDocumentResources(Dictionary<string, List<string>> linksUrls, DataEntry dataEntry)
        {
            String urlRoot = dataEntry.AbsoluteWebUri.ToString();
            string webPageRootFolder = dataEntry.RootLocalPathWithRootPage;
            try
            {
                foreach (KeyValuePair<String, List<String>> kvp in linksUrls)
                {
                    // GREMO CEZ CSS URL-je SAMO TO
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
                                if (!File.Exists(localResourcePath))
                                    Request.DownloadAndSaveFiles(webUri, localResourcePath).Wait();
                            }
                        }
                }
            }
            catch (Exception ex) { }
        }
        #endregion
        #region CSS parsing and renaming functionality
        internal static List<CSS> GetCSSFiles(Dictionary<string, List<string>> linksUrls, DataEntry dataEntry)
        {
            string urlRoot = dataEntry.RootLocalPathWithRootPage;
            string webPageRootFolder = dataEntry.RootLocalPathWithRootPage;

            List<CSS> CSSList = new List<CSS>();
            foreach (String cssRelativePath in linksUrls["css"])
            {
                CSS css = new CSS();
                css.RelativelocalPath = cssRelativePath;
                css.LocalRootFolder = webPageRootFolder;
                css.AbsoluteLocalPath = Path.Combine(webPageRootFolder, cssRelativePath);
                css.FileName = Path.GetFileName(cssRelativePath);
                css.WebPageHost = urlRoot;
                if (File.Exists(css.AbsoluteLocalPath)) GetURLPathsFromCSS(css);
                CSSList.Add(css);
            }
            return CSSList;
        }
        // Reads urls() function from css files, reading paths to fonts and downloads them
        private static void GetURLPathsFromCSS(CSS css)
        {
            css.urlResources = new List<CssUrlResource>();
            try
            {
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
                            Request.DownloadAndSaveFiles(cssUrlResource.FileAbsoluteRemotePath.ToString(), NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath)).Wait();
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
        //private static PagesList AddLinksFromDocumentToPageList(Dictionary<string, List<string>> linksUrls, List<DataEntry> currEntries, DataEntry dataentry)
        public static List<DataEntry> AddLinksFromDocumentToPageList(Dictionary<string, List<string>> linksUrls, List<DataEntry> currEntries, DataEntry dataentry)
        {
            Helpers.Functions func1 = new Helpers.Functions();
            //public DataEntry getDataEntryFromUrl(Uri fullUri, String hardcodedSavePath)

            

            
                

            foreach (String htmlRelativePath in linksUrls["a"])
            {

                /*
                string uriString = dataentry.Scheme + "://" + dataentry.WebHost + "/" + htmlRelativePath;
                Uri uriPrefix = new Uri(uriString);
                */
                Uri uriPrefix;


                if (htmlRelativePath.ToLower().Equals("index.html"))
                {
                    //uriPrefix = new Uri("/");
                }
                else
                {
                    //string filenameToRemove = "index.html";

                    int lastIndex = htmlRelativePath.LastIndexOf('/');
                    string substring = htmlRelativePath.Substring(0, lastIndex+1);

                    uriPrefix = new Uri(substring);
                    DataEntry temp = func1.getDataEntryFromUrl(uriPrefix, dataentry.RootLocalPathWithRootPage);
                }
                
                int stopmenote = 1;
            }

            //uriPrefix = new Uri("catalogue/category/books_1/");
            //System.UriFormatException: 'Invalid URI: The format of the URI could not be determined.'
            //PagesList pagesList
            /*
            PagesList currentPagesList = pagesList; // reference

            String currentUrlRoot = "";
            Page firstPage;
            if (currentPagesList != null)
                if (currentPagesList.Pages != null)
                    if (currentPagesList.Pages.Count == 1)
                    {
                        currentUrlRoot = pagesList.Pages.FirstOrDefault().UrlRoot;
                        firstPage = pagesList.Pages.FirstOrDefault();
                    }
            foreach (String htmlRelativePath in linksUrls["a"])
            {
                if (!pagesList.ContainsPageWithUrlRoot(htmlRelativePath))
                {
                    Page pageWithoutLocalAddress = new Page(htmlRelativePath, false);
                    pagesList.AddPage(pageWithoutLocalAddress);
                }
            }
            int stopmenot = 0;
            return currentPagesList;
            */



            return currEntries;
        }
        #endregion
    }
}