using HtmlAgilityPack;

namespace GetMeThatPage
{
    internal class Program
    {
        private static Stream filePath;

        static void Main(string[] args)
        {
            Console.WriteLine("Scrap, scrap, get some HTML snack...");

            // From Web
            var url = "http://books.toscrape.com/";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            Console.ReadKey();
        }
    }
}