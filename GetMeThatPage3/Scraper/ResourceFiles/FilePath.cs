﻿namespace GetMeThatPage3.Scraper.ResourceFiles
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
        private string? filename;
        private string? extension;
        public string? Filename
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativePath))
                    return Path.GetFileName(RelativePath);
                else return filename;
            }
            set { filename = value; }
        }
        public string? Extension
        {
            get
            {
                if (!string.IsNullOrEmpty(Filename))
                    return Path.GetExtension(Filename);
                else return extension;
            }
            set { extension = value; }
        }
        public string? RelativePath { get; set; }      // oscar/kilo/
        public string? AbsolutePath { get; set; }     // oscar/kilo/index.html
    }
}