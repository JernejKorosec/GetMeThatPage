using GetMeThatPage.Parser.Web;

namespace GetMeThatPage
{
    internal class Program
    {
        private static String webPageUrl = "http://books.toscrape.com/";
        private static string savePath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            Console.WriteLine("Scrap, scrap, get some HTML snack...");

            WebScraper w = WebScraper.Instance.                         // At this point, the save directory for web page is set
                                                                        // and also the web address to scrape the data from.
                SaveDirectory(AppDomain.CurrentDomain.BaseDirectory).   // Set directory to Save scraped data to
                WebAddress("www.neobstaja.si").                         // set custom web address to traverse and scrape
                WebAddress().                                           // reset to the default hardcoded WebAddress
                Scrape();                                               // start scraping.
            
            Console.ReadKey();
        }
    }
}