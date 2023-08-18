using GetMeThatPage.Parser.Web;
using GetMeThatPage.v2;

namespace GetMeThatPage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //WebScraper.Instance.Scrape();
            MainRun mr = new MainRun();
            mr.mainRun();
        }
    }
}