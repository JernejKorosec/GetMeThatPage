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
        public ResourceFile(string url)
        {
            State = new State();
            Local = new LocalPath();
            Remote = new RemotePath();
            Remote.RelativePath = url;

            Remote.setAbsoluteFromRelative();
            int stopfordebugandfood = 1;
        }
        public static string? AppRoot { get; set; }  // c:\some\where\
        public static string? WebRoot { get; set; } // http://books.toscrape.com
        public RemotePath Remote { get; set; }
        public LocalPath Local { get; set; }
        public State State { get; set; }
    }
}