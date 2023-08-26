using System.Collections.Concurrent;
using GetMeThatPage3.Helpers.WebOperations.ResourceFiles;
using GetMeThatPage3.Helpers.Html.Extensions;
using GetMeThatPage3.Helpers.Url.Extensions;
using GetMeThatPage3.Scraper.Downloader;
using HtmlAgilityPack;
using static GetMeThatPage3.Scraper.ResourceFiles.Extensions.ResourceFileExtensions;
using System;


namespace GetMeThatPage3.Scraper
{
    public class WebScraper
    {
        private readonly string? _webPageUrl;
        private readonly string? _appRoot;
        public WebScraper(string? webPageUrl, string? appRoot)
        {
            _webPageUrl = webPageUrl;
            _appRoot = appRoot;
        }

        private ConcurrentDictionary<string, ResourceFile> resources = new ConcurrentDictionary<string, ResourceFile>();

        public async Task Run(int Pagecount = 0)
        {
            string? url = "";
            // First run
            if (Pagecount == 0)
            {
                ResourceFile.WebRoot = _webPageUrl;
                ResourceFile.AppRoot = _appRoot;
                url = ResourceFile.WebRoot;
            }

            bool isPageDone = SaveResource(GetNextResource(url));
            bool unsavedResourcesExists = UnsavedResourcesExists();
            if (isPageDone && !unsavedResourcesExists) return;
            if (Pagecount > 5) return; // Hard return
            else Pagecount++;

            await Run(Pagecount);
        }
        private bool UnsavedResourcesExists()
        {
            bool hasUnsavedResources = resources.Values.Any(resource => !resource.State.IsSaved);
            return hasUnsavedResources;
        }
        private ResourceFile? GetNextResource(string? url)
        {
            ResourceFile? resource;
            // Try to get Resource from dictionary
            if (string.IsNullOrEmpty(url))
            {
                resource = GetNextNotSavedResource();
            }
            else
            {
                // Obivously, first run of the recursive iteration
                resource = new ResourceFile(url);

                if (!url.HasSchema()) // fixme!
                    if (resources.TryAdd(url, resource))
                        return resource;
                    else
                        throw new Exception();
            }
            return resource;
        }
        private ResourceFile? GetNextNotSavedResource()
        {
            IEnumerable<ResourceFile>? unsavedResources = resources.Values.Where(resource => !resource.State.IsSaved);
            return unsavedResources.FirstOrDefault();
        }
        private bool SaveResource(ResourceFile? resourceFile)
        {
            // Obviously, need to sepereate to seperate parsable files (HTMLs, CSS) and other files
            // not parsable files (images, fonts, javascript files)
            Console.WriteLine("Url: " + resourceFile?.Remote?.RelativePath);

            if (resourceFile == null) return false;
            else
            {
                //FIXME remove later or refactor
                if (!resourceFile.Local.AbsolutePath.Contains(@"GetMeThatPage3\bin\Debug\net7.0\books.toscrape.com"))
                {
                    Console.WriteLine("Url: " + resourceFile.Remote.RelativePath);
                    throw new Exception("Trying to save outside of allowed resource!!");
                }

                if (resources.TryGetValue(resourceFile?.Remote?.RelativePath, out ResourceFile? savedResource))
                {
                    if (savedResource.State.IsSaved)
                        return true;
                    else
                    {
                        if (resourceFile.isHTML())
                        {
                            savedResource = DownLoadAndSave(savedResource);
                            ParseForLinksFromFile(resources, savedResource).Wait();
                            return true;
                        }
                        else
                        {
                            savedResource = DownLoadAndSave(savedResource);
                            // TODO: Just download images
                        }
                        
                    }
                }
                else
                {
                    if (resources.TryAdd(resourceFile.Remote.RelativePath, resourceFile))
                        return true;
                    else
                        throw new Exception();
                }

                
            }
            return false;


            
        }
        private ResourceFile DownLoadAndSave(ResourceFile savedResource)
        {
            if (savedResource.State.IsSaved || File.Exists(savedResource.Local.AbsolutePath))
                return savedResource;
            else
            {
                string? fileDir = Path.GetDirectoryName(savedResource.Local.AbsolutePath);
                if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                FileDownloader.DownloadAndSaveFile(savedResource.Remote.AbsolutePath, savedResource.Local.AbsolutePath).Wait();
                savedResource.State.IsSaved = true;
            }
            return savedResource;
        }
        public static Task ParseForLinksFromFile(ConcurrentDictionary<string, ResourceFile> resources, ResourceFile resourceFile)
        {
            if (resources.TryGetValue(resourceFile?.Remote?.RelativePath, out ResourceFile? savedResource))
            {
                if (savedResource.State.IsSaved && File.Exists(savedResource.Local.AbsolutePath))
                //if (savedResource.State.IsSaved || File.Exists(savedResource.Local.AbsolutePath))
                {
                    if (savedResource.isHTML())
                    {
                        // Load the HTML file
                        HtmlDocument doc = new HtmlDocument();
                        doc.Load(savedResource.Local.AbsolutePath);

                        // Select source nodes using an XPath query
                        string sourceXPath = "//img[@src] | //script[@src] | //link[@href] | //a[@href]";
                        HtmlNodeCollection sourceNodes = doc.DocumentNode.SelectNodes(sourceXPath);

                        if (sourceNodes != null)
                        {
                            //Console.WriteLine("Source nodes found:");
                            foreach (HtmlNode node in sourceNodes)
                            {
                                string? relativePath = node.GetHTMLNodeAttributeValue();

                                ResourceFile newResourceFromLink = new ResourceFile(relativePath);
                                //FixMe, adding source and destinatino (ResourceFile and relative uri)
                                //ResourceFile newResourceFromLink = new ResourceFile(resourceFile,relativePath);
                                bool successFullAdd = resources.TryAdd(newResourceFromLink.Remote.RelativePath, newResourceFromLink);
                                //Console.WriteLine("Source: " + relativePath);
                            }
                        }
                        else
                        {
                            //Console.WriteLine("No source nodes found.");
                        }

                    }
                    else
                    {
                        // TODO: Just download the pic and set isSaved to true;
                    }


                }
            }
            return Task.CompletedTask;
        }
    }
}