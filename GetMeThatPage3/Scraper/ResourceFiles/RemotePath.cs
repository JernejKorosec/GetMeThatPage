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
            {
                if (!string.IsNullOrEmpty(base.RelativePath))
                {

                    // is relative isMiddleRelative (urlPartRelative)
                    // example
                    // https://www.24ur.com/novice/tujina/slovenski-tanki-v-ukrajini-ustavili-napredovanje-ruske-vojske.html
                    // - https://www.24ur.com/                                                      // schema://host
                    // - novice/tujina                                                              // midleReltivepath
                    // - /slovenski-tanki-v-ukrajini-ustavili-napredovanje-ruske-vojske.html        // endRelativeWithFilename
                    if (base.RelativePath.HasSchema())
                    {
                        // FIXME:TODO:CHECK
                        base.AbsolutePath = base.AddIndexHtmlToPath(base.RelativePath);

                        //return true;
                    }
                    else
                    {
                        base.AbsolutePath = base.RelativePath.AddSchema(Schema.Http);
                        base.AbsolutePath = base.AddIndexHtmlToPath(base.AbsolutePath);
                        //return true;
                    }

                    if (EndsWithFileName(base.AbsolutePath))
                    {
                        //string fileName = Path.GetFileName(new Uri(uriString).AbsolutePath);

                    }
                }
            }
            else
            {
                // TODO: Whole lotta work later
            }
            return false;
        }

        public override bool EndsWithFileName(string url)
        {
            return !url.EndsWith("/");
        }


    }
}