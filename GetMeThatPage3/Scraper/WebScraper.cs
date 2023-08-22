using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage3.Scraper
{
    public class WebScraper
    {
        private readonly string? _webPageUrl;
        public WebScraper(string? webPageUrl)
        {
            _webPageUrl = webPageUrl;
        }
    }
}
