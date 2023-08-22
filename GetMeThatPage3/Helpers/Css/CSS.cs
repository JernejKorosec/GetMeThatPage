using GetMeThatPage3.Helpers.State;

namespace GetMeThatPage2.Helpers.WebOperations.Css
{
    /// <summary>
    /// Remote   is Web
    /// Local    is Computer (HDD)
    /// Absolute address from start up to file
    /// Relative address from arbitrary path to file
    /// File     if prepended
    /// </summary>
    public class CSS
    {
        public string? FileName { get; set; }               // index.html
        public string? RemoteAbsolutePath { get; set; }     // www.example.com/somewhere/
        public string? RemoteRelativePath { get; set; }     // somewhere/
        public string? LocalAbsolutePath { get; set; }      // c:\www.example.com\somewhere\
        public string? LocalRelativePath { get; set; }      // www.example.com\somewhere\
        public State? State { get; set; }
        public List<CssUrlResource>? UrlResources { get; set; }
    }
}