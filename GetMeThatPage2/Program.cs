using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
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

            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");

            new Downloader(appDirectory, rootUrl).saveHTMLDocumentResources(allDescendants).Wait();

            DateTime endTime = DateTime.Now; // Record end time

            // Calculate and display execution time
            TimeSpan executionTime = endTime - startTime;
            Console.WriteLine($"Execution Time: {(int)executionTime.TotalMinutes} minutes, {(int)executionTime.Seconds} seconds, {executionTime.Milliseconds} milliseconds");
            Console.ReadKey();

            // sync blocking

            // Execution Time: 0 minutes, 43 seconds, 189 milliseconds

            // Paralel

        }
    }
}