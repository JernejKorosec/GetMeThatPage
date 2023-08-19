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

            HtmlDocument document = await getHTMLDocument(rootUrl);

            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");

            List<HtmlNode> htmlNodeList = allDescendants.ToList();
            List<ResourceFile> resources = new List<ResourceFile>();
            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory);
                resources.Add(r);
                //Console.WriteLine($"resources.Count: {r.RelativeFilePath}");
            }

            //Console.WriteLine($"resources.Count: {resources.Count}");


            // Remove duplicates
            List<ResourceFile> uniqueResources = resources
            .GroupBy(resource => resource.absoluteFilePath)
            .Select(group => group.First())
            .ToList();





            /*
            foreach (var resource in uniqueResources)
            {
                Console.WriteLine(resource.AbsoluteFileDirectoryPath);
            }
            */
            Console.WriteLine($"uniqueResources.Count: {uniqueResources.Count}");

            /*
            RemoveNodesWithDuplicatePaths(allDescendants);
            int stopmenot = 0;
            RemoveNodesWithIndexHtml(allDescendants);
            int stopmenot1 = 0;
            */
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