using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage.Parser.Resources
{
    public class CSS
    {
        public CSS() { }
        public string LocalRootFolder { get; set; }
        public string RelativelocalPath { get; set; }
        public string FileRelativeRemotePath { get; set; }
        public string AbsoluteLocalPath { get; set; }
        public string FileAbsoluteRemotePath { get; set; }
        public string WebPageHost { get; set; }
        public string FileName { get; set; }
        public List<CssUrlResource> urlResources { get; set; }
        
    }
}