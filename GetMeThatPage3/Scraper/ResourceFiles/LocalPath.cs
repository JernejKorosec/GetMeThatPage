using GetMeThatPage3.Scraper.ResourceFiles;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    // Todo: Functions to convert from Remote to Local Path
    // Given only baseUri and Remote Relative Path
    //
    public class LocalPath : FilePath
    {
        public override bool EndsWithFileName(string url)
        {
            return !url.EndsWith(@"\");
        }
    }
}