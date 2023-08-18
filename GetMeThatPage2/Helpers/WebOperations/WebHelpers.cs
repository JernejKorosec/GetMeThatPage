using HtmlAgilityPack;
using System.Text;

namespace GetMeThatPage2.Helpers.WebOperations
{
    public class WebHelpers
    {
        public static async Task<HtmlDocument> getHTMLDocument(string url)
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
        public static async Task DownloadAndSaveFile(string fileUrl, String filename)
        {
            try
            {
                using (HttpResponseMessage response = await new HttpClient().GetAsync(fileUrl))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), 
                        fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                    Console.WriteLine("Downloaded and saved: " + fileUrl);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            await Task.CompletedTask;
        }
        public static Uri getWebFileAbsolutePath(string rootUrl, String fileRelativeUrl)
        {
            Uri rootUri = new Uri(rootUrl);
            Uri relativeUri = new Uri(fileRelativeUrl, UriKind.Relative);
            return new Uri(rootUri, relativeUri);
        }
    }
}
