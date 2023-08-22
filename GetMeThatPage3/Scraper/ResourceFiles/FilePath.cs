using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage3.Scraper.ResourceFiles
{    /// <summary>
     /// Properties naming convention
     /// Remote   is Web
     /// Local    is Computer (HDD)
     /// Absolute address from start up to file
     /// Relative address from arbitrary path to file
     /// File     if prepended
     /// Dir     if path is a directory
     /// </summary>
    public class FilePath
    {
        public string? Filename { get; set; }  // index.html
        public string? RelativePath { get; set; }      // oscar/kilo/
        public string? AbsolutePath { get; set; }     // oscar/kilo/index.html
        public string? getExtension()
        {
            return Path.GetExtension(Filename);
        }
    }
}
