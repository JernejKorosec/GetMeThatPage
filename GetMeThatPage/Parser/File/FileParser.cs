using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage.Parser.File
{
    public class FileParser
    {
        public static void createDirectoryFromUrl(String url, String savePath)
        {
            // Creates directories if they dont exists
            String convertedString = Path.Combine(new Uri(url).Host, new Uri(url).AbsolutePath.TrimStart('/'));
            savePath = Path.Combine(savePath, convertedString);
            Directory.CreateDirectory(savePath);
        }
    }
}
