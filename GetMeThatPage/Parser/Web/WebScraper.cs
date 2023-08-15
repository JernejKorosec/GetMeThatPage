using HtmlAgilityPack;

namespace GetMeThatPage.Parser.Web
{
    public class WebScraper
    {
        private const String hardcodedWebPageUrl = "http://books.toscrape.com/";
        private static string hardcodedSavePath = AppDomain.CurrentDomain.BaseDirectory;
        private string savePath;
        private string webPageUrl;
        private static WebScraper? _instance;
        public string SavePath
        {
            get { return savePath; }
        }
        public string WebPageUrl
        {
            get { return webPageUrl; }
        }
        public static WebScraper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WebScraper();
                return _instance;
            }
        }
        public WebScraper () {
            webPageUrl = hardcodedWebPageUrl;
            savePath = hardcodedSavePath;
        }
        public WebScraper WebAddress(string webAddr = WebScraper.hardcodedWebPageUrl)
        {
            webPageUrl = webAddr;
            return this;
        }
        internal WebScraper SaveDirectory(string baseDirectory)
        {
            savePath = baseDirectory;
            return Instance;
        }
        internal WebScraper Scrape()
        {
            //Functions.CopyWebPageDataToDirectories(webPageUrl, savePath).Wait();
            Functions.CopyWebPageDataToDirectories(this).Wait();
            return Instance;
        }
    }
}