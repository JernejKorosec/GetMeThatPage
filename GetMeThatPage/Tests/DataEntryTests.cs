using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetMeThatPage.v2;
using NUnit.Framework;
using GetMeThatPage.v2.WebScraper.Helpers;

namespace GetMeThatPage.Tests
{
    public class DataEntryTests
    {
        private Functions helpers = new Functions();
        
        [Test]
        public DataEntry getDataEntryFromUrl(Uri fullUri)
        {
            /*
            //doc = web.Load(fullUri);
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
