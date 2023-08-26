using HtmlAgilityPack;
using System.Text;

namespace GetMeThatPage3.Scraper.Downloader
{
    public static class FileDownloader
    {
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
        public static async Task DownloadAndSaveFile(string fileUrl, string filename)
        {
            //FIXME: Remove hard block when done
            
            if (!filename.Contains(@"GetMeThatPage3\bin\Debug\net7.0\books.toscrape.com"))
            {
                Console.WriteLine("Url: " + fileUrl);
                throw new Exception("Trying to save outside of allowed resource!!");
            }

            try
                {
                using (var response = await new HttpClient().GetAsync(fileUrl))
                {
                    response.EnsureSuccessStatusCode();
                    await _fileLock.WaitAsync(); // Acquire the lock before accessing the file
                    try
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }
                    finally
                    {
                        _fileLock.Release(); // Release the lock after accessing the file
                    }

                    //Console.WriteLine("Downloaded and saved: " + fileUrl);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
