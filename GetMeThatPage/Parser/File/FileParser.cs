using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GetMeThatPage.Parser.File
{
    public class FileParser
    {
        public static string createDirectoryFromUrl(string savePath, string url)
        {
            string convertedString = Path.Combine(new Uri(url).Host, new Uri(url).AbsolutePath.TrimStart('/'));
            string newSavePath = Path.Combine(savePath, convertedString);
            Directory.CreateDirectory(newSavePath);
            return newSavePath;
        }
       
    }
}
