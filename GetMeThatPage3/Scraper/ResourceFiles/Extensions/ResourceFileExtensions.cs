using GetMeThatPage3.Helpers.WebOperations.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage3.Scraper.ResourceFiles.Extensions
{
    public static class ResourceFileExtensions
    {
        public static bool isParsable(this ResourceFile? resourceFile)
        {
            if (resourceFile != null)
            {
                    return resourceFile.isCSS() || resourceFile.isHTML();
            }
            return false;
        }
        /*
        public static bool isHTML(this string filename)
        {
            return filename.ToLower().EndsWith(".html") || filename.ToLower().EndsWith(".htm");
        }
        */
        public static bool isHTML(this ResourceFile? resourceFile)
        {
            if (resourceFile != null)
            {
                string? extension = resourceFile.getResourceExtension();
                if (!string.IsNullOrEmpty(extension))
                    if (extension.Equals(".html") || extension.Equals(".htm"))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static bool isCSS(this ResourceFile? resourceFile)
        {
            if (resourceFile != null)
            {
                string? extension = resourceFile.getResourceExtension();
                if (!string.IsNullOrEmpty(extension))
                    if (extension.Equals(".css"))
                    return true;
                else
                    return false;
            }
            return false;
        }
        /*
        public static bool isCSS(this string filename)
        {
            return filename.ToLower().EndsWith(".css");
        }
        */
        public static string? getResourceExtension(this ResourceFile resourceFile)
        {
            return Path.GetExtension(resourceFile.Local.AbsolutePath);
        }
    }
}