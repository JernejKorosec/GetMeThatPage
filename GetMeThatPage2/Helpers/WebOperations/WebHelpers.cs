using HtmlAgilityPack;
using System.Text;

namespace GetMeThatPage2.Helpers.WebOperations
{
    public class WebHelpers
    {
        public static async Task<HtmlDocument> DownloadHTMLDocument(string url)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            Encoding utf8Encoding = Encoding.UTF8;
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();
                    string htmlContent = utf8Encoding.GetString(contentBytes);
                    htmlDoc.LoadHtml(htmlContent);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return htmlDoc;
        }

        public static Uri getWebFileAbsolutePath(string rootUrl, String fileRelativeUrl)
        {
            Uri rootUri = new Uri(rootUrl);
            Uri relativeUri = new Uri(fileRelativeUrl, UriKind.Relative);
            return new Uri(rootUri, relativeUri);
        }
    }
    public static class FileDownloader
    {
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
        public static async Task DownloadAndSaveFile(string fileUrl, string filename)
        {
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

                    Console.WriteLine("Downloaded and saved: " + fileUrl);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public static async Task<Boolean> DownloadAndSaveFile2(string fileUrl, string filename)
        {
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

                    Console.WriteLine("Downloaded and saved: " + fileUrl);
                    return true; // Return true indicating success
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false; // Return false indicating failure
            }
        }
    }
}
