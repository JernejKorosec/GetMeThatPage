using GetMeThatPage2.Helpers.WebOperations.ResourceFiles;
using System.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

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
            SaveResource(GetNextResource(url));  
            //if (AreAllPagesVisited(url)) return;
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
                // Obivously, first run of the recursive iteration
                resource = new ResourceFile(url);
                resources.TryAdd(url, resource);
                Console.WriteLine($"Resource not found, saving, adding to resources, returning: {url}");
            }
            return resource;
        }
        private WebScraper SaveResource(ResourceFile resourceFile)
        {
            //TODO: implementation
            return this;
        }
        private bool AreAllPagesVisited(string url)
        {

            if (string.IsNullOrEmpty(url)) return true;
            return false;
        }
    }
}