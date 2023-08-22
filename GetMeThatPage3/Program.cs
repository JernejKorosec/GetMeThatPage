using GetMeThatPage3.Scraper;
using System.Collections.Concurrent;

namespace GetMeThatPage3
{
    internal class Program
    {
        private static string rootUrl = "http://books.toscrape.com/";
        private static string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static async Task Main(string[] args)
        {
            WebScraper spider = new WebScraper(rootUrl,appDirectory);
            await spider.RunExampleAsync();
            Console.ReadKey();
        }
    }
}