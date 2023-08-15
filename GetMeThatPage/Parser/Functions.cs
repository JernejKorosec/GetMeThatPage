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

    public static class PagesList
    {
        public static string? urlRoot { get; set; }
        public static string? savepathRoot { get; set; }
        public static List<Page>? Pages { get; set; }
    }
    public class Page
    {
        public static string? urlRoot { get; set; }
        public static string? localRoot { get; set; }
        public static bool? isSaved { get; set; }
    }
}