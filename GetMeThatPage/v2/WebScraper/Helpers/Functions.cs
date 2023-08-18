using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage.v2.WebScraper.Helpers
{
    public class Functions
    {
        #region public DataEntry getDataEntryFromRelativeUrl(String path)
        public string createDirectoryFromUrl(string savePath, string url)
        {
            string convertedString = Path.Combine(new Uri(url).Host, new Uri(url).AbsolutePath.TrimStart('/'));
            string newSavePath = Path.Combine(savePath, convertedString);
            Directory.CreateDirectory(newSavePath);
            return newSavePath;
        }
        public String getFileNameFromUri(Uri uri)
        {
            // checks if there is no filename in uri
            string path = uri.LocalPath;
            if (string.IsNullOrWhiteSpace(Path.GetFileName(path)))
                return "index.html";
            return Path.GetFileName(path);
        }
        public String getRelativeRemotePath(Uri uri)
        {
            string path = uri.LocalPath; // Get the local path

            string filename = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(filename))
                path = path.Substring(0, path.Length - filename.Length);

            if (path.StartsWith("/"))
                path = path.Substring(1);

            return path;
        }
        public String getRootLocalPathWithRootPage(string localPath, Uri uri)
        {
            string combinedString = "";
            if (!(string.IsNullOrEmpty(localPath) && string.IsNullOrEmpty(uri.ToString())))
            {
                string hostWithoutSchema = uri.Host;
                combinedString = Path.Combine(localPath, hostWithoutSchema);// + "\\";
            }
            return combinedString;
        }
        public static string RemoveSchemeAndFilename(Uri uri)
        {
            string path = uri.AbsolutePath;
            string filename = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(filename))
                path = path.Substring(0, path.Length - filename.Length);
            return path;
        }
        public String getAbsoluteLocalPathWithoutFilename(string localPath, Uri uri)
        {
            string combinedString = "";
            if (!(string.IsNullOrEmpty(localPath) && string.IsNullOrEmpty(uri.ToString())))
            {
                string relativeUriPath = RemoveSchemeAndFilename(uri);
                string webpageFolder = uri.Host.ToString();
                string relativePath = relativeUriPath.TrimStart('/').Replace('/', '\\');
                combinedString = Path.Combine(localPath, webpageFolder, relativePath);
            }
            return combinedString;
        }
        public string getAbsoluteLocalPath(string localPath, Uri uri, String filename = "index.html")
        {
            String uriHost = uri.Host;
            String temp = getAbsoluteLocalPathWithoutFilename(localPath, uri);
            String temp2 = Path.Combine(temp, filename); ;
            return temp2;
        }
        public string getRelativeLocalPath(Uri fullUri)
        {
            string absolutePath = fullUri.AbsolutePath;
            if (absolutePath.StartsWith("/"))
                absolutePath = absolutePath.Substring(1);

            string filename = System.IO.Path.GetFileName(absolutePath);
            if (!string.IsNullOrEmpty(filename))
                absolutePath = absolutePath.Substring(0, absolutePath.Length - filename.Length);

            if (!string.IsNullOrEmpty(absolutePath))
                return "\\" + absolutePath;
            else
                return "";
        }
        internal ResourceType getResourceType(String filename)
        {
            string ext = Path.GetExtension(filename)?.Substring(1).ToLower();

            bool enumExists = Enum.IsDefined(typeof(ResourceType), ext);
            ResourceType resourceType;
            if (enumExists)
            {
                resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), ext, true);
                return resourceType;
            }
            else
            {
                throw new Exception($"ResourceType Enum doesnt exists !!! you are downloading unpredictab le extension! {ext}");
            }


            return ResourceType.unkown;
        }
        public DataEntry getDataEntryFromUrl(Uri fullUri, String hardcodedSavePath)
        {
            DataEntry entry = new DataEntry
            {
                FileName = getFileNameFromUri(fullUri),
                RelativeLocalPath = getRelativeLocalPath(fullUri),
                AbsoluteLocalPath = getAbsoluteLocalPath(hardcodedSavePath, fullUri, getFileNameFromUri(fullUri)),
                RootLocalPathWithRootPage = getRootLocalPathWithRootPage(hardcodedSavePath, fullUri),
                RootLocalPath = hardcodedSavePath,
                isSaved = false,
                isParsedForResources = false,
                Size = 0,
                Scheme = fullUri.Scheme,
                RelativeWebUri = getRelativeRemotePath(fullUri),
                AbsoluteWebUri = fullUri,
                WebHost = fullUri.Host,
                FileNameOnWeb = getFileNameFromUri(fullUri),
                ResourceType = getResourceType(getFileNameFromUri(fullUri)),
            };
            return entry;
        }
        #endregion

        public String getFileNameFrompath(string path)
        {
            // checks if there is no filename in uri
            if (string.IsNullOrWhiteSpace(Path.GetFileName(path)))
                return "index.html";
            return Path.GetFileName(path);
        }


    }
}
