using GetMeThatPage3.Helpers;
using GetMeThatPage3.Helpers.Html;
using GetMeThatPageTestProject.Address;
using HtmlAgilityPack;

namespace GetMeThatPageTestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_AddrClass()
        {
            try
            {
                foreach (string? relativePath in AddrData.relativePaths)
                {

                    bool isPathRelativeAddress = relativePath.IsRelativeAdress();
                    if (isPathRelativeAddress)
                    {
                        string strToJoin = relativePath;
                        string joined = Addr.Join(strToJoin, strToJoin);

                        bool areStringsJoinedAlsoRelative = joined.IsRelativeAdress();
                        Assert.That(areStringsJoinedAlsoRelative, Is.EqualTo(isPathRelativeAddress));
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [Test]
        public void Test_HtmlNodeExtensions()
        {
            string url = "http://books.toscrape.com";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            HtmlNodeCollection scriptElements = doc.DocumentNode.SelectNodes("//script");
            foreach (HtmlNode scriptElement in scriptElements)
            {
                bool isScriptElement = scriptElement.IsScript();
                Assert.That(isScriptElement, Is.EqualTo(true));
            }
            Assert.Pass();
        }
    }
}