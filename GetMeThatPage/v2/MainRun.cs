using GetMeThatPage.v2.WebScraper.Parser;
using System;
using GetMeThatPage.v2.Url;
using static GetMeThatPage.v2.Url.UrlStringExtensions;

namespace GetMeThatPage.v2
{
    public class MainRun
    {
        private Uri hardcodedWebPageUrl = new("http://books.toscrape.com/");
        private static string hardcodedSavePath = AppDomain.CurrentDomain.BaseDirectory;
        public void mainRun()
        {
            List<DataEntry> dataEntryList = new List<DataEntry>();

            Uri fullUri1 = hardcodedWebPageUrl;
            WebScraperV2.WebScraperV2 webScraperV2 = new WebScraperV2.WebScraperV2(hardcodedWebPageUrl, hardcodedSavePath);

            DataEntry entry1 = webScraperV2.getDataEntryFromUrl(fullUri1);

            // Create a string array containing the URLs
            string[] urls = {
            "/wrgerg/ergerg/ergegr/",
            "/index.html",
            "index.html",
            "",
            "wrgerg/ergerg/ergegr/",
            "/wrgerg/ergerg/ergegr",
            "http://books.toscrape.com",
            "http://books.toscrape.com/wrgerg/ergerg/ergegr/index.html",
            "https://books.toscrape.com/wrgerg/ergerg/ergegr/index.html",
            "ftp://books.toscrape.com/wrgerg/ergerg/ergegr/index.html",
            "http://books.toscrape.com/wrgerg/../../ergerg/ergegr/index.html",
            "/",
            };

            String hardcodedString = "http://books.toscrape.com";
            //UrlV2 url = new UrlV2(hardcodedString);

            foreach (string urlString in urls)
            {
                Console.WriteLine(" ****************************************** ");
                if (urlString.HasSchema())
                {
                    Console.WriteLine($"Url: {urlString}");
                    String extractedDomain = urlString.ExtractDomain();
                    if (!string.IsNullOrEmpty(extractedDomain))
                        Console.WriteLine($"Extracted Domain: {extractedDomain}");
                    else
                        Console.WriteLine($"Extracted Domain: No domain present");
                }

            }


            /*
            #region before not to use
            Uri baseUri = new Uri("http://books.toscrape.com");

            //new Uri(new Uri(url).LocalPath).ToString()
            // Iterate over the URLs and perform the web scraping operation
            foreach (string url in urls)
            {
                string relativeUriString = "";
                //Uri relativeUriTemp = new Uri(url); // Look uper array
                if (url.StartsWith("http://"))
                    relativeUriString = url.Substring(7);
                if (url.StartsWith("https://"))
                    relativeUriString = url.Substring(8);

                Uri calculatedUri = new Uri(baseUri, relativeUriString);

                DataEntry entry = webScraperV2.getDataEntryFromRelativeUrl(url);
                Console.WriteLine($"URL: {url}");
                Console.WriteLine($"Properties of the class Uri for {calculatedUri.AbsoluteUri}:");
                Console.WriteLine($"AbsolutePath: {calculatedUri.AbsolutePath}");
                Console.WriteLine($"AbsoluteUri: {calculatedUri.AbsoluteUri}");
                Console.WriteLine($"Authority: {calculatedUri.Authority}");
                Console.WriteLine($"DnsSafeHost: {calculatedUri.DnsSafeHost}");
                Console.WriteLine($"Fragment: {calculatedUri.Fragment}");
                Console.WriteLine($"Host: {calculatedUri.Host}");
                Console.WriteLine($"HostNameType: {calculatedUri.HostNameType}");
                Console.WriteLine($"IdnHost: {calculatedUri.IdnHost}");
                Console.WriteLine($"IsAbsoluteUri: {calculatedUri.IsAbsoluteUri}");
                Console.WriteLine($"IsDefaultPort: {calculatedUri.IsDefaultPort}");
                Console.WriteLine($"IsFile: {calculatedUri.IsFile}");
                Console.WriteLine($"IsLoopback: {calculatedUri.IsLoopback}");
                Console.WriteLine($"IsUnc: {calculatedUri.IsUnc}");
                Console.WriteLine($"LocalPath: {calculatedUri.LocalPath}");
                Console.WriteLine($"OriginalString: {calculatedUri.OriginalString}");
                Console.WriteLine($"PathAndQuery: {calculatedUri.PathAndQuery}");
                Console.WriteLine($"Port: {calculatedUri.Port}");
                Console.WriteLine($"Query: {calculatedUri.Query}");
                Console.WriteLine($"Scheme: {calculatedUri.Scheme}");
                Console.WriteLine($"Segments: {string.Join(", ", calculatedUri.Segments)}");
                Console.WriteLine($"UserEscaped: {calculatedUri.UserEscaped}");
                Console.WriteLine($"UserInfo: {calculatedUri.UserInfo}");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("");
            }
            #endregion
            */
            //dataEntryList.Add(entry1);

            //dataEntryList = doEntries(dataEntryList);

            System.ConsoleKeyInfo c = Console.ReadKey();
        }




        public List<DataEntry> doEntries(List<DataEntry> entries)
        {
            List<DataEntry> newEntries = new List<DataEntry>();
            if (entries.Count == 0)
                return newEntries;
            else if (entries.Count == 1)
            {
                newEntries = EntryParser.ParseAndMergeEntries(entries);
                if (newEntries.Count == 1) return newEntries;
                else if (newEntries.Count > 1) return doEntries(newEntries);
            }
            else if (entries.Count > 1)
            {
                newEntries = EntryParser.ParseAndMergeEntries(entries);
                if (newEntries.All(e => e.isParsedForResources == true && e.isSaved == true))
                    return newEntries;
                else
                    return doEntries(newEntries);
            }
            return newEntries;
        }
    }
}


