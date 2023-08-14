using GetMeThatPage.Parser;
using HtmlAgilityPack;


namespace GetMeThatPage
{
    internal class Program
    {
        private static String webPageUrl = "http://books.toscrape.com/";
        private static string savePath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            Console.WriteLine("Scrap, scrap, get some HTML snack...");
            var web = new HtmlWeb();
            HtmlDocument? doc = web.Load(webPageUrl);
            Functions.CopyWebPageDataToDirectories(webPageUrl, savePath).Wait();
            Console.WriteLine("Download completed!");
            Console.ReadKey();
        }
    }
}