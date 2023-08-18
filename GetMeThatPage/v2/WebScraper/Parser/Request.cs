using HtmlAgilityPack;
using System.Net.Http;
using System.Text;

namespace GetMeThatPage.v2.WebScraper.Parser
{
    public class Request
    {
        public static HtmlDocument? SaveHTMLDocument(String fileName, Uri urlRoot)
        {
            HtmlWeb web = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 };
            HtmlDocument? doc = web.Load(urlRoot);
            doc.Save(fileName);
            return doc;
        }
        public static HtmlDocument? LoadHTMLDocument(DataEntry dataEntry)
        {
            HtmlWeb web = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 };
            HtmlDocument? doc = new HtmlDocument();
            if (File.Exists(dataEntry.AbsoluteLocalPath))
            {
                //string htmlContent = File.ReadAllText(dataEntry.AbsoluteLocalPath);
                //doc.Load(htmlContent);
                doc.Load(dataEntry.AbsoluteLocalPath);
            }
            return doc;
        }
        public static async Task DownloadAndSaveFiles(string url, String filename)
        {
            HttpClient client = new HttpClient();
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            await Task.CompletedTask;
        }
    }
}
