using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Bolt_AIO;
using Leaf.xNet;
using Console = Colorful.Console;

namespace BoltCUI.Tools
{
    public class ProxyTools
    {
        /*
        private static readonly Variables writer = new Variables();

        // i did NOT make this
        public static void CheckProxy(string line)
        {
            try
            {
                using (var httpRequest = new HttpRequest())
                {
                    var str = Path.Combine(Variables.Folder, "Alive.txt");
                    httpRequest.KeepAlive = false;
                    httpRequest.ConnectTimeout = Variables.TimeOut;
                    var flag = Variables.Type.Contains("1");
                    if (flag)
                    {
                        httpRequest.KeepAlive = false;
                        httpRequest.ConnectTimeout = Variables.TimeOut;
                        httpRequest.Proxy = line;
                        httpRequest.Type = "HTTP";
                        var flag2 = Variables.Permission.Contains("y");
                        if (flag2)
                        {
                            var flag3 = !httpRequest
                                .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null).ToString()
                                .Contains("Connection working!");
                            if (!flag3)
                            {
                                writer.FilePath = str;
                                writer.AppendToFile(line);
                                Variables.Alive++;
                                Console.WriteLine(string.Format("    {0} | Working proxy HTTP/s", line),
                                    Color.Lime);
                                Console.Title =
                                    string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                        Variables.Alive, Variables.Dead);
                            }
                        }
                        else
                        {
                            var flag4 = !httpRequest
                                .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null).ToString()
                                .Contains("Connection working!");
                            if (!flag4)
                            {
                                writer.FilePath = str;
                                writer.AppendToFile(line);
                                Variables.Alive++;
                                Console.WriteLine(string.Format("    {0} | Working proxy HTTP/s", line),
                                    Color.Lime);
                                Console.Title =
                                    string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                        Variables.Alive, Variables.Dead);
                            }
                        }
                    }
                    else
                    {
                        var flag5 = Variables.Type.Contains("2");
                        if (flag5)
                        {
                            httpRequest.KeepAlive = false;
                            httpRequest.ConnectTimeout = Variables.TimeOut;
                            httpRequest.Proxy = line;
                            httpRequest.Type = "socks4";
                            var flag6 = Variables.Permission.Contains("y");
                            if (flag6)
                            {
                                var flag7 = !httpRequest
                                    .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null).ToString()
                                    .Contains("Connection working!");
                                if (!flag7)
                                {
                                    writer.FilePath = str;
                                    writer.AppendToFile(line);
                                    Variables.Alive++;
                                    Console.WriteLine(string.Format("    {0} | Working proxy Socks4", line),
                                        Color.Lime);
                                    Console.Title =
                                        string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                            Variables.Alive, Variables.Dead);
                                }
                            }
                            else
                            {
                                var flag8 = !httpRequest
                                    .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null).ToString()
                                    .Contains("Connection working!");
                                if (!flag8)
                                {
                                    writer.FilePath = str;
                                    writer.AppendToFile(line);
                                    Variables.Alive++;
                                    Console.WriteLine(string.Format("    {0} | Working proxy Socks4", line),
                                        Color.Lime);
                                    Console.Title =
                                        string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                            Variables.Alive, Variables.Dead);
                                }
                            }
                        }
                        else
                        {
                            var flag9 = !Variables.Type.Contains("3");
                            if (!flag9)
                            {
                                httpRequest.KeepAlive = false;
                                httpRequest.ConnectTimeout = Variables.TimeOut;
                                httpRequest.Proxy = line;
                                httpRequest.Type = "socks5";
                                var flag10 = Variables.Permission.Contains("y");
                                if (flag10)
                                {
                                    var flag11 = !httpRequest
                                        .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null)
                                        .ToString().Contains("Connection working!");
                                    if (!flag11)
                                    {
                                        writer.FilePath = str;
                                        writer.AppendToFile(line);
                                        Variables.Alive++;
                                        Console.WriteLine(
                                            string.Format("    {0} | Working proxy Socks5", line), Color.Lime);
                                        Console.Title =
                                            string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                                Variables.Alive, Variables.Dead);
                                    }
                                }
                                else
                                {
                                    var flag12 = !httpRequest
                                        .Start(HttpMethod.GET, new Uri("http://judge.proxyscrape.com/"), null)
                                        .ToString().Contains("Connection working!");
                                    if (!flag12)
                                    {
                                        writer.FilePath = str;
                                        writer.AppendToFile(line);
                                        Variables.Alive++;
                                        Console.WriteLine(
                                            string.Format("    {0} | Working proxy Socks5", line), Color.Lime);
                                        Console.Title =
                                            string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                                                Variables.Alive, Variables.Dead);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("    {0} | Dead proxy", line), Color.Red);
                Variables.Dead++;
                Console.Title = string.Format("Bolt AIO | Proxy Checker | Working: {0} Not Working: {1}",
                    Variables.Alive, Variables.Dead);
            }
        }

        private static void Runner()
        {
            while (Variables.ProxiesQueue.Count != 0) CheckProxy(Variables.ProxiesQueue.Dequeue());
        }

        public static void Check1()
        {
            Console.Title =
                "                                                                                      [>] BoltAIO | Proxy Checker & Scraper | Quanotics#3931 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            selectTimout:
            Console.Write("    [", Color.White);
            Console.Write("Timeout in MS (1 - 5000)", Color.Purple);
            Console.Write("]\n", Color.White);
            Console.Write("    [", Color.White);
            Console.Write(">", Color.Purple);
            Console.Write("]", Color.White);
            try
            {
                Variables.TimeOut = Convert.ToInt32(System.Console.ReadLine());
            }
            catch
            {
                Console.Write("    [", Color.White);
                Console.Write("Error! Input a number", Color.Red);
                Console.Write("]\n", Color.White);
                goto selectTimout;
            }

            selectThreads:
            Console.Write("\n    [", Color.White);
            Console.Write("How many threads do you want to use", Color.Purple);
            Console.Write("]\n", Color.White);
            Console.Write("    [", Color.White);
            Console.Write(">", Color.Purple);
            Console.Write("]", Color.White);
            try
            {
                Variables.Threads = Convert.ToInt32(System.Console.ReadLine());
            }
            catch
            {
                Console.Write("    [", Color.White);
                Console.Write("Error! Input a number", Color.Red);
                Console.Write("]\n", Color.White);
                goto selectThreads;
            }


            for (;;)
            {
                Console.Write("    [", Color.White);
                Console.Write("Select Proxy Type", Color.Purple);
                Console.Write("]\n\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("1", Color.Purple);
                Console.Write("]", Color.White);
                Console.Write(" HTTP\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("2", Color.Purple);
                Console.Write("]", Color.White);
                Console.Write(" SOCKS4\n", Color.White);

                Console.Write("    [", Color.White);
                Console.Write("3", Color.Purple);
                Console.Write("]", Color.White);
                Console.Write(" SOCKS5\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write(">", Color.Purple);
                Console.Write("]", Color.White);

                var Read = System.Console.ReadLine();
                switch (Read)
                {
                    case "1":
                        Variables.Type = "1";
                        break;

                    case "2":
                        Variables.Type = "2";
                        break;

                    case "3":
                        Variables.Type = "3";
                        break;
                }

                if (Variables.Type != "1" && Variables.Type != "2" && Variables.Type != "3") continue;
                break;
            }

            Console.Write("    [", Color.White);
            Console.Write("1", Color.Purple);
            Console.Write("]", Color.White);
            Console.Write(" Check from ProxyScrape.com\n", Color.White);

            Console.Write("    [", Color.White);
            Console.Write("2", Color.Purple);
            Console.Write("]", Color.White);
            Console.Write(" Check from your own file\n", Color.White);

            Console.Write("    [", Color.White);
            Console.Write(">", Color.Purple);
            Console.Write("]", Color.White);
            var flag2 = Console.ReadLine().Contains("1");
            switch (flag2)
            {
                case true:
                {
                    var webClient = new WebClient();
                    var flag3 = Variables.Type.Contains("1");
                    if (flag3)
                    {
                        foreach (var obj in Regex.Matches(
                            webClient.DownloadString(
                                "https://api.proxyscrape.com/?request=displayproxies&proxytype=http"),
                            "\\b(\\d{1,3}\\.){3}\\d{1,3}\\:\\d{1,8}\\b", RegexOptions.Singleline))
                        {
                            var match = (Match) obj;
                            Variables.Proxies.Add(match.Groups[0].Value);
                        }

                        Variables.PrintWithoutPrefix("    Loaded: " + Variables.Proxies.Count);
                    }
                    else
                    {
                        var flag4 = Variables.Type.Contains("2");
                        if (flag4)
                        {
                            foreach (var obj2 in Regex.Matches(
                                webClient.DownloadString(
                                    "https://api.proxyscrape.com/?request=displayproxies&proxytype=socks4"),
                                "\\b(\\d{1,3}\\.){3}\\d{1,3}\\:\\d{1,8}\\b", RegexOptions.Singleline))
                            {
                                var match2 = (Match) obj2;
                                Variables.Proxies.Add(match2.Groups[0].Value);
                            }

                            Variables.PrintWithoutPrefix("    Loaded: " + Variables.Proxies.Count);
                        }
                        else
                        {
                            var flag5 = Variables.Type.Contains("3");
                            if (flag5)
                            {
                                foreach (var obj3 in Regex.Matches(
                                    webClient.DownloadString(
                                        "https://api.proxyscrape.com/?request=displayproxies&proxytype=socks5"),
                                    "\\b(\\d{1,3}\\.){3}\\d{1,3}\\:\\d{1,8}\\b", RegexOptions.Singleline))
                                {
                                    var match3 = (Match) obj3;
                                    Variables.Proxies.Add(match3.Groups[0].Value);
                                }

                                Variables.PrintWithoutPrefix("    Loaded: " + Variables.Proxies.Count);
                            }
                        }
                    }

                    CheckProxies();
                    break;
                }
                default:
                {
                    Variables.PrintWithoutPrefix("    Drag your Proxy file in here!");
                    var str = Console.ReadLine();
                    Variables.Proxies = File.ReadLines(!str.Contains("\"") ? str : str.Replace("\"", ""))
                        .ToList();
                    Variables.PrintWithoutPrefix("    Loaded: " + Variables.Proxies.Count +
                                                 " From your file!");
                    CheckProxies();
                    break;
                }
            }
        }

        private static void CheckProxies()
        {
            var flag = Variables.Type.Contains("1");
            switch (flag)
            {
                case true:
                    Variables.Folder = Folder("Checker - HTTP");
                    break;
                default:
                {
                    var flag2 = Variables.Type.Contains("2");
                    if (flag2)
                    {
                        Variables.Folder = Folder("Checker - Socks4");
                    }
                    else
                    {
                        var flag3 = Variables.Type.Contains("3");
                        if (flag3) Variables.Folder = Folder("Checker - Socks5");
                    }

                    break;
                }
            }

            ConvertProxies();
            try
            {
                for (var index = 0; index < Variables.Threads; index++) new Thread(Runner).Start();
            }
            catch (Exception)
            {
            }
        }

        private static string Folder(string mode)
        {
            var str = DateTime.Now.ToString("dd-MM-yyyy-hhmm - ");
            Directory.CreateDirectory(str + mode);
            return str + mode;
        }

        private static void ConvertProxies()
        {
            foreach (var proxy in Variables.Proxies) Variables.ProxiesQueue.Enqueue(proxy);
        }
        */
    }
}