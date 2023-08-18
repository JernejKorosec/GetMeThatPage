using HtmlAgilityPack;
using GetMeThatPage.Resources;
using GetMeThatPage.Parser.Web;
using GetMeThatPage.Parser.File;
using System;

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
                                firstPage = await SinglePageSave(firstPage, pagesList);
                                int index = pagesList.Pages.IndexOf(firstPage);
                                if (index != -1)
                                    pagesList.Pages[index] = firstPage;

                                PagesList.Host = firstPage.UrlRoot;
                                PagesList.RootDirectory = firstPage.LocalRoot;

                                Traverse(pagesList).Wait();
                            }
                        }
                    }
                    else if (pagesList.Pages.Count > 1)
                    {
                        
                        Page firstUnsavedPage = pagesList.Pages.FirstOrDefault(page => page.IsSaved == false);
                        if (firstUnsavedPage != null)
                        {
                            String nextUrlRoot = firstUnsavedPage.UrlRoot;
                            if (nextUrlRoot.ToLower().StartsWith("index.html"))
                                firstUnsavedPage.IsSaved = true;

                            firstUnsavedPage = pagesList.Pages.FirstOrDefault(page => page.IsSaved == false);

                            firstUnsavedPage.IsSaved = false;
                            //firstUnsavedPage.LocalRoot = Path.GetDirectoryName(Path.Combine(PagesList.RootDirectory, firstUnsavedPage.UrlRoot) );
                            firstUnsavedPage.LocalRoot = Path.GetDirectoryName(PagesList.RootDirectory);
                            firstUnsavedPage.UrlRoot = new Uri(Path.Combine(PagesList.Host, firstUnsavedPage.UrlRoot)).ToString();

                            // <a href="../../a-light-in-the-attic_1000/index.html">


                            // Remove index.html from the last part of uri
                            int lastSlashIndex = firstUnsavedPage.UrlRoot.LastIndexOf('/');
                            if (lastSlashIndex >= 0) firstUnsavedPage.UrlRoot = firstUnsavedPage.UrlRoot.Substring(0, lastSlashIndex).ToString();
                            


                            firstUnsavedPage = await SinglePageSave(firstUnsavedPage, pagesList);
                            int index = pagesList.Pages.IndexOf(firstUnsavedPage);
                            if (index != -1) pagesList.Pages[index] = firstUnsavedPage;

                            Traverse(pagesList).Wait();


                            //PagesList.Host = 
                            //PagesList.RootDirectory = 

                            int stopHere = 0;
                            // I need to get LocalRoot. So i need Web host first
                            // Than i need a local address where i usually store everything and is changeable



                        }

                        if (pagesList.AllPagesAreSaved()) await Task.CompletedTask;
                    }
                    else
                    {
                        int stophere = 1;
                        await Task.CompletedTask;
                    }
            return pagesList;
        }
        // TODO: async rethink non blocking async flow
        private static async Task<Page> SinglePageSave(Page page, PagesList pageslist)
        {
            string urlRoot = page.UrlRoot;
            string savepathRoot = page.LocalRoot;
            Boolean? isSaved = page.IsSaved;
            //if (page == null) page = new Page("","",true);
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
                    // Add apges scraped from HTML (No need for other files)
                    pageslist = AddLinksFromDocumentToPageList(linksUrls, pageslist);
                    // prepend Host to web and prepend
                    page.IsSaved = true;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            
            
            }

            return page;

        }
        // TODO: async rethink non blocking async flow
        private static PagesList AddLinksFromDocumentToPageList(Dictionary<string, List<string>> linksUrls, PagesList pagesList)
        {
            PagesList currentPagesList = pagesList; // reference
            String currentUrlRoot = "";
            Page firstPage;
            if (currentPagesList != null)
                if (currentPagesList.Pages != null)
                    if (currentPagesList.Pages.Count == 1) { 
                        currentUrlRoot = pagesList.Pages.FirstOrDefault().UrlRoot;
                        firstPage = pagesList.Pages.FirstOrDefault();
                    }
            //else
            //  currentUrlRoot = GetWebHost();
            
            foreach (String htmlRelativePath in linksUrls["a"])
            {
                // If the page with the given url doesnt exists add it 
                // and than add the property IsSaved = false;
                if (!pagesList.ContainsPageWithUrlRoot(htmlRelativePath))
                {
                    Page pageWithoutLocalAddress = new Page(htmlRelativePath, false);
                    pagesList.AddPage(pageWithoutLocalAddress);
                }
            }


            int stopmenot = 0;


        

            return currentPagesList;
        }

    }
}