using HtmlAgilityPack;
using GetMeThatPage.Resources;
using GetMeThatPage.Parser.Web;
using GetMeThatPage.Parser.File;

namespace GetMeThatPage.Parser
{
    public static class Functions
    {
        internal static async Task CopyWebPageDataToDirectories(WebScraper webScraper)
        {
            string urlRoot = webScraper.WebPageUrl;
            string savepathRoot = webScraper.SavePath;
            // TODO is to refactor this for traversing

            // Tole pohendla za en html
            Dictionary<string, List<string>> linksUrls = await CopyWebPageDataToDirectories(urlRoot, savepathRoot);
            List<string> listOfLinksToParse = linksUrls["a"].Distinct().ToList();
            await Task.CompletedTask;
        }
        private static async Task<Dictionary<string, List<string>>> CopyWebPageDataToDirectories(string urlRoot, string savepathRoot)
        {
            // Naredi direktorij iz podanega urlja in podane lokalne poti
            String webPageRootFolder = FileParser.createDirectoryFromUrl(savepathRoot, urlRoot);
            HtmlDocument? doc = WebScraper.SaveHTMLDocument(webPageRootFolder, urlRoot);
            Dictionary<string, List<string>> linksUrls = WebScraper.GetLinksDictionary(doc);
            WebScraper.SaveHTMLDocumentResources(linksUrls, urlRoot, webPageRootFolder);
            List<CSS> CSSFiles = WebScraper.GetCSSFiles(linksUrls, urlRoot, webPageRootFolder);
            WebScraper.RenameCSSResources(CSSFiles).Wait();
            return linksUrls;
        }
    }
    public class PagesList
    {
        private static readonly object lockObject = new object();  // Lock object for synchronization
        public string? UrlRoot { get; set; }
        public string? SavepathRoot { get; set; }
        public List<Page> Pages { get; } = new List<Page>();  // Initialize the list
        public void AddPage(Page page)
        {
            lock (lockObject) // if threads try to add the page and another thread is adding it wait.
            {
                Pages.Add(page);
            }
        }
        public void RemovePage(Page page)
        {
            lock (lockObject) // if threads try to remove the page and another thread is adding it wait.
            {
                Pages.Remove(page);
            }
        }
    }
    public class Page
    {
        public string? UrlRoot { get; set; }
        public string? LocalRoot { get; set; }
        public bool? IsSaved { get; set; }
    }
}