using GetMeThatPage3.Helpers;
using GetMeThatPageTestProject.Address;

namespace GetMeThatPageTestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1_AddrClass()
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
            //Assert.Pass();
        }
    }
}