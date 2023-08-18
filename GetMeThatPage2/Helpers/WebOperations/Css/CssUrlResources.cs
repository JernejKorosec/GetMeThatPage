using System;

namespace GetMeThatPage2.Helpers.WebOperations.Css
{
    public class CssUrlResource
    {
        public CssUrlResource() { }
        public string LocalRootFolder { get; set; }
        public string RelativelocalPath { get; set; }
        public string FileAbsoluteLocalPath { get; set; }
        public string RenamedFileAbsoluteLocalPath { get; set; }
        public string RemoteFileName { get; set; }
        public string LocalFileName { get; set; }
        public string FileRelativeRemotePath { get; set; }
        public Uri FileAbsoluteRemotePath { get; set; }
        public string WebPageHost { get; set; }

    }
}