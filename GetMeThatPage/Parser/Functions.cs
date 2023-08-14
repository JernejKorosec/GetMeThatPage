using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GetMeThatPage.Parser.Resources;
using System.Web;

namespace GetMeThatPage.Parser
{
    public static class Functions
    {
        static readonly HttpClient client = new HttpClient();
        public static async Task CopyWebPageDataToDirectories(string url, string savePath)
        {
            Uri absoluteUri = new Uri(url);
            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8 // Use the appropriate encoding
            };
            HtmlDocument? doc = web.Load(url);


            // Creates directories if they dont exists
            String convertedString = Path.Combine(new Uri(url).Host, new Uri(url).AbsolutePath.TrimStart('/'));
            savePath = Path.Combine(savePath, convertedString);
            Directory.CreateDirectory(savePath);

            // Saves current web page to appropriate directory
            String fileName = Path.Combine(savePath, Path.GetFileName(absoluteUri.LocalPath));
            if (absoluteUri.LocalPath.StartsWith("/"))
            {
                // Saving first WebPage or other
                fileName = Path.Combine(fileName, "index.html");
                doc.Save(fileName);
            }
            else
            {   // Saving subpages
                int stopmenot123 = 1;
            }

            // Gets all local and saveable resources (links) contained in a page from different attributes
            var linksUrls = new Dictionary<string, List<string>>
            {
                { "img", new List<string>() },
                { "script", new List<string>() },
                { "link", new List<string>() },
                { "a", new List<string>() },
                { "css", new List<string>() }
            };
            HtmlNodeCollection allNodes = doc.DocumentNode.SelectNodes("//img[@src] | //script[@src] | //link[@href] | //a[@href]");
            foreach (var node in allNodes)
            {
                var srcAttribute = node.Attributes["src"];
                var hrefAttribute = node.Attributes["href"];
                if (srcAttribute != null)
                {
                    if (!srcAttribute.Value.StartsWith("http://") && !srcAttribute.Value.StartsWith("https://"))
                    {
                        linksUrls[node.Name.ToLower()].Add(srcAttribute.Value);
                    }
                }
                else if (hrefAttribute != null)
                {
                    if (!hrefAttribute.Value.StartsWith("http://") && !hrefAttribute.Value.StartsWith("https://"))
                    {
                        linksUrls[node.Name.ToLower()].Add(hrefAttribute.Value);
                        if (hrefAttribute.Value.ToString().ToLower().Contains("css"))
                            linksUrls["css"].Add(hrefAttribute.Value);
                    }
                }
            }
            try
            {
                foreach (KeyValuePair<String, List<String>> kvp in linksUrls)
                {
                    List<String> resourcesList = kvp.Value;
                    if (kvp.Key.ToLower() != "css")
                        foreach (string resourceUrl in resourcesList)
                        {
                            String? localResourcePath = Path.Combine(savePath, resourceUrl);
                            String webPathToSave = Path.Combine(url, resourceUrl);
                            if (!string.IsNullOrEmpty(localResourcePath))
                            {
                                String? directoryPath = Path.GetDirectoryName(localResourcePath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    if (directoryPath != null)
                                        Directory.CreateDirectory(directoryPath);
                                }
                                // wait the task to finish, update for http 2.0 later
                                if (!File.Exists(localResourcePath))
                                    DownloadAndSaveFiles(webPathToSave, localResourcePath).Wait();
                            }
                        }
                }
            }
            catch (Exception ex) { }

            List<CSS> CSSList = new List<CSS>();
            foreach (String cssRelativePath in linksUrls["css"])
            {
                CSS css = new CSS();
                css.RelativelocalPath = cssRelativePath;
                css.LocalRootFolder = savePath;
                css.AbsoluteLocalPath = Path.Combine(savePath, cssRelativePath);
                css.FileName = Path.GetFileName(cssRelativePath);
                css.WebPageHost = url;
                if (File.Exists(css.AbsoluteLocalPath)) GetURLPathsFromCSS(css);
                CSSList.Add(css);
            }

            foreach (CSS css in CSSList)
            {
                String cssFilePath = css.AbsoluteLocalPath;
                string cssContent = File.ReadAllText(cssFilePath);

                int countCSSResaource = css.urlResources.Count;

                if (countCSSResaource > 0)
                {
                    Console.WriteLine($"cssFilePath:{cssFilePath}, countCSSResaource:{countCSSResaource}");
                }

                foreach (CssUrlResource urlResource in css.urlResources)
                {
                    String oldAString = urlResource.RemoteFileName;
                    String newString = Path.GetFileName(urlResource.RenamedFileAbsoluteLocalPath);

                    if ((!string.IsNullOrEmpty(oldAString) && !string.IsNullOrEmpty(newString)))
                    {
                        cssContent = cssContent.Replace(oldAString, newString);
                    }
                }
                File.WriteAllText(cssFilePath, cssContent);
            }
            //Console.ReadKey();
            await Task.CompletedTask;
        }
        private static void GetURLPathsFromCSS(CSS css)
        {
            css.urlResources = new List<CssUrlResource>();
            try
            {
                string cssContent = File.ReadAllText(css.AbsoluteLocalPath);
                string pattern = @"url\((['""]?)(?!https?://)(?!data:)([^)]+)\1\)";
                MatchCollection matches = Regex.Matches(cssContent, pattern);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 3)
                    {
                        // Setting Remote Resource Location
                        CssUrlResource cssUrlResource = new CssUrlResource();
                        cssUrlResource.FileRelativeRemotePath = match.Groups[2].Value;
                        cssUrlResource.RemoteFileName = Path.GetFileName(match.Groups[2].Value);
                        String remoteMiddlePath = Path.GetDirectoryName(css.RelativelocalPath);
                        Uri uri = new Uri(Path.Combine(css.WebPageHost, remoteMiddlePath, cssUrlResource.FileRelativeRemotePath));
                        cssUrlResource.FileAbsoluteRemotePath = uri;

                        // Setting Local Resource Location
                        String localStart = css.LocalRootFolder;
                        String middlePath = Path.GetDirectoryName(css.RelativelocalPath);
                        String temp1 = Path.Combine(localStart, middlePath);
                        String temp2 = cssUrlResource.FileRelativeRemotePath;  // "../fonts/glyphicons-halflings-regular.eot"
                        String temp3 = Path.Combine(temp1, temp2);
                        cssUrlResource.FileAbsoluteLocalPath = Path.GetFullPath(temp3);
                        css.urlResources.Add(cssUrlResource);

                        // Create directories if they dont exist
                        if (!Directory.Exists(cssUrlResource.FileAbsoluteLocalPath))
                        {
                            String? dirToCreate = Path.GetDirectoryName(cssUrlResource.FileAbsoluteLocalPath);
                            if (dirToCreate != null)
                            {
                                if (!Directory.Exists(dirToCreate))
                                    Directory.CreateDirectory(dirToCreate);
                            }
                        }
                        cssUrlResource.RenamedFileAbsoluteLocalPath = NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                        if (!File.Exists(cssUrlResource.FileAbsoluteLocalPath))
                            DownloadAndSaveFiles(cssUrlResource.FileAbsoluteRemotePath.ToString(), NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath)).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static String NormalizeTheFileName(String filename)
        {
            int percentIndex = filename.IndexOf('%');
            if (percentIndex != -1)
                return filename.Substring(0, percentIndex);
            else
                return filename;
        }
        public static Task CopyCSSFileURLSLocally(string url, string savePath)
        {
            Uri absoluteUri = new Uri(url);
            String convertedString = Path.Combine(new Uri(url).Host, new Uri(url).AbsolutePath.TrimStart('/'));
            savePath = Path.Combine(savePath, convertedString);
            List<string> cssFiles = new List<string>();
            foreach (string file in Directory.EnumerateFiles(savePath, "*.css", SearchOption.AllDirectories))
            {
                cssFiles.Add(file);
            }
            return Task.CompletedTask;
        }
        static async Task DownloadAndSaveFiles(string imageUrl, String filename)
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(imageUrl))
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