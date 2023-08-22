using GetMeThatPage3.Scraper.ResourceFiles;
using GetMeThatPage3.Helpers.State;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    /// <summary>
    /// Properties naming convention
    /// Remote   is Web
    /// Local    is Computer (HDD)
    /// Absolute address from start up to file
    /// Relative address from arbitrary path to file
    /// File     if prepended
    /// Dir     if path is a directory
    /// </summary>
    public class ResourceFile
    {
        private string url;

        public ResourceFile(string url)
        {
            this.url = url;
            State = new State();
        }

        public static string? AppRoot { get; set; }  // c:\some\where\
        public static string? WebRoot { get; set; } // http://books.toscrape.com
        public FilePath? Remote { get; set; }
        public FilePath? Local { get; set; }
        public State State { get; set; }
    }
}