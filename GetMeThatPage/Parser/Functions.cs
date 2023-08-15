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

            PagesList pagesList = new PagesList();
            Page page = new Page();




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
}