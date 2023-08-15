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


            // First we create a List containing pages
            PagesList pagesList = new PagesList();
            Page page = new Page(urlRoot, savepathRoot, false);
            pagesList.AddPage(page);

            Traverse(pagesList).Wait();
            await Task.CompletedTask;


        }

        private static async Task<PagesList> Traverse(PagesList pagesList)
        {
            /*
             * Iterate through pagesList 
             *      IF none do a first search and first save of first page 
             *      on the end return list with added html pages
             *      check the one which is saved as isSaved
             */
            string? urlRoot = null;
            string? savepathRoot = null;

            if (pagesList != null)
                if (pagesList.Pages != null)
                    if (pagesList.Pages.Count == 1)
                    {
                        var firstPage = pagesList.Pages.FirstOrDefault();
                        if (firstPage != null && firstPage.IsSaved == false)
                        {
                            urlRoot = firstPage.UrlRoot;
                            savepathRoot = firstPage.LocalRoot;
                            if (urlRoot != null && savepathRoot != null)
                            {
                                // Sets the flag that it is processed
                                firstPage = await SinglePageSave(firstPage);
                            }
                        }
                    }
                    else if (pagesList.Pages.Count > 0)
                    {
                        //for Loop

                    }
                    else
                    {
                        await Task.CompletedTask;

                    }

            return pagesList;
        }
        private static async Task<Page> SinglePageSave(Page page)
        {
            string? urlRoot = page.UrlRoot.ToString();
            string? savepathRoot = page.LocalRoot.ToString();
            Boolean? isSaved = page.IsSaved;
            try
            {
                if (page != null && page.UrlRoot != null && page.LocalRoot != null && page.IsSaved == false)
                {
                    String webPageRootFolder = FileParser.createDirectoryFromUrl(savepathRoot, urlRoot);
                    HtmlDocument? doc = WebScraper.SaveHTMLDocument(webPageRootFolder, urlRoot);
                    Dictionary<string, List<string>> linksUrls = WebScraper.GetLinksDictionary(doc);
                    WebScraper.SaveHTMLDocumentResources(linksUrls, urlRoot, webPageRootFolder);
                    List<CSS> CSSFiles = WebScraper.GetCSSFiles(linksUrls, urlRoot, webPageRootFolder);
                    WebScraper.RenameCSSResources(CSSFiles).Wait();
                    page.IsSaved = true;
                }
            }
            catch (Exception ex) { } 
            return page;
        }
    }
}