using HtmlAgilityPack;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;
using static GetMeThatPage2.Helpers.FileSystemOperations.FileSystemHelpers;

namespace GetMeThatPage2.Helpers.WebOperations.Download
{
    public class Downloader
    {
        private string rootUrl { get; set; }
        private string appDirectory { get; set; }

        public Downloader(string _appDirectory, string _rootUrl)
        {
            rootUrl = _rootUrl;
            appDirectory = _appDirectory;
        }

        public async Task saveHTMLDocumentImages(IEnumerable<HtmlNode> imageNodes)
        {
            foreach (var imageNode in imageNodes)
            {
                string fileRelativeUrl = imageNode.GetAttributeValue("src", "");

                Uri combinedUri = getWebFileAbsolutePath(rootUrl, fileRelativeUrl);

                string absoluteFilePath = getFileAbsolutePath(appDirectory, rootUrl, fileRelativeUrl);

                string? directoryPath = Path.GetDirectoryName(absoluteFilePath);

                if (!string.IsNullOrWhiteSpace(fileRelativeUrl) && !string.IsNullOrEmpty(directoryPath))
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                        Console.WriteLine("Directory created: " + directoryPath);
                    }
                    DownloadAndSaveFile(combinedUri.AbsoluteUri, absoluteFilePath).Wait();
                }
            }
        }

        public async Task saveHTMLDocumentImagesAsync(IEnumerable<HtmlNode> imageNodes)
        {


        }
    }
}
