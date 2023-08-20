using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.WebOperations.Html.HtmlNodeExtensions;
using static GetMeThatPage2.Helpers.WebOperations.Url.UrlStringExtensions;
using GetMeThatPage2.Helpers.WebOperations.Download;
using GetMeThatPage2.Helpers.WebOperations.Html;

namespace GetMeThatPage2
{
    internal class Program
    {
        private static string rootUrl = "http://books.toscrape.com/";
        private static string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        static async Task Main(string[] args)
        {
            DateTime startTime = DateTime.Now; // Record start time
            List<ResourceFile> resources = new List<ResourceFile>();



            HtmlDocument document = await getHTMLDocument(rootUrl);
            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            List<HtmlNode> htmlNodeList = allDescendants.ToList();

            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory);
                resources.Add(r);
            }

            resources = resources.RemoveDuplicateValues();
            
            // sync non blocking
            //new Downloader(appDirectory, rootUrl).saveHTMLDocumentResources(allDescendants).Wait();

            // Todo async non blocking
            new Downloader(appDirectory, rootUrl).saveHTMLDocumentResourcesAsync(allDescendants).Wait();






            DateTime endTime = DateTime.Now; // Record end time
            // Calculate and display execution time
            TimeSpan executionTime = endTime - startTime;
            Console.WriteLine($"Execution Time: {(int)executionTime.TotalMinutes} minutes, {(int)executionTime.Seconds} seconds, {executionTime.Milliseconds} milliseconds");
            Console.ReadKey();
            Console.ReadKey();
            // sync non blocking 
            // Execution Time: 0 minutes, 41 seconds, 532 milliseconds
            // async non blocking
            // Execution Time: 0 minutes, 14 seconds, 460 milliseconds
        }
    }
}