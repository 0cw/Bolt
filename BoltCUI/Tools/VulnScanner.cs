using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Bolt_AIO;
using Leaf.xNet;
using static System.IO.File;
using Console = Colorful.Console;
using HttpRequest = Leaf.xNet.HttpRequest;

namespace BoltCUI.Tools
{
    public class VulnScanner
    { 
        private static List<string> Urls;
        private static int URLindex;
        private static int threads;
        private static int Vulns;
        private static int NonVulns;
        private static int TotalChecks;
        private static int errors;
        public static bool ScannerRunning = false;

        public static string[] Keys = {
            "Warning:",
            "<b>Warning</b>:",
            "mysql_fetch_array()",
            ".php on line",
            "MySQL"
        };
        
        
        public static void LoadURLs()
        {
            string fileName = null;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select URLs";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!Exists(fileName));

                Urls = new List<string>(ReadAllLines(fileName));
                Console.Write("\n    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Urls.Count.ToString(), Color.Purple);
                Console.Write(" URLs", Color.White);
                Console.Write("]", Color.White);
                
                var lines = ReadAllLines(fileName);
                WriteAllLines(fileName, lines.Distinct().ToArray());

            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        
        public static void Scanner0()
        {
            if (ScannerRunning == true)
            {
                Console.Title =
                    "                                                                                                [>] BoltAIO | Vuln Scanner | Quanotics#3931 [<]";
                Task.Factory.StartNew(delegate { printt(); });
                return;
            }
            Export.InitializeBlacklist();
            Console.Title =
                "                                                                                                [>] BoltAIO | Vuln Scanner | Quanotics#3931 [<]";
            Console.Clear();
            Console.WriteLine();
            Program.Ascii();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("\n    [", Color.White);
            Console.Write("Select ", Color.White);
            Console.Write("URLs", Color.Purple);
            Console.Write("]", Color.White);
            LoadURLs();
            selectThreads:
            Program.prefix("How many threads do you want to use", "\n");
            Program.prefix(">", "");
            try
            {
                threads = Convert.ToInt32(System.Console.ReadLine());
            }
            catch
            {
                Console.Write("    [", Color.White);
                Console.Write("Error! Input a number", Color.Red);
                Console.Write("]\n", Color.White);
                goto selectThreads;
            }
            Task.Factory.StartNew(delegate { printt(); });
            ScannerRunning = true;
            var num = 0;
            while (num <= threads)
            {
                new Thread(Scanner).Start();
                num = num + 1;
            }
        }
        
        public static void Scanner()
        {
            for(;;)
                try
                {
                    if (URLindex >= Urls.Count - 40) break;
                    var URL = Urls[URLindex];
                    try
                    {
                        using (var req = new HttpRequest())
                        {
                            Interlocked.Increment(ref URLindex);
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
                            req.Cookies = new CookieStorage(false);
                            req.ConnectTimeout = 5000;
                            req.ReadWriteTimeout = 5000;
                            req.KeepAlive = true;
                            req.IgnoreProtocolErrors = true;
                            string get = req.Get(URL + "'").ToString();
                            if (Keys.Any(get.Contains))
                            {
                                Vulns++;
                                TotalChecks++;
                                Export.AsResult("/Vuln_URLs", URL);
                            }
                            else
                            {
                                TotalChecks++;
                                NonVulns++;
                                Export.AsResult("/Non_Vuln_Urls", URL);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        errors++;
                    }
                }
                catch (Exception e)
                {
                    errors++;
                }
        }
        
        public static void printt()
        {
            var lastUpm = TotalChecks;
            for (;;)
            {
                var UPM = TotalChecks - lastUpm;
                lastUpm = TotalChecks;

                Console.Clear();
                Console.WriteLine("");
                Program.Ascii();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("    [", Color.White);
                Console.Write("Vulnerable", Color.DarkOrange);
                Console.Write($"] {Vulns}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("UPM", Color.RoyalBlue);
                Console.Write($"] {UPM * 60}\n", Color.White);

                
                Console.Write("    [", Color.White);
                Console.Write("URLs Checked", Color.DarkGreen);
                Console.Write($"] {URLindex}/{Urls.Count.ToString()}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Non-Vulnerable", Color.Red);
                Console.Write($"] {TotalChecks - Vulns}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("ERRORS", Color.Red);
                Console.Write($"] {errors}\n", Color.White);
                
                Console.Write("\n    [", Color.White);
                Console.Write("INFO", Color.Green);
                Console.Write("] Press \"S\" to go to the menu... NOTE: This does not stop the Vuln-Scanner \n",
                    Color.White);

                if (URLindex >= Urls.Count - 20) break;
                
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.S)
                    {
                        Program.Menu0();
                        break;
                    }
                Thread.Sleep(1000);
            }
        }
    }
}