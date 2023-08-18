using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetMeThatPage.v2.WebScraper.Helpers;

namespace GetMeThatPage.v2.WebScraperV2
{
    public class WebScraperV2
    {
        private Uri hardcodedWebPageUrl;
        private string hardcodedSavePath;
        private Functions helpers = new Functions();
        HtmlWeb web = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 };
        HtmlDocument? doc;
        public WebScraperV2(Uri hardcodedWebPageUrl, string hardcodedSavePath)
        {
            this.hardcodedWebPageUrl = hardcodedWebPageUrl;
            this.hardcodedSavePath = hardcodedSavePath;
        }
        public DataEntry getDataEntryFromUrl(Uri fullUri)
        {
            //doc = web.Load(fullUri);
            DataEntry entry = new DataEntry
            {
                FileName = helpers.getFileNameFromUri(fullUri),
                RelativeLocalPath = helpers.getRelativeLocalPath(fullUri), 
                AbsoluteLocalPath = helpers.getAbsoluteLocalPath(hardcodedSavePath, fullUri,  helpers.getFileNameFromUri(fullUri)),
                RootLocalPathWithRootPage = helpers.getRootLocalPathWithRootPage(hardcodedSavePath, fullUri),
                RootLocalPath = hardcodedSavePath,
                isSaved = false,
                isParsedForResources = false,
                Size = 0,   
                Scheme = fullUri.Scheme,
                RelativeWebUri = helpers.getRelativeRemotePath(fullUri),
                AbsoluteWebUri = fullUri,
                WebHost = fullUri.Host,
                FileNameOnWeb = helpers.getFileNameFromUri(fullUri),
                ResourceType = helpers.getResourceType(helpers.getFileNameFromUri(fullUri)),
            };
            return entry;
        }
        public DataEntry getDataEntryFromRelativeUrl(String path)
        {

            /*
            DataEntry entry = new DataEntry
            {
                FileName = helpers.getFileNameFromUri(fullUri),
                RelativeLocalPath = helpers.getRelativeLocalPath(fullUri),
                AbsoluteLocalPath = helpers.getAbsoluteLocalPath(hardcodedSavePath, fullUri, helpers.getFileNameFromUri(fullUri)),
                RootLocalPathWithRootPage = helpers.getRootLocalPathWithRootPage(hardcodedSavePath, fullUri),
                RootLocalPath = hardcodedSavePath,
                isSaved = false,
                isParsedForResources = false,
                Size = 0,
                Scheme = fullUri.Scheme,
                RelativeWebUri = helpers.getRelativeRemotePath(fullUri),
                AbsoluteWebUri = fullUri,
                WebHost = fullUri.Host,
                FileNameOnWeb = helpers.getFileNameFromUri(fullUri),
                ResourceType = helpers.getResourceType(helpers.getFileNameFromUri(fullUri)),
            };
            return entry;
            */
            return null;
        }

    }
}
