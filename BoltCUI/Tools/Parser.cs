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
using TextBox = System.Windows.Forms.TextBox;

namespace BoltCUI.Tools
{
    public class Parser
    {
        public static List<string> Dorks;
        public static List<string> Proxies;
        public static int DorkIndex;
        public static int ProxyIndex;
        public static int Stop;
        public static int ValidUrls;
        public static int FilteredUrls;
        public static int blacklistedUrls;
        public static int errs;
        public static string ProxyType0;
        public static int threads;
        private static readonly TextBox Links = new TextBox();
        public static string[] BlackLUrls;
        public static int DPM;
        public static int UPM;
        public static string action;
        public static bool ParserRunning = false;

        public static void LoadDorks()
        {
            string fileName = null;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Dorks";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!Exists(fileName));

                Dorks = new List<string>(ReadAllLines(fileName));
                Console.Write("\n    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Dorks.Count.ToString(), Color.Purple);
                Console.Write(" Dorks", Color.White);
                Console.Write("]", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
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
                } while (!Exists(fileName));

                Proxies = new List<string>(ReadAllLines(fileName));
                Console.Write("\n    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Proxies.Count.ToString(), Color.Purple);
                Console.Write(" Proxies", Color.White);
                Console.Write("]\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static void Parser0()
        {
            if (ParserRunning == true)
            {
                Console.Title =
                    "                                                                                       [>] BoltAIO | Dork Parser | Quanotics#3931 [<]";
                Task.Factory.StartNew(delegate { printt(); });
                return;
            }
            Export.InitializeBlacklist();
            Console.Title =
                "                                                                                       [>] BoltAIO | Dork Parser Menu | Quanotics#3931 [<]";
            Console.Clear();
            Console.WriteLine();
            Program.Ascii();
            Console.WriteLine();
            Console.WriteLine();
            selectOption:
            Program.prefix("1", "Bing Parser\n");
            Program.prefix("2", "AOL Parser\n");
            Program.prefix("3", "Ask Parser\n");
            Program.prefix("4", "Mail.ru Parser\n");
            Program.prefix("5", "My Web Search\n");
            Program.prefix("X", "Go Back\n");
            Program.prefix(">", "");
            var op1 = System.Console.ReadLine().ToUpper();
            switch (op1)
            {
                case "1":
                    action = "bing.com";
                    break;
                case "2":
                    action = "aol.com";
                    break;
                case "3":
                    action = "ask.com";
                    break;
                case "4":
                    action = "mail.ru";
                    break;
                case "5":
                    action = "mywebsearch";
                    break;
                case "X":
                    Program.Tools0();
                    break;
                default:
                    Program.prefix("Invalid Option", "\n");
                    Thread.Sleep(300);
                    goto selectOption;
                    break;
            }

            Console.Write("\n    [", Color.White);
            Console.Write("Select ", Color.White);
            Console.Write("Dorks", Color.Purple);
            Console.Write("]", Color.White);
            LoadDorks();
            if (op1 != "1")
            {
                Console.Write("\n    [", Color.White);
                Console.Write("Select ", Color.White);
                Console.Write("Proxies", Color.Purple);
                Console.Write("]", Color.White);
                LoadProxies();
                for (;;)
                {
                    Program.prefix("Select Proxy Type", "\n");
                    Program.prefix("1", "HTTP\n");
                    Program.prefix("2", "SOCKS4\n");
                    Program.prefix("3", "SOCKS5\n");
                    Program.prefix(">", "");
                    var Read = System.Console.ReadLine();
                    switch (Read)
                    {
                        case "1":
                            ProxyType0 = "HTTP";
                            break;
                        case "2":
                            ProxyType0 = "SOCKS4";
                            break;

                        case "3":
                            ProxyType0 = "SOCKS5";
                            break;
                    }

                    if (ProxyType0 != "HTTP" && ProxyType0 != "SOCKS4" && ProxyType0 != "SOCKS5") continue;
                    break;
                }
            }

            selectThreads:
            Console.WriteLine();
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

            ParserRunning = true;
            Task.Factory.StartNew(delegate { printt(); });
            BlackLUrls = ReadAllLines("blacklist.txt").ToArray();
            switch (op1)
            {
                case "1":
                {
                    var num = 0;
                    while (num <= threads)
                    {
                        new Thread(StartBingParser).Start();
                        num = num + 1;
                    }

                    break;
                }
                case "2":
                {
                    var num = 0;
                    while (num <= threads)
                    {
                        new Thread(StartAOLParser).Start();
                        num = num + 1;
                    }

                    break;
                }
                case "3":
                {
                    var num = 0;
                    while (num <= threads)
                    {
                        new Thread(StartAskParser).Start();
                        num = num + 1;
                    }

                    break;
                }
                case "4":
                {
                    var num = 0;
                    while (num <= threads)
                    {
                        new Thread(StartMailRuParser).Start();
                        num = num + 1;
                    }

                    break;
                }
                case "5":
                {
                    var num = 0;
                    while (num <= threads)
                    {
                        new Thread(StartMyWebSearchParser).Start();
                        num = num + 1;
                    }

                    break;
                }
            }

        }

        public static void StartBingParser()
        {            
            Console.Title = "                                                                                                 [>] BoltAIO | Bing Parser | Quanotics#3931 [<]";

            for (;;)
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        using (var req = new HttpRequest())
                        {
                            try
                            {
                                switch (ProxyType0)
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

                                MatchCollection regex;

                                req.UserAgent =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";

                                var urls1 = req.Get("https://www.bing.com/search?num=100&q=" + dork).ToString();


                                regex = new Regex(
                                        @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)")
                                    .Matches(urls1);


                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/Bing_urls", line);
                                            }
                                        req.Dispose();
                                    }
                                }

                                req.Dispose();
                                Interlocked.Increment(ref DorkIndex);
                            }
                            catch
                            {
                                Interlocked.Increment(ref errs);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref errs);
                    }
                }
                catch (Exception e)
                {
                    Interlocked.Increment(ref errs);
                }
            
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }


        public static void StartAOLParser()
        {
            Console.Title = "                                                                                                 [>] BoltAIO | AOL Parser | Quanotics#3931 [<]";
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        string str1;
                        using (var req = new HttpRequest())
                        {
                            var timeout = 250;
                            req.ConnectTimeout = timeout;
                            req.ReadWriteTimeout = timeout;
                            switch (ProxyType0)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                            }

                            MatchCollection regex;

                            req.Cookies = new CookieStorage();
                            req.IgnoreProtocolErrors = true;
                            req.CharacterSet = Encoding.GetEncoding(65001);
                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                            req.AddHeader("Upgrade-Insecure-Requests", "1");
                            req.AddHeader("Accept",
                                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                            req.AddHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                            var getUrls = req.Get("http://search.aol.com/aol/search?q=" + dork + "&v_t=na&page=1")
                                .ToString();
                            if (getUrls.Contains(
                                    "Request denied: source address is sending an excessive volume of requests.") ||
                                !getUrls.Contains("aol.com"))
                            {
                                errs++;
                            }
                            else
                            {
                                regex = new Regex(
                                        @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)")
                                    .Matches(getUrls);


                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/AOL_urls", line);
                                            }
                                        req.Dispose();
                                    }
                                }

                                Interlocked.Increment(ref DorkIndex);
                                req.Dispose();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errs++;
                    }

                }
                catch (Exception e)
                {
                    errs++;
                    throw;
                }
            }
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }


        public static void StartAskParser()
        {
            Console.Title = "                                                                                                 [>] BoltAIO | Ask Parser | Quanotics#3931 [<]";
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        string str1;
                        using (var req = new HttpRequest())
                        {
                            var timeout = 250;
                            req.ConnectTimeout = timeout;
                            req.ReadWriteTimeout = timeout;
                            switch (ProxyType0)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                            }
                            
                            req.Cookies = new CookieStorage();
                            req.IgnoreProtocolErrors = true;
                            req.CharacterSet = Encoding.GetEncoding(65001);
                            req.UserAgent = "Linux / Firefox 44: Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:44.0) Gecko/20100101 Firefox/44.0";
                            var getUrls = req.Get("www.ask.com/web?q=" + dork + "&page=1").ToString();
                            if (getUrls.Contains("Your client does not have permission to access this site.") || !getUrls.Contains("ask.com"))
                            {
                                errs++;
                            }
                            else
                            {
                                var regex = Regex.Matches(getUrls,
                                    @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");


                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/Ask_urls", line);
                                            }
                                        req.Dispose();
                                    }
                                }

                                Interlocked.Increment(ref DorkIndex);
                                req.Dispose();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errs++;
                    }

                }
                catch (Exception e)
                {
                    errs++;
                    throw;
                }
            }
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }


        
        
        
        
        public static void StartMailRuParser()
        {
            Console.Title = "                                                                                                 [>] BoltAIO | Ask Parser | Quanotics#3931 [<]";
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        string str1;
                        using (var req = new HttpRequest())
                        {
                            var timeout = 250;
                            req.ConnectTimeout = timeout;
                            req.ReadWriteTimeout = timeout;
                            switch (ProxyType0)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                            }
                            
                            req.Cookies = new CookieStorage();
                            req.IgnoreProtocolErrors = true;
                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                            req.AddHeader("Upgrade-Insecure-Requests", "1");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                            req.KeepAlive = true;
                            var getUrls = req.Get("https://go.mail.ru/search?rf=0010&fm=1&q=" + dork + "&sf=" + "1").ToString();
                            if (getUrls.Contains("\"blocked\":true") || !getUrls.Contains("mail.ru"))
                            {
                                errs++;
                            }
                            else
                            {
                                var regex = Regex.Matches(getUrls,
                                    @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");


                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/Mail_ru_urls", line);
                                            }
                                        req.Dispose();
                                    }
                                }

                                Interlocked.Increment(ref DorkIndex);
                                req.Dispose();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errs++;
                    }
                }
                catch (Exception e)
                {
                    errs++;
                    throw;
                }
            }
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }
        
        public static void StartMyWebSearchParser()
        {
            Console.Title = "                                                                                                 [>] BoltAIO | Ask Parser | Quanotics#3931 [<]";
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        string str1;
                        using (var req = new HttpRequest())
                        {
                            var timeout = 250;
                            req.ConnectTimeout = timeout;
                            req.ReadWriteTimeout = timeout;
                            switch (ProxyType0)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                            }
                            
                            req.Cookies = new CookieStorage();
                            req.IgnoreProtocolErrors = true;
                            req.CharacterSet = Encoding.GetEncoding(65001);
                            req.UserAgent =
                                "Windows / IE 11: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            var getUrls = req.Get("http://int.search.mywebsearch.com/mywebsearch/GGweb.jhtml?searchfor=" + dork + "&pn=" + "1").ToString();
                            if (getUrls.Contains("mywebsearch.com"))
                            {
                                var regex = Regex.Matches(getUrls,
                                    @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/websearch_urls", line);
                                            }
                                    }
                                }
                                req.Dispose();
                                Interlocked.Increment(ref DorkIndex);
                            }
                            else
                            {
                                errs++;
                                req.Dispose();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errs++;
                    }

                }
                catch (Exception e)
                {
                    errs++;
                    throw;
                }
            }
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }
        
        
        
        
        public static void StartOrangeParser()
        {
            Console.Title = "                                                                                                 [>] BoltAIO | Ask Parser | Quanotics#3931 [<]";
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    if (DorkIndex >= Dorks.Count - 40) break;

                    var dork = Dorks[DorkIndex];
                    try
                    {
                        string str1;
                        using (var req = new HttpRequest())
                        {
                            var timeout = 250;
                            req.ConnectTimeout = timeout;
                            req.ReadWriteTimeout = timeout;
                            switch (ProxyType0)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Proxies[ProxyIndex]);
                                    req.Proxy.ConnectTimeout = timeout;
                                    break;
                            }
                            
                            req.Cookies = new CookieStorage();
                            req.IgnoreProtocolErrors = true;
                            req.CharacterSet = Encoding.GetEncoding(65001);
                            req.UserAgent =
                                "Windows / IE 11: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            var getUrls = req.Get("http://lemoteur.orange.fr/?kw=" + dork + "&bhv=web_fr&module=orange&target=orange").ToString();
                            if (getUrls.Contains("mywebsearch.com"))
                            {
                                var regex = Regex.Matches(getUrls,
                                    @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
                                req.Dispose();
                                if (regex.Count != 0)
                                {
                                    var arr = (from m in regex.OfType<Match>()
                                        select m.Value).ToArray();
                                    if (Links != null)
                                    {
                                        Links.Text = string.Join(Environment.NewLine, arr.Distinct());

                                        foreach (var line in Links.Lines)
                                            if (BlackLUrls.Any(line.Contains))
                                            {
                                                ValidUrls++;
                                                blacklistedUrls++;
                                            }
                                            else
                                            {
                                                ValidUrls++;
                                                FilteredUrls++;
                                                Export.AsResult("/websearch_urls", line);
                                            }
                                    }
                                }
                                req.Dispose();

                                Interlocked.Increment(ref DorkIndex);
                            }
                            else
                            {
                                req.Dispose();
                                errs++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errs++;
                    }
                }
                catch (Exception e)
                {
                    errs++;
                    throw;
                }
            }
            var lines = ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt");
            WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Ask_urls" + ".txt",
                lines.Distinct().ToArray());

            Console.Write("\n\n    INFO", Color.ForestGreen);
            Console.Write(" | ", Color.LightGreen);
            Console.Write("Done Parsing", Color.Purple);
            Thread.Sleep(5000);
            Program.Menu0();
        }
        
        public static void printt()
        {
            var lastDpm = DorkIndex;
            var lastUpm = ValidUrls;
            for (;;)
            {
                DPM = DorkIndex - lastDpm;
                UPM = ValidUrls - lastUpm;
                lastDpm = DorkIndex;
                lastUpm = ValidUrls;

                Console.Clear();
                Console.WriteLine("");
                Program.Ascii();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("    [", Color.White);
                Console.Write("Urls", Color.DarkOrange);
                Console.Write($"] {ValidUrls}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("DPM", Color.RoyalBlue);
                Console.Write($"] {DPM * 60}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("UPM", Color.MediumBlue);
                Console.Write($"] {UPM * 60}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Filtered Urls", Color.LimeGreen);
                Console.Write($"] {FilteredUrls}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Dorks Parsed", Color.DarkGreen);
                Console.Write($"] {DorkIndex}/{Dorks.Count.ToString()}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("Blacklisted Urls", Color.Red);
                Console.Write($"] {blacklistedUrls}\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("ERRORS", Color.Red);
                Console.Write($"] {errs}\n", Color.White);
                
                Console.Write("\n    [", Color.White);
                Console.Write("INFO", Color.Green);
                Console.Write("] Press \"S\" to go to the menu... NOTE: This does not stop the parser \n",
                    Color.White);

                if (DorkIndex >= Dorks.Count - 20) break;
                
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