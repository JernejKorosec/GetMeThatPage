namespace GetMeThatPage2.Helpers.WebOperations.Css
{
    public class CssUrlResource
    {
        public string? FileName { get; set; }               // ArialBold.wof
        public string? RemoteAbsolutePath { get; set; }     // www.example.com/fonts/
        public string? RemoteRelativePath { get; set; }     // somewhere/
        public string? LocalAbsolutePath { get; set; }      // c:\www.example.com\fonts\
        public string? LocalRelativePath { get; set; }      // www.example.com\fonts\
    }
}