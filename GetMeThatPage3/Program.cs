using GetMeThatPage3.Scraper;
using System.Collections.Concurrent;

namespace GetMeThatPage3
{
    public class ConcurrentDictionaryExample
    {
        private ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();

        public async Task RunExampleAsync()
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

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var example = new ConcurrentDictionaryExample();
            await example.RunExampleAsync();
            //WebScraper spider = new WebScraper("http://books.toscrape.com");
            Console.ReadKey();
        }
    }
}