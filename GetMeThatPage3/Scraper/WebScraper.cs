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
        private ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
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

        private async Task executeRunAsync()
        {
            // Create a pool of 3 threads for adding to the ConcurrentDictionary asynchronously
            Task[]? addTasks = new Task[3];
            for (int i = 0; i < addTasks.Length; i++)
            {
                int copyOfI = i; // Capture a local copy of i
                addTasks[i] = Task.Run(() => AddToDictionaryAsync($"Key{copyOfI}", $"Value{copyOfI}"));
            }

            // Wait for all add tasks to complete
            await Task.WhenAll(addTasks);

            // Create 2 threads for reading from the ConcurrentDictionary
            Task[]? readTasks = new Task[2];
            for (int i = 0; i < readTasks.Length; i++)
            {
                int copyOfI = i; // Capture a local copy of i
                readTasks[i] = Task.Run(() => ReadFromDictionary($"Key{copyOfI}"));
            }
            // Wait for all read tasks to complete
            await Task.WhenAll(readTasks);
        }
        private string GetNextResource(string? url)
        {
            string? nextUrl="";

            // TODO: Implementation
            return nextUrl;
        }
        private WebScraper SaveResource(string url)
        {
            ResourceFile resource = new ResourceFile(url);

            // Try to get Resource from dictionary
            if (resources.TryGetValue(url, out var value))
            {
                if (!resource.State.IsSaved)
                {
                    Console.WriteLine($"Resource exists, not saved, saving: {url} - {value}");
                    // Call function to download, and save
                    // Sets the proper flags
                }
                else
                {
                    Console.WriteLine($"Resource exists, saved, returning: {url} - {value}");
                    return this;
                }
            }
            else
            {
                // If not found create new resource
                // Copy Resource
                // Save it to Dictionary as Done
                resources.TryAdd(url, new ResourceFile(url));
                Console.WriteLine($"Resource not found, saving, adding to resources, returning: {url}");
            }
            //TODO: implementation
            return this;
        }
        private bool AreAllPagesVisited(string url)
        {

            if (string.IsNullOrEmpty(url)) return true;
            return false;
        }
        private async Task AddToDictionaryAsync(string key, string value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100)); // Simulating asynchronous work
            dictionary.TryAdd(key, value);
            Console.WriteLine($"Added: {key} - {value}");
        }
        private void ReadFromDictionary(string key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                Console.WriteLine($"Read: {key} - {value}");
            }
            else
            {
                Console.WriteLine($"Key not found: {key}");
            }
        }
    }
}