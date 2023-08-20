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
}