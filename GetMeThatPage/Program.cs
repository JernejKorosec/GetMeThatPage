﻿using GetMeThatPage.Parser.Web;

namespace GetMeThatPage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scrap, scrap, get some HTML snack...");
            WebScraper.Instance.Scrape();
            Console.ReadKey();
        }
    }
}