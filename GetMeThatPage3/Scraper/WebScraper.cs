using System.Collections.Concurrent;
using GetMeThatPage2.Helpers.WebOperations.ResourceFiles;
using GetMeThatPage3.Helpers.Html.Extensions;
using GetMeThatPage3.Scraper.Downloader;
using HtmlAgilityPack;

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
        
        public async Task Run(int Pagecount=0)
        {
            string? url = "";
            // First run
            if(Pagecount == 0) {
                ResourceFile.WebRoot = _webPageUrl;
                ResourceFile.AppRoot = _appRoot;
                url = ResourceFile.WebRoot;
            }
            bool allDone = SaveResource(GetNextResource(url));
            if (allDone) return;
            if (Pagecount>2) return; // Hard return
            else Pagecount++;

            await Run(Pagecount);
        }
        private ResourceFile GetNextResource(string? url)
        {
            ResourceFile resource;
            // Try to get Resource from dictionary
            if (resources.TryGetValue(url, out ResourceFile? savedResource))
            {
                
                if (!savedResource.State.IsSaved)
                {
                    // TODO: Implementation
                    Console.WriteLine($"Resource exists, not saved, saving: {url} - {savedResource.State}");
                    return savedResource;
            
                }
                else
                {
                    Console.WriteLine($"Resource exists, saved, returning: {url} - {savedResource.State}");
                    resource = new ResourceFile(url);
                    return resource;
                }
            }
            else
            {
                Console.WriteLine($"Resource not found, saving, adding to resources, returning: {url}");
                // Obivously, first run of the recursive iteration
                resource = new ResourceFile(url);
                if(resources.TryAdd(url, resource))
                    return resource;
                else
                    throw new Exception();
            }
        }
        private bool SaveResource(ResourceFile resourceFile)
        {
            // Try to get Resource from dictionary
            // TODO: Check all the null values in code
            if (resources.TryGetValue(resourceFile?.Remote?.RelativePath, out ResourceFile? savedResource))
            {
                if (savedResource.State.IsSaved)
                    return true;
                else
                {
                    savedResource = DownLoadAndSave(savedResource);
                    //DOES: It Changes
                    ParseForLinksFromFile(resources,savedResource).Wait();
                    return true; ; //DONE
                }
            }
            else
            {
                //TODO: Download Resource
                //TODO: Save Resource
                //TODO: Set proper boolean values
                //TODO: Replace The ResourceFile in dictionary
                if(resources.TryAdd(resourceFile.Remote.RelativePath, resourceFile))
                {
                    // Added successfully
                    return true;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private ResourceFile DownLoadAndSave(ResourceFile savedResource)
        {
            //Check if it is saved or first if exists on disk
            if (savedResource.State.IsSaved || File.Exists(savedResource.Local.AbsolutePath))
                return savedResource;
            else
            {
                // Create directory if it doesnt exists
                string? fileDir = Path.GetDirectoryName(savedResource.Local.AbsolutePath);
                if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                // FIXME: Check if possible making async or is it for current application inheritance layer state not neccesary
                //DOES: Download Resource
                //DOES: Save Resource
                FileDownloader.DownloadAndSaveFile(savedResource.Remote.AbsolutePath, savedResource.Local.AbsolutePath).Wait();
                //DOES: Set proper boolean values
                savedResource.State.IsSaved = true;
            }
            return savedResource;
        }

        // Todo, can be 2 functions
        // It would be wise to pase as the file is open
        // Todo: Solid: Move To ResourceFile Class
        public static Task ParseForLinksFromFile(ConcurrentDictionary<string, ResourceFile> resources, ResourceFile resourceFile)
        {
            if (resources.TryGetValue(resourceFile?.Remote?.RelativePath, out ResourceFile? savedResource))
            {
                if (savedResource.State.IsSaved || File.Exists(savedResource.Local.AbsolutePath))
                {
                    // Load the HTML file
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(savedResource.Local.AbsolutePath);

                    // Select source nodes using an XPath query
                    string sourceXPath = "//img[@src] | //script[@src] | //link[@href] | //a[@href]";
                    HtmlNodeCollection sourceNodes = doc.DocumentNode.SelectNodes(sourceXPath);

                    if (sourceNodes != null)
                    {
                        Console.WriteLine("Source nodes found:");
                        foreach (HtmlNode node in sourceNodes)
                        {
                            string? relativePath = node.GetHTMLNodeAttributeValue();
                            ResourceFile newResourceFromLink = new ResourceFile(relativePath);
                            bool successFullAdd = resources.TryAdd(newResourceFromLink.Remote.RelativePath, newResourceFromLink);
                            Console.WriteLine("Source: " + relativePath);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No source nodes found.");
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}