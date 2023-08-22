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
            if (resources.TryGetValue(resourceFile.Url, out ResourceFile? savedResource))
            {
                if (savedResource.State.IsSaved)
                    return true;
                else
                {
                    savedResource = DownLoadAndSave(savedResource);
                    //TODO: Check if resource in dictionaty changes(Look Line up)
                    return true; ; //FIXME: When TODO done
                }
            }
            else
            {
                //TODO: Download Resource
                //TODO: Save Resource
                //TODO: Set proper boolean values
                //TODO: Replace The ResourceFile in dictionary
                if(resources.TryAdd(resourceFile.Url, resourceFile))
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
            //TODO: Check if it is saved, but first if exists on disk
            //TODO: Download Resource
            //TODO: Save Resource
            //TODO: Set proper boolean values
            savedResource.State.IsSaved = true;
            return savedResource;
        }
    }
}