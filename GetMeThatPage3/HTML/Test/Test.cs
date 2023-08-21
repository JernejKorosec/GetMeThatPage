using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage3.HTML.Test
{
    internal class Test
    {
        // URL of the website to scrape
        public static void test1(string url = "http://books.toscrape.com")
        {
            // Create a new HtmlWeb instance
            var web = new HtmlWeb();

            // Load the HTML content of the webpage
            var doc = web.Load(url);

            // Print the HTML content of the webpage
            HtmlNodeCollection scriptElements = doc.DocumentNode.SelectNodes("//script");
            foreach (HtmlNode scriptElement in scriptElements)
            {
                string sourcePath = scriptElement.GetAttributeValue("src", "");
                // Do something with the source path
            }
            foreach (HtmlNode scriptElement in scriptElements)
            {
                string sourcePath = scriptElement.GetAttributeValue("src", "");
                Console.WriteLine(sourcePath);
                // Or do something else with the source path
            }
        }
    }
}
