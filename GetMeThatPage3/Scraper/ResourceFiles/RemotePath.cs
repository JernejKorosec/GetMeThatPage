using GetMeThatPage3.Helpers.Url;
using GetMeThatPage3.Helpers.Url.Extensions;
using GetMeThatPage3.Scraper.ResourceFiles;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    // TODO:
    // URI manipulation functions
    // first Path is always Remote
    public class RemotePath : FilePath
    {
        public RemotePath(string appRoot, string webRoot)
        {
            AppRoot = appRoot;
            WebRoot = webRoot;
        }
        // TODO: A function that convert its own relative path to absolute for fetch
        public bool setRemoteAbsoluteFromRelative(string? url = null)
        {
            RelativePath = url;
            if (!string.IsNullOrEmpty(url))
            {
                string? tempUrlPath = RelativePath;
                if (isUriRelative(tempUrlPath))
                {
                    Uri baseUri = new Uri(WebRoot);
                    Uri relativeUri = new Uri(baseUri, tempUrlPath);
                    AbsolutePath = relativeUri.ToString();
                }
                else
                {
                    // check for schema
                    if (tempUrlPath.HasSchema())
                        tempUrlPath = RelativePath;
                    else
                        tempUrlPath = RelativePath.AddSchema(Schema.Http);
                    //check for filename
                    if (EndsWithFileName(tempUrlPath))
                        AbsolutePath = tempUrlPath;
                    else
                        //AbsolutePath = AddIndexHtmlToPath(tempUrlPath);
                        AbsolutePath = tempUrlPath;
                    return true;
                }

               
            }
            return false;
        }
        public override bool EndsWithFileName(string url)
        {
            return !url.EndsWith("/");
        }

        public override string AddIndexHtmlToPath(string filepath)
        {
            return filepath.EndsWith("/") ? filepath + "index.html" : filepath;

        }
    }
}