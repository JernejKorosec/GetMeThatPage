using GetMeThatPage3.Helpers;
using GetMeThatPage3.HTML.Test;
using HtmlAgilityPack;

namespace GetMeThatPage3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            //Test.test1();
            Addr a = new Addr();
            string[] urls = a.relativePaths;

            foreach (string url in urls)
            {
                try
                {
                    Uri uri = new Uri(url);
                    Console.WriteLine(uri);
                }
                catch (Exception)
                {

                    //throw;
                }
                
            }




            Console.ReadKey();
        }

      
    }
}