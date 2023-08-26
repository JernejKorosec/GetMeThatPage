using GetMeThatPage3.Helpers.States;


namespace GetMeThatPage3.Helpers.WebOperations.ResourceFiles
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
        // There can be multiple, relative paths
        // If your opening an existing CSS http://books.toscrape.com/oscar/static/css/style.css
        // you allready heve path but the file containts other relative paths, that are relative
        // to the position of css file
        // public ResourceFile(params string[] url)
        public ResourceFile(string url)
        {
            State = new State();
            Local = new LocalPath(AppRoot, WebRoot);
            Remote = new RemotePath(AppRoot, WebRoot);
            if (!string.IsNullOrEmpty(url)) { 
                Remote.setRemoteAbsoluteFromRelative(url);
                Local.setLocalAbsoluteFromRelative(url);
            }
        }
        public static string? AppRoot { get; set; }  // c:\some\where\
        public static string? WebRoot { get; set; } // http://books.toscrape.com
        public RemotePath Remote { get; set; }
        public LocalPath Local { get; set; }
        public State State { get; set; }
        public string? RelativePath { get; }
    }
}