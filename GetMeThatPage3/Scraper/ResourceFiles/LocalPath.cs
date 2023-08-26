using GetMeThatPage3.Helpers.Url.Extensions;
using GetMeThatPage3.Scraper.ResourceFiles;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    // Todo: Functions to convert from Remote to Local Path
    // Given only baseUri and Remote Relative Path
    //
    public class LocalPath : FilePath
    {
        private string? WebPageSaveFolder;
        public LocalPath(string appRoot, string webRoot)
        {
            AppRoot = appRoot;
            WebRoot = webRoot;
            WebPageSaveFolder = getLocalSavePathFromWebPage(appRoot,webRoot);
        }
        public bool setLocalAbsoluteFromRelative(string? url = null)
        {
            RelativePath = url;
            if (!string.IsNullOrEmpty(url))
            {
                string? tempUrlPath = RelativePath;
                if (isUriRelative(tempUrlPath))
                {
                    AbsolutePath = Path.Combine(WebPageSaveFolder, tempUrlPath);
                }
                else 
                {
                    if (tempUrlPath.HasSchema())
                        tempUrlPath = RelativePath.RemoveSchema();
                    else
                        tempUrlPath = RelativePath;

                    string? hostName = WebRoot.RemoveSchema();
                    if (!string.IsNullOrEmpty(tempUrlPath))
                    {
                        if (tempUrlPath.StartsWith(hostName))
                        {
                            int indexofstring = tempUrlPath.IndexOf(hostName);
                            string? modified = tempUrlPath.Remove(indexofstring, hostName.Length);
                            if (modified.Length.Equals(0))
                            {
                                AbsolutePath = Path.Combine(AppRoot, AddIndexHtmlToPath(tempUrlPath));
                                return true;
                            }
                        }
                        else
                        {
                            AbsolutePath = Path.Combine(AppRoot, tempUrlPath);
                        }
                    }
                }
                
            }
            return false;

        }
        public override bool EndsWithFileName(string url)
        {
            return !url.EndsWith(@"\");
        }
        public override string? AddIndexHtmlToPath(string filepath)
        {
            // Todo: Check other exceptions, create else if and else if needed
            string? simple = null;
            if (filepath.EndsWith(@"\"))
            {
                int stopmenot = 1;
            }
            if (filepath.EndsWith(@"/"))
            {
                string? dir = Path.GetDirectoryName(filepath);
                simple = Path.Combine(dir,"index.html");
            }
            if (filepath.EndsWith(@"\\"))
            {
                
                int stopmenot = 1;
            }
            return simple;
        }
        public string getLocalSavePathFromWebPage(string? appRoot, string? webRoot)
        {
            return Path.Combine(appRoot, new Uri(webRoot).Host.ToLower());
        }
    }
}