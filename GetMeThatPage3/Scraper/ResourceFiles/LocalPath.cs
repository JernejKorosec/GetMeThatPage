using GetMeThatPage3.Helpers.Url.Extensions;
using GetMeThatPage3.Scraper.ResourceFiles;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    // Todo: Functions to convert from Remote to Local Path
    // Given only baseUri and Remote Relative Path
    //
    public class LocalPath : FilePath
    {
        public LocalPath(string appRoot, string webRoot)
        {
            AppRoot = appRoot;
            WebRoot = webRoot;
        }
        public bool setLocalAbsoluteFromRelative(string? url = null)
        {
            RelativePath = url;
            if (!string.IsNullOrEmpty(url))
            {
                string? tempUrlPath = RelativePath;
                string? savepath = getLocalSavePathFromWebPage(AppRoot, WebRoot);

                // check for schema
                if (tempUrlPath.HasSchema())
                    tempUrlPath = RelativePath.RemoveSchema();
                else
                    tempUrlPath = RelativePath;


                // ce vsebuje books.toscrape.com moramo ta text umakniti ker je že v root poti

                string? hostName = WebRoot.RemoveSchema();
                if (!string.IsNullOrEmpty(tempUrlPath)) { 
                    if (tempUrlPath.StartsWith(hostName))
                    {
                        int indexofstring = tempUrlPath.IndexOf(hostName);
                        //string? modified = tempUrlPath.Remove(indexofstring, hostName.Length).Trim();
                        string? modified = tempUrlPath.Remove(indexofstring, hostName.Length);
                        if (modified.Length.Equals(0))
                        {
                            AbsolutePath = Path.Combine(AppRoot, AddIndexHtmlToPath(tempUrlPath));
                            return true;
                        }
                    }
                    else
                    {
                        // ce vsebuje le ime datoteke, kar pripnemo approotpath-u 
                        // ce vsebuje relativno pot in ime datoteke kar pripnemo approotpath-u 
                        // FIXME: Probably CSS, TODO: Later
                            AbsolutePath = Path.Combine(AppRoot, tempUrlPath);
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