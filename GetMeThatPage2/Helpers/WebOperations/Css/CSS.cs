using GetMeThatPage2.Helpers.WebOperations.Url;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GetMeThatPage2.Helpers.WebOperations.WebHelpers;

namespace GetMeThatPage2.Helpers.WebOperations.Css
{
    public class CSS
    {
        public CSS() { }
        public string LocalRootFolder { get; set; }
        public string RelativelocalPath { get; set; }
        public string FileRelativeRemotePath { get; set; }
        public string AbsoluteLocalPath { get; set; }
        public string FileAbsoluteRemotePath { get; set; }
        public string WebPageHost { get; set; }
        public string FileName { get; set; }
        public List<CssUrlResource> urlResources { get; set; }
        public static List<CSS> GetCSSFiles(List<string> linksUrls, string urlRoot, string webPageRootFolder)
        {
            List<CSS> CSSList = new List<CSS>();
            foreach (String cssRelativePath in linksUrls)
            {
                CSS css = new CSS();
                css.RelativelocalPath = cssRelativePath;
                css.LocalRootFolder = webPageRootFolder;
                //css.AbsoluteLocalPath = Path.Combine(webPageRootFolder, cssRelativePath); //FIXME: Check
                css.AbsoluteLocalPath = Path.Combine(webPageRootFolder, urlRoot.RemoveSchema(), cssRelativePath);
                css.FileName = Path.GetFileName(cssRelativePath);
                css.WebPageHost = urlRoot;
                if (File.Exists(css.AbsoluteLocalPath)) GetURLPathsFromCSS(css);
                CSSList.Add(css);
            }
            return CSSList;
        }
        public static void GetURLPathsFromCSS(CSS css)
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

                        String temp1 = Path.Combine(localStart, css.WebPageHost.RemoveSchema(), middlePath);
                        String temp2 = cssUrlResource.FileRelativeRemotePath;  // "../fonts/glyphicons-halflings-regular.eot"
                        String temp3 = Path.Combine(temp1, temp2);

                        cssUrlResource.FileAbsoluteLocalPath = Path.GetFullPath(temp3);
                        css.urlResources.Add(cssUrlResource);

                        // Create directories if they dont exist
                        if (!Directory.Exists(cssUrlResource.FileAbsoluteLocalPath))
                        {
                            String? dirToCreate = Path.GetDirectoryName(cssUrlResource.FileAbsoluteLocalPath);
                            if (dirToCreate != null)
                                if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
                        }
                        cssUrlResource.RenamedFileAbsoluteLocalPath = NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath);
                        if (!File.Exists(cssUrlResource.FileAbsoluteLocalPath))
                            DownloadAndSaveFile(cssUrlResource.FileAbsoluteRemotePath.ToString(), NormalizeTheFileName(cssUrlResource.FileAbsoluteLocalPath)).Wait();
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
        public static async Task RenameCSSResources(List<CSS> CSSFiles)
        {
            foreach (CSS css in CSSFiles)
            {
                String cssFilePath = css.AbsoluteLocalPath;
                string cssContent = "";
                try
                {
                    cssContent = File.ReadAllText(cssFilePath); //FIXME: Crashes
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message );
                }
                //if (css.urlResources.Count > 0) Console.WriteLine($"cssFilePath:{cssFilePath}, countCSSResaource:{css.urlResources.Count}");
                foreach (CssUrlResource urlResource in css.urlResources)
                {
                    String oldAString = urlResource.RemoteFileName;
                    String newString = Path.GetFileName(urlResource.RenamedFileAbsoluteLocalPath);
                    if ((!string.IsNullOrEmpty(oldAString) && !string.IsNullOrEmpty(newString)))
                        cssContent = cssContent.Replace(oldAString, newString);
                }
                File.WriteAllText(cssFilePath, cssContent);
            }
            await Task.CompletedTask;
        }
    }
    public static class CSSExtensions
    {
        public static bool IsCss(this HtmlNode node)
        {
            if (node == null || node.NodeType != HtmlNodeType.Element)
                return false;
            string nodeName = node.Name.ToLowerInvariant();
            if (nodeName.ToLowerInvariant().Equals("link"))
            {
                HtmlAttribute htmlAttr = node.Attributes["href"];
                if (htmlAttr != null)
                    if (!htmlAttr.Value.HasSchema())
                        return RelativePathContainsCSSFile(htmlAttr.Value);
            }
            return false;
        }
        // Optimization: proper would be to check  rel="stylesheet" type="text/css"
        public static bool RelativePathContainsCSSFile(string path)
        {
            return Path.GetExtension(path).ToLower().Equals(".css");
        }
    }
}