﻿using GetMeThatPage2.Helpers.WebOperations.Url;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage2.Helpers.WebOperations.ResourceFiles
{
    public class ResourceFile
    {
        public static string? AppRoot { get; set; }
        public static string? WebRoot { get; set; }
        private string? filename;
        private string? extension;
        private string? relativePath;
        private string? relativeFilePath;
        private string? absoluteUriPath;
        private string? absoluteUriFilePath;
        public string? absoluteFilePath;
        public string? absoluteFileDirectoryPath;
        public bool isSaved;
        public bool isParsed = false;
        public static string ScrapeRoot
        {
            get
            {
                if (WebRoot.HasSchema())
                    WebRoot = WebRoot.RemoveSchema();
                return Path.Combine(AppRoot, WebRoot); ;
            }
        }
        public string Filename
        {
            get
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return "index.html";
                }
                return filename;
            }
            set { filename = value; }
        }
        public string? Extension
        {
            get
            {
                if (string.IsNullOrEmpty(extension))
                {
                    return null;
                }
                return extension;
            }
            set { extension = value; }
        }
        public string? RelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(relativePath))
                {
                    return null;
                }
                return relativePath;
            }
            set { relativePath = value; }
        }
        public string? RelativeFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(relativeFilePath))
                {
                    return null;
                }
                return relativeFilePath;
            }
            set { relativeFilePath = value; }
        }
        public string? AbsoluteUriPath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteUriPath))
                {
                    return null;
                }
                return absoluteUriPath;
            }
            set { absoluteUriPath = value; }
        }
        public string? AbsoluteUriFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteUriFilePath))
                {
                    return null;
                }
                return absoluteUriFilePath;
            }
            set { absoluteUriFilePath = value; }
        }
        public string? AbsoluteFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteFilePath))
                {
                    return null;
                }
                return absoluteFilePath;
            }
            set { absoluteFilePath = value; }
        }
        public string? AbsoluteFileDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteFileDirectoryPath))
                {
                    return null;
                }
                return absoluteFileDirectoryPath;
            }
            set { absoluteFileDirectoryPath = value; }
        }
        public bool IsSaved
        {
            get
            {
                return isSaved;
            }
            set { isSaved = value; }
        }
        public ResourceFile(string path)
        {
            relativeFilePath = path;
        }
        public ResourceFile(HtmlNode? htmlNode, string? baseUriString = null, string? appRoot = null)
        {
            if (htmlNode != null)
            {
                //relativeFilePath
                relativeFilePath = htmlNode.GetRelativeUri();
                if (relativeFilePath != null)
                {
                    //relativePath
                    relativePath = Path.GetDirectoryName(relativeFilePath).ReplaceBackslashesWithForwardslashes();

                    // filename
                    filename = Path.GetFileName(relativeFilePath);

                    // extension
                    extension = Path.GetExtension(relativeFilePath);

                    // absoluteUriFilePath
                    absoluteUriFilePath = Path.Combine(baseUriString, relativeFilePath);

                    // absoluteUriPath
                    absoluteUriPath = Path.Combine(baseUriString, Path.GetDirectoryName(relativeFilePath)).ReplaceBackslashesWithForwardslashes();
                    string absoluteUriFilePathWithoutSchema = absoluteUriFilePath;
                    if (absoluteUriFilePath.HasSchema())
                        absoluteUriFilePathWithoutSchema = absoluteUriFilePathWithoutSchema.RemoveSchema();
                    absoluteUriFilePathWithoutSchema = absoluteUriFilePathWithoutSchema.ReplaceForwardslashesWithBackslashes();

                    //absoluteFilePath
                    //absoluteFileDirectoryPath
                    if (appRoot != null)
                    {
                        absoluteFilePath = Path.Combine(appRoot, absoluteUriFilePathWithoutSchema);
                        absoluteFileDirectoryPath = Path.GetDirectoryName(absoluteFilePath);
                    }
                }
            }
        }
    }
    public static class ResourceFileExtensions
    {
        private static readonly object _lock = new object();
        public static List<ResourceFile> RemoveDuplicateValues(this List<ResourceFile> resources)
        {
            lock (_lock)
            {
                // Remove duplicates
                List<ResourceFile> uniqueResources = resources
                    .GroupBy(resource => resource.absoluteFilePath)
                    .Select(group => group.First())
                    .ToList();

                return uniqueResources;
            }
        }
    }
}
