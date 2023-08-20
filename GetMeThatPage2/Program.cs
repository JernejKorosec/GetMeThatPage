using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using GetMeThatPage2.Helpers.WebOperations.Download;
using GetMeThatPage2.Helpers.WebOperations.Html;
using System.Resources;
using GetMeThatPage2.Helpers.WebOperations.Url;
using GetMeThatPage2.Helpers.WebOperations;
using System.Text.RegularExpressions;
using GetMeThatPage2.Helpers.WebOperations.Css;
using System;
using System.Collections.Generic;

namespace GetMeThatPage2
{
    internal class Program
    {
        private static string rootUrl = "http://books.toscrape.com/";
        private static string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        static async Task Main(string[] args)
        {
            DateTime startTime = DateTime.Now;                          // Record start time
            List<ResourceFile> resources = new List<ResourceFile>();    // Global Resources List
            ResourceFile.WebRoot = rootUrl;
            ResourceFile.AppRoot = appDirectory;



            // iz rootUrl = https://books.toscrape.com/index.html, dobi vse relativne poti na druge datoteke
            // oziroma iz nekega spletnega HTMLja dobi vse linke in jih shrani v LISTO RESOURCOV
            // ta funkcija NE HRANI same index.html datoteke ali pripadajočih podstrani
            await GetHTMLLinks(rootUrl, resources); // podamo OD KJE(rootUrl) naložimo vse POVEZAVE(resources)

            // Ta funkcija SHRANI vse datoteke iz LIST RESOURCEOV
            Downloader downloader = new Downloader(appDirectory, rootUrl); // Podamo KAM(appDirectory) shranimo in od KJE(rootUrl) s spleta vzamemo
            resources = await downloader.SaveAllResources(resources);

            // 1. Najdi vse datoteke s končnico CSS iz LISTE RESOURCOV
            // 2. Iz vsake ŽE SHRANJENE datoteke CSS preberi:
            //      vse spletne poti(fontov) in jih shrani v LISTO RESOURCOV

            resources = GetCSSLinks(resources);


            TimeSpan executionTime = DateTime.Now - startTime; // Record end time
            Console.WriteLine($"Execution Time: {(int)executionTime.TotalMinutes} minutes, {(int)executionTime.Seconds} seconds, {executionTime.Milliseconds} milliseconds");
            Console.ReadKey();
            // Execution Time: 0 minutes, 1 seconds, 435 milliseconds
        }


        // Zdownloadaj datoteko s spleta in dobi vn vse urlje jih dodaj v LISTO RESOURCOV če že ne obstaja na disku jo tudi shrani.
        public static List<ResourceFile> GetCSSLinks(List<ResourceFile> resources)
        {
            foreach (ResourceFile resource in resources)
            {
                // Če je datoteka CSS
                if (resource.Extension.ToLower().Equals(".css"))
                {
                    // Če je že shranjena so tudi njeni resourci
                    // Če še ni shranjena jo zdownloadaj in shrani njene resource v listo
                    // Spremeni imena fontov
                    // spremeni samo datoteko da vsebuje nova imena fontov
                    // (To mam že sprogramirano)
                    if (resource.isSaved) // Če je css na disku ga lahko odpremo
                    {

                        CSS css = new CSS();
                        css.FileName = resource.Filename;
                        css.AbsoluteLocalPath = resource.AbsoluteFilePath; // absolutna pot datoteke na disku
                        css.RelativelocalPath = resource.RelativePath;      // mapa v kateri se css nahaja, nalaganje fontov bo relativno na to pot
                        css.WebPageHost = UrlStringExtensions.GetBaseUrl(resource.AbsoluteUriPath);

                        List<CSS> CSSFiles = new List<CSS>();

                        if (File.Exists(css.AbsoluteLocalPath))
                        {
                            css.urlResources = new List<CssUrlResource>();
                            string cssContent = File.ReadAllText(css.AbsoluteLocalPath);
                            string pattern = @"url\((['""]?)(?!https?://)(?!data:)([^)]+)\1\)";
                            MatchCollection matches = Regex.Matches(cssContent, pattern);
                            foreach (Match match in matches)
                            {
                                if (match.Groups.Count >= 3)
                                {
                                    // Setting Remote Resource Location
                                    CssUrlResource cssUrlResource = new CssUrlResource();
                                    cssUrlResource.FileRelativeRemotePath = match.Groups[2].Value;
                                    cssUrlResource.RemoteFileName = Path.GetFileName(match.Groups[2].Value);
                                    Uri baseUriObject = new Uri(css.WebPageHost);
                                    Uri combinedUri = new Uri(baseUriObject, css.RelativelocalPath);
                                    Uri modifiedUri = new Uri(combinedUri, combinedUri.AbsolutePath.EndsWith("/") ? combinedUri.AbsolutePath : combinedUri.AbsolutePath + "/");
                                    Uri finalUri = new Uri(modifiedUri, cssUrlResource.FileRelativeRemotePath);
                                    cssUrlResource.FileAbsoluteRemotePath = finalUri;
                                    String path1 = ResourceFile.ScrapeRoot.Replace('/', '\\').TrimEnd('\\');
                                    String path2 = finalUri.AbsolutePath.Replace('/', '\\').TrimStart('\\');
                                    cssUrlResource.FileAbsoluteLocalPath = Path.Combine(path1, path2);
                                    css.urlResources.Add(cssUrlResource);
                                    // Create directories if they dont exist
                                    if (!Directory.Exists(cssUrlResource.FileAbsoluteLocalPath))
                                    {
                                        String? dirToCreate = Path.GetDirectoryName(cssUrlResource.FileAbsoluteLocalPath);
                                        if (dirToCreate != null)
                                            if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
                                    }

                                    cssUrlResource.RenamedFileAbsoluteLocalPath = CSS.NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                                    if (!File.Exists(cssUrlResource.FileAbsoluteLocalPath))
                                    {
                                        String temp1 = cssUrlResource.FileAbsoluteRemotePath.ToString();
                                        String temp2 = CSS.NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                                        FileDownloader.DownloadAndSaveFile(temp1, temp2).Wait();
                                    }
                                }





                            }

                            // Dodati moramo fonte kot shranjen resource
                            // Spremeniti moramo imena fontov
                            // enako moramo spremeniti vsebino CSS datoteke

                            //Spremeni vsebino CSS-a
                            foreach (CssUrlResource urlResource in css.urlResources)
                            {
                                String oldAString = urlResource.RemoteFileName;
                                String newString = Path.GetFileName(urlResource.RenamedFileAbsoluteLocalPath);
                                if ((!string.IsNullOrEmpty(oldAString) && !string.IsNullOrEmpty(newString)))
                                    cssContent = cssContent.Replace(oldAString, newString);
                            }
                            //Shrani spremenjen css
                            File.WriteAllText(css.AbsoluteLocalPath, cssContent);
                        }
                    }
                }
            }
            return resources;
        }


        public static async Task GetHTMLLinks(string rootUrl, List<ResourceFile> resources)
        {
            HtmlDocument document = await DownloadHTMLDocument(rootUrl);
            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            List<HtmlNode> htmlNodeList = allDescendants.ToList();

            List<ResourceFile> newResources = new List<ResourceFile>();
            List<ResourceFile> uniqueResources;
            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory); // Todo 2 konstruktorja za vhodni pogoj ko je lsita resourceov prazna
                if (!htmlNode.GetRelativeUri().HasSchema())
                    newResources.Add(r);
            }
            lock (resources)
            {
                resources.AddRange(newResources);
                uniqueResources = resources.RemoveDuplicateValues();
                resources.Clear();
                resources.AddRange(uniqueResources);
            }

        }
        public static async Task<HtmlNodeCollection> GetAllDescendants(string rootUrl, List<ResourceFile> resources)
        {
            HtmlDocument document = await DownloadHTMLDocument(rootUrl);
            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            List<HtmlNode> htmlNodeList = allDescendants.ToList();
            List<ResourceFile> newResources = new List<ResourceFile>(); // Temporarily hold new resources
            List<ResourceFile> uniqueResources;
            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory);
                if (!htmlNode.GetRelativeUri().HasSchema())
                    newResources.Add(r);
            }
            lock (resources)
            {
                resources.AddRange(newResources);
                uniqueResources = resources.RemoveDuplicateValues();
                resources.Clear();
                resources.AddRange(uniqueResources);
            }
            return allDescendants;
        }
        public static async Task<List<ResourceFile>> LoadHTMLResources(string rootUrl, List<ResourceFile> resources)
        {
            HtmlDocument document = await DownloadHTMLDocument(rootUrl);
            HtmlNodeCollection allDescendants = document.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            List<HtmlNode> htmlNodeList = allDescendants.ToList();
            List<ResourceFile> newResources = new List<ResourceFile>(); // Temporarily hold new resources
            List<ResourceFile> uniqueResources;
            foreach (HtmlNode htmlNode in htmlNodeList)
            {
                ResourceFile r = new ResourceFile(htmlNode, rootUrl, appDirectory);
                newResources.Add(r);
            }
            lock (resources)
            {
                resources.AddRange(newResources);
                uniqueResources = resources.RemoveDuplicateValues();
                resources.Clear();
                resources.AddRange(uniqueResources);
            }
            return resources;
        }

    }
}