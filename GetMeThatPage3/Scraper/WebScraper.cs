using GetMeThatPage2.Helpers.WebOperations.ResourceFiles;
using System.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

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
        //private ConcurrentDictionary<string, ResourceFile> dictionary = new ConcurrentDictionary<string, ResourceFile>();
        private ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
        public async Task Run(int Pagecount=0)
        {
            string url = "";
            // First run
            if(Pagecount == 0) {
                ResourceFile.WebRoot = _webPageUrl;
                ResourceFile.AppRoot = _appRoot;
            }
            url = CopyResource(url); // Copies url resource and gets next url from dictionary
            

            if (AreAllPagesVisited()) return;
            if (Pagecount>2) return; // Hard return
            Pagecount++;

            await Run(Pagecount);


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

        private string CopyResource(string url)
        {
            //TODO: implementation
            return "";
        }

        private bool AreAllPagesVisited()
        {
            //TODO: implementation
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