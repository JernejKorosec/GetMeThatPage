using GetMeThatPage.Parser.Web;

namespace GetMeThatPage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scrap, scrap, get some HTML snack...");
            /*
            WebScraper w = WebScraper.Instance.                         // At this point, the save directory for web page is set
                                                                        // and also the web address to scrape the data from.
                SaveDirectory(AppDomain.CurrentDomain.BaseDirectory).   // Set directory to Save scraped data to
                WebAddress("www.neobstaja.si").                         // set custom web address to traverse and scrape
                WebAddress().                                           // reset to the default hardcoded WebAddress
                Scrape();                                               // start scraping using custom or hardcoded values
            */

            // Suffice, using hardcoded values
            WebScraper.Instance.Scrape();
            Console.ReadKey();
        }
    }
}