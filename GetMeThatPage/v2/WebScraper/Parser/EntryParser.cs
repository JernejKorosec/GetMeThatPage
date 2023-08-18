using GetMeThatPage.Parser;
using GetMeThatPage.Resources;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage.v2.WebScraper.Parser
{
    public class EntryParser
    {
        

        public static List<DataEntry> ParseAndMergeEntries(List<DataEntry> entries)
        {
            HtmlDocument doc = new HtmlDocument();

            List<DataEntry> newEntries = new List<DataEntry>();
            DataEntry dataEntry = entries.FirstOrDefault(e => e.isParsedForResources == false && e.isSaved == false);
            //DataEntry dataEntry = entries.FirstOrDefault(e => e.isSaved == false);

            if (dataEntry.isSaved == false)
            {
                // Ce Je HTML datoteka
                if (dataEntry.ResourceType.Equals(ResourceType.html))
                {
                    doc = SaveEntry(dataEntry);
                }
                // ce je druga datoteka
                // else if

                dataEntry.isSaved = true;
            }

            if (dataEntry.isParsedForResources == false)
            {
                if (dataEntry.ResourceType.Equals(ResourceType.html))
                {
                    newEntries = ParseEntryLinks(dataEntry, doc);
                    dataEntry.isParsedForResources = true;
                }
                else if (dataEntry.ResourceType.Equals(ResourceType.css))
                {
                    newEntries = ParseEntryLinks(dataEntry, doc);
                    dataEntry.isParsedForResources = true;
                }
                newEntries = ParseEntryLinks(dataEntry, doc);
            }
            newEntries.AddRange(entries);
            return newEntries;
        }
        private static List<DataEntry> ParseEntryLinks(DataEntry dataEntry, HtmlDocument doc)
        {
            // To je lista vseh resourceov znotraj HTML-ja
            // od jpeg, png, ico, css, js in seveda tudi linki na ostale html datoteke
            // To napolnimo tako da odpremo  dataEntry.AbsoluteLocalPath
            // in poiščemo vse linke css je itd...

            List<DataEntry> currEntries = new List<DataEntry>();

            Dictionary<string, List<string>> linksUrls = Interpreter.GetLinksDictionary(doc);  // naredi spisek vsega kar trenutna stran vsebuje, vse datoteke
            Interpreter.SaveHTMLDocumentResources(linksUrls, dataEntry);

            // funkcionalnost za prebiranje CSS-jev (v cssjih pridobimo naslove za ostale datoteke)
            List<CSS> CSSFiles = Interpreter.GetCSSFiles(linksUrls, dataEntry);
            Interpreter.RenameCSSResources(CSSFiles).Wait();
            currEntries = Interpreter.AddLinksFromDocumentToPageList(linksUrls, currEntries,dataEntry);

            return currEntries;

        }
        /** Naloži HTML iz diska oziroma iz Spleta če na disku ne obstaja
        //  DataEntry je struktura ki opisuje vse poti torej na spletu in lokalno da se lahko
        //  bere in shranjuje. Vsebuje je le poti direktorijev. imena datotek. itd.
        **/
        private static HtmlDocument? SaveEntry(DataEntry dataEntry)
        {
            HtmlDocument doc = new HtmlDocument();

            if (File.Exists(dataEntry.AbsoluteLocalPath))
            {
                doc = Request.LoadHTMLDocument(dataEntry);
                return doc;
            }
            else
            {
                string? fileDir = Path.GetDirectoryName(dataEntry.AbsoluteLocalPath);
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                //else
                    doc = Request.SaveHTMLDocument(dataEntry.AbsoluteLocalPath, dataEntry.AbsoluteWebUri);
            }
            return doc;
        }
    }
}