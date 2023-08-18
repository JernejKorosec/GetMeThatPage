using GetMeThatPage2.Helpers.WebOperations.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage2.Helpers.FileSystemOperations
{
    public class FileSystemHelpers
    {
        public static string getFileAbsolutePath(string appDirectory, string rootUrl, String fileRelativeUrl)
        {
            string webPageFolder = rootUrl.RemoveSchema();
            return Path.Combine(appDirectory, webPageFolder, fileRelativeUrl);
        }
    }
}
