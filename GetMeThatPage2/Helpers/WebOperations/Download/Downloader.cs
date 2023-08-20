using HtmlAgilityPack;
using GetMeThatPage2.Helpers.WebOperations.Url;
using System.Text;
using GetMeThatPage2.Helpers.WebOperations.ResourceFiles;

namespace GetMeThatPage2.Helpers.WebOperations.Download
{
    public class Downloader
    {
        private string RootUrl { get; set; }
        private string appDirectory { get; set; }
        public Downloader(string _appDirectory, string _rootUrl)
        {
            RootUrl = _rootUrl;
            appDirectory = _appDirectory;
        }
        public async Task<List<ResourceFile>> SaveAllResources(List<ResourceFile> resources)
        {
            List<Task<ResourceFile>> resourceTasks = new List<Task<ResourceFile>>();
            foreach (ResourceFile resource in resources)
            {
                Task<ResourceFile> resourceTask = SaveSingleResource(resource);
                resourceTasks.Add(resourceTask);
            }
            ResourceFile[] updatedResourcesArray = await Task.WhenAll(resourceTasks);
            List<ResourceFile> updatedResourcesList = updatedResourcesArray.ToList();
            return updatedResourcesList;
        }
        public async Task<ResourceFile> SaveSingleResource(ResourceFile resourceFile)
        {
            string? absoluteFilePath = resourceFile.AbsoluteFilePath;
            string? directoryPath = Path.GetDirectoryName(absoluteFilePath);
            string? fileRelativeUrl = resourceFile.RelativeFilePath;
            if (!string.IsNullOrWhiteSpace(fileRelativeUrl) && !string.IsNullOrEmpty(directoryPath))
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                if (!File.Exists(absoluteFilePath)) {
                    resourceFile.isSaved = await FileDownloader.DownloadAndSaveFile2(resourceFile.AbsoluteUriFilePath, absoluteFilePath);
                }
                else
                {
                    resourceFile.isSaved = true;
                }
            }
            return resourceFile;
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
    }
}