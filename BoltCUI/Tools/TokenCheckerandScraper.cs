using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bolt_AIO;
using Leaf.xNet;
using Console = Colorful.Console;

namespace BoltCUI.Tools
{
    public class TokenCheckerandScraper
    {
        public static List<string> Proxies = new List<string>();
        public static int ProxyIndex;
        public static List<string> Tokens = new List<string>();
        public static int TokensIndex;
        public static int errors;
        public static int validTokens;
        public static int totalTokenChecks;
        public static int verified;
        public static int Threads;
        public static string ProxyType;
        public static bool CheckerRunning = false;

        public static void TCS0()
        {
            if (CheckerRunning == true)
            {
                Console.Title =
                    "                                                                                                 [>] BoltAIO | Token Checker | Quanotics#3931 [<]";
                Task.Factory.StartNew(delegate { printt(); });
                return;
            }
            Console.Title =
                "                                                                                                 [>] BoltAIO | Token Checker | Quanotics#3931 [<]";
            Console.Clear();
            Console.WriteLine();
            Program.Ascii();
            Console.WriteLine();
            Console.WriteLine();
            Colorful.Console.Write("\n    [", Color.White);
            Colorful.Console.Write("Select ", Color.White);
            Colorful.Console.Write("Proxies", Color.Purple);
            Colorful.Console.Write("]", Color.White);
            LoadProxies();
            Colorful.Console.Write("\n    [", Color.White);
            Colorful.Console.Write("Select ", Color.White);
            Colorful.Console.Write("Tokens", Color.Purple);
            Colorful.Console.Write("]", Color.White);
            LoadTokens();
            for (;;)
            {
                Program.prefix("Select Proxy Type", "\n");
                Program.prefix("1", "HTTP\n");
                Program.prefix("2", "SOCKS4\n");
                Program.prefix("3", "SOCKS5\n");
                Program.prefix(">", "");
                var Read = Console.ReadLine();
                switch (Read)
                {
                    case "1":
                        ProxyType = "HTTP";
                        break;

                    case "2":
                        ProxyType = "SOCKS4";
                        break;

                    case "3":
                        ProxyType = "SOCKS5";
                        break;
                }

                if (ProxyType != "HTTP" && ProxyType != "SOCKS4" && ProxyType != "SOCKS5") continue;
                break;
            }

            selectThreads:
            Console.WriteLine();
            Program.prefix("How many threads do you want to use", "\n");
            Program.prefix(">", "");
            try
            {
                Threads = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Program.prefix("Error! Input a number", "");
                Console.Write("    [", Color.White);
                Console.Write("Error! Input a number", Color.Red);
                Console.Write("]\n", Color.White);
                goto selectThreads;
            }

            CheckerRunning = true;
            Task.Factory.StartNew(delegate { printt(); });
            var num = 0;
            while (num <= Threads)
            {
                new Thread(CheckTokens).Start();
                num = num + 1;
            }
        }

        public static void CheckTokens()
        {
            for (;;)
                try
                {
                    if (ProxyIndex >= Proxies.Count) ProxyIndex = 0;
                    if (TokensIndex >= Tokens.Count) break;
                    var token = Tokens[TokensIndex];
                    using (var req = new HttpRequest())
                    {
                        try
                        {
                            Interlocked.Increment(ref ProxyIndex);
                            switch (ProxyType)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                            }

                            Interlocked.Increment(ref TokensIndex);
                            req.AddHeader("authorization", token);
                            var get = req.Get("https://discord.com/api/v8/users/@me").ToString();
                            if (get.Contains("\"id\": \""))
                            {
                               string Verified = Parse(get, "\"verified\": ", ",");
                               if (Verified == "true")
                               {
                                   verified++;
                                   Export.AsResult("/Verified_Tokens", token);
                               }
                               validTokens++;
                               Export.AsResult("/All_Valid_Tokens", token);
                            }
                        }
                        catch (Exception e)
                        {
                            errors++;
                        }
                    }
                }
                catch (Exception e)
                {
                    errors++;
                }
        }

        public static void LoadProxies()
        {
            string fileName = null;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Proxies";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                Proxies = new List<string>(File.ReadAllLines(fileName));
                Colorful.Console.Write("\n    [", Color.White);
                Colorful.Console.Write("Selected ", Color.White);
                Colorful.Console.Write(Proxies.Count.ToString(), Color.Purple);
                Colorful.Console.Write(" Proxies", Color.White);
                Colorful.Console.Write("]", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static void LoadTokens()
        {
            string fileName = null;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Tokens";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                Tokens = new List<string>(File.ReadAllLines(fileName));
                Colorful.Console.Write("\n    [", Color.White);
                Colorful.Console.Write("Selected ", Color.White);
                Colorful.Console.Write(Tokens.Count.ToString(), Color.Purple);
                Colorful.Console.Write(" Tokens", Color.White);
                Colorful.Console.Write("]\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
        
        public static void printt()
        {
            var lastDpm = TokensIndex;
            for (;;)
            {
                var CPM = TokensIndex - lastDpm;
                lastDpm = TokensIndex;

                Console.Clear();
                Console.WriteLine("");
                Program.Ascii();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("    [", Color.White);
                Console.Write("Valid Tokens", Color.LimeGreen);
                Console.Write($"] {validTokens}\n", Color.White);
                Console.Write("        [", Color.White);
                Console.Write("Verified Tokens", Color.MediumSpringGreen);
                Console.Write($"] {verified}\n", Color.White);
                Console.Write("        [", Color.White);
                Console.Write("Non Verified Tokens", Color.SpringGreen);
                Console.Write($"] {validTokens - verified}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Invalid Tokens", Color.Red);
                Console.Write($"] {TokensIndex - validTokens}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Tokens Checked", Color.DarkGreen);
                Console.Write($"] {TokensIndex}/{Tokens.Count.ToString()}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("CPM", Color.RoyalBlue);
                Console.Write($"] {CPM * 60}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("ERRORS", Color.Red);
                Console.Write($"] {errors}\n", Color.White);
                
                Console.Write("\n    [", Color.White);
                Console.Write("INFO", Color.Green);
                Console.Write("] Press \"S\" to go to the menu... NOTE: This does not stop the Checker \n",
                    Color.White);

                if (TokensIndex >= Tokens.Count - 20) break;
                
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.S)
                    {
                        Program.Menu0();
                        break;
                    }
                Thread.Sleep(1000);
            }
        }
        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }

    }
}