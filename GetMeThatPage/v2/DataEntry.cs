namespace GetMeThatPage.v2
{
    public enum ResourceType
    {
        none,
        html,
        picture,
        css,
        javscript,
        unkown
    }
    public class DataEntry
    {
        public string FileName{ get; set; }                     // index.html, index_2.html
        public string RelativeLocalPath { get; set; }           // \\nekje\\na\\spletu
        public string AbsoluteLocalPath { get; set; }           // c:\\nekamapa\\www.bookstoscrape.com\\nekje\\na\\spletu\\index.html
        public string RootLocalPathWithRootPage { get; set; }   // c:\\nekamapa\\www.bookstoscrape.com
        public string RootLocalPath { get; set; }               // c:\\nekamapa\\
        public bool isSaved { get; set; }                       // true, false
        public bool isParsedForResources { get; set; }          // true, false for HTMLs, CSSs and Javascript files
        public long Size { get; set; }                          // 4564564 bytes version 3 will preload sizes or it will 
        public string Scheme { get; set; }                      // http: https:
        public string RelativeWebUri { get; set; }              // /, /nekje/na/spletu, ...
        public Uri AbsoluteWebUri { get; set; }                 // http://www.bookstoscrape.com/nekje/na/spletu/index.html
        public string FileNameOnWeb { get; set; }               // index.html
        public string WebHost { get; internal set; }            // http://www.bookstoscrape.com
        public ResourceType ResourceType { get; set; }
    }
    public class Entries
    {
        //public String LocalRoot { get; set; }               // c:\\nekamapa\\
        //public String WebRootRoot { get; set; }             // http://www.bookstoscrape.com
        //public DateTime StartTime { get; set; }             // 09:26
        //public DateTime EndTime { get; set; }               // 09:26
        public List<DataEntry> entriesList { get; set; }      // Lista vnosov
        public Entries() { entriesList = new List<DataEntry>(); }
    }

}
