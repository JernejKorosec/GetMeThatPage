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
        // TODO: A function that convert its own relative path to absolute for fetch
        public bool setAbsoluteFromRelative(string? baseuri = null)
        {
            if (string.IsNullOrEmpty(baseuri))
                if (!string.IsNullOrEmpty(base.RelativePath))
                {
                    if (base.RelativePath.HasSchema())
                    {
                        // FIXME:TODO:CHECK
                        base.AbsolutePath = base.AddIndexHtmlToUrl(base.RelativePath);
                        return true;
                    }
                    else
                    {
                        base.AbsolutePath = base.RelativePath.AddSchema(Schema.Http);
                        base.AbsolutePath = base.AddIndexHtmlToUrl(base.AbsolutePath);
                        return true;
                    }
                }
            return false;
        }
       
        public override bool EndsWithFileName(string url)
        {
            return !url.EndsWith("/");
        }
    }
}