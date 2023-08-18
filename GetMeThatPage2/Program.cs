using GetMeThatPage2.Helpers.FileSystemOperations;
using GetMeThatPage2.Helpers.WebOperations.Url;
using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.Url.UrlV2;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.FileSystemOperations.FileSystemHelpers;
using System.Collections.Generic;
using GetMeThatPage2.Helpers.WebOperations.Download;
using System; 

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

            IEnumerable<HtmlNode> imageNodes = document.DocumentNode.Descendants("img");
            
            new Downloader(appDirectory, rootUrl).saveHTMLDocumentImages(imageNodes).Wait();

            DateTime endTime = DateTime.Now; // Record end time

            // Calculate and display execution time
            TimeSpan executionTime = endTime - startTime;
            Console.WriteLine($"Execution Time: {(int)executionTime.TotalMinutes} minutes, {(int)executionTime.Seconds} seconds, {executionTime.Milliseconds} milliseconds");
            Console.ReadKey();

            // sync blocking
            // files exists:
            // Execution Time: 0 minutes, 5 seconds, 480 milliseconds
            // files dont exists:
            // Execution Time: 0 minutes, 5 seconds, 672 milliseconds

            // Paralel
            // files exists:

            // files dont exists:

        }


    }
}