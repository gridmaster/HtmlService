using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ServiceTest
{
    class Program
    {
        private static void Main(string[] args)
        {
            string symbolPath = @"D:\Templates\symbols.txt";
            var logFile = File.ReadAllLines(symbolPath);

            List<string> SymbolList = new List<string>(logFile);

            string[] symbols = new string[] {"aapl", "ibm", "goog", "yhoo", "f", "v", "ua", "sbux", "elx", "klac", 
                                             "qlgc", "gm", "gs", "c", "glow", "bfd", "x", "grpn", "msft", "znga"};

            string uri = string.Concat("http://localhost:8040",
                                       string.Format("/htmlservice?uri={0}",
                                                     "http://finance.yahoo.com/q/ks?s=X!!X+Key+Statistics"));
                // "file:///D:/Templates/Test4.htm")); // "www.cnn.com")); 

            DateTime dtStart = DateTime.Now;
            Console.WriteLine("Start at {0}", dtStart);
            foreach (string symbol in SymbolList )
            {
                TryThis(uri.Replace("X!!X", symbol), symbol);
            }
            Console.WriteLine("Ends at {0}", DateTime.Now);
            Console.WriteLine("elapsed time {0} reading {1} symbols", DateTime.Now.Subtract(dtStart), SymbolList.Count);

            Console.ReadKey();
        }

        public static void TryThis(string uri,  string symbol)
        {  // this works
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";

            try
            {
                // Get response   
                using (WebResponse response = request.GetResponse() as WebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[response.ContentLength];
                        MemoryStream ms = new MemoryStream();
                        int bytesRead, totalBytesRead = 0;

                        do
                        {
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            totalBytesRead += bytesRead;

                            ms.Write(buffer, 0, bytesRead);
                        } while (bytesRead > 0);

                        ms.Position = 0;
                        var sr = new StreamReader(ms);
                        var myStr = sr.ReadToEnd();

                        string result = System.Web.HttpUtility.HtmlDecode(myStr);

                        string path = @"D:\Templates\test\ks_" + symbol + ".htm";

                        // Write the string to a file.
                        StreamWriter file = new StreamWriter(path);
                        file.WriteLine(result);

                        file.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Blew up on {0}: {1}", symbol, ex.Message));
                //throw;
            }
        }
    }
}