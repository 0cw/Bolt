using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Leaf.xNet;
using Newtonsoft.Json.Linq;

namespace Bolt_AIO
{
    public enum Hash
    {
        SHA1,
        SHA256,
        SHA512
    }

    public class Captcha
    {
        public string apiKey { get; set; }
        public string siteKey { get; set; }
        public string siteURL { get; set; }
    }

    public static class Crypto
    {
        public static byte[] HMACSHA1(byte[] input, byte[] key)
        {
            using (var hmac = new HMACSHA1(key))
            {
                return hmac.ComputeHash(input);
            }
        }

        public static string ToHex(this byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }


    internal class Functions
    {
        public static string Hmac(string baseString, Hash type, string key, bool inputBase64 = false,
            bool keyBase64 = false, bool outputBase64 = false)
        {
            var rawInput = inputBase64 ? Convert.FromBase64String(baseString) : Encoding.UTF8.GetBytes(baseString);
            var rawKey = keyBase64 ? Convert.FromBase64String(key) : Encoding.UTF8.GetBytes(key);
            byte[] signature;

            switch (type)
            {
                case Hash.SHA1:
                    signature = Crypto.HMACSHA1(rawInput, rawKey);
                    break;

                default:
                    throw new NotSupportedException("Unsupported algorithm");
            }

            return outputBase64 ? Convert.ToBase64String(signature) : signature.ToHex();
        }

        public static string RandomString(string Randomize)
        {
            var After = "";

            // Lists
            var HexList = "123456789abcdef";
            var LowerList = "abcdefghijklmnopqrstuvwxyz";
            var UpperList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var DigitList = "1234567890";
            var SymbolList = "!@#$%^&*()_+";
            var UpperDigitList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var UpperLowerDigitList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var ran = new Random();

            for (var i = 0; i < Randomize.Length - 1; i++)
                switch (Randomize[i] + Randomize[i + 1].ToString())
                {
                    // HexaDecimal Random
                    // LowerCase Random
                    case "?h":
                        After += HexList[ran.Next(0, HexList.Length)];
                        break;
                    // UpperCase Random
                    case "?l":
                        After += LowerList[ran.Next(0, LowerList.Length)];
                        break;
                    // Digit Random
                    case "?u":
                        After += UpperList[ran.Next(0, UpperList.Length)];
                        break;
                    // Upper Digit Random
                    case "?d":
                        After += DigitList[ran.Next(0, DigitList.Length)];
                        break;
                    // Upper Lower Digit Random
                    case "?m":
                        After += UpperDigitList[ran.Next(0, UpperDigitList.Length)];
                        break;
                    case "?i":
                        After += UpperLowerDigitList[ran.Next(0, UpperLowerDigitList.Length)];
                        break;
                    // Dash Separators
                    case "?s":
                        After += SymbolList[ran.Next(0, SymbolList.Length)];
                        break;
                    default:
                    {
                        if (Randomize[i].ToString().Contains("-"))
                            After += "-";
                        // Incase there's a static number/letter here, we keep it the same
                        else if (Randomize[i - 1].ToString().Equals("-") && !Randomize[i].ToString().Equals("?"))
                            After += Randomize[i].ToString();
                        break;
                    }
                }

            return After;
        }

        public static string Base64Decode(string Base)
        {
            var data = Convert.FromBase64String(Base);
            var decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        public static string Base64Encode(string Base)
        {
            var plain = Encoding.UTF8.GetBytes(Base);
            return Convert.ToBase64String(plain);
        }

        public static string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (var theByte in crypto) hash.Append(theByte.ToString("x2"));
            return hash.ToString();
        }

        public static string SolveRecaptcha(Captcha captcha)
        {
            while (true)
                using (var req = new HttpRequest())
                {
                    var res1 = req.Get("http://2captcha.com/in.php?key=" + captcha.apiKey +
                                       "&method=userrecaptcha&googlekey=" + captcha.siteKey + "&pageurl=" +
                                       captcha.siteURL + "");
                    var text1 = res1.ToString();
                    if (text1.Contains("OK"))
                    {
                        var taskId = text1.Replace("OK|", "");
                        while (true)
                        {
                            Thread.Sleep(5000);
                            var res2 = req.Get("http://2captcha.com/res.php?key=" + captcha.apiKey + "&action=get&id=" +
                                               taskId + "");
                            var text2 = res2.ToString();
                            if (text2.Contains("OK"))
                            {
                                var token = text2.Replace("OK|", "");
                                return token;
                            }

                            if (!text2.Contains("CAPCHA_NOT_READY") && !text2.Contains("OK"))
                                //Export.SaveData(text2, "errors_res");
                                return null;
                        }
                    }

                    return null;
                }
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString().ToLower();
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

        private static string BuildLRPattern(string ls, string rs)
        {
            var left = string.IsNullOrEmpty(ls) ? "^" : Regex.Escape(ls); // Empty LEFT = start of the line
            var right = string.IsNullOrEmpty(rs) ? "$" : Regex.Escape(rs); // Empty RIGHT = end of the line
            return "(?<=" + left + ").+?(?=" + right + ")";
        }

        public static IEnumerable<string> LR(string input, string left, string right, bool recursive = false,
            bool useRegex = false)
        {
            // No L and R = return full input
            if (left == string.Empty && right == string.Empty)
                return new[] {input};

            // L or R not present and not empty = return nothing
            if (left != string.Empty && !input.Contains(left) || right != string.Empty && !input.Contains(right))
                return new string[] { };

            var partial = input;
            var pFrom = 0;
            var pTo = 0;
            var list = new List<string>();

            if (recursive)
            {
                if (useRegex)
                    try
                    {
                        var pattern = BuildLRPattern(left, right);
                        var mc = Regex.Matches(partial, pattern);
                        foreach (Match m in mc)
                            list.Add(m.Value);
                    }
                    catch
                    {
                    }
                else
                    try
                    {
                        while (left == string.Empty ||
                               partial.Contains(left) && (right == string.Empty || partial.Contains(right)))
                        {
                            // Search for left delimiter and Calculate offset
                            pFrom = left == string.Empty ? 0 : partial.IndexOf(left) + left.Length;
                            // Move right of offset
                            partial = partial.Substring(pFrom);
                            // Search for right delimiter and Calculate length to parse
                            pTo = right == string.Empty ? partial.Length - 1 : partial.IndexOf(right);
                            // Parse it
                            var parsed = partial.Substring(0, pTo);
                            list.Add(parsed);
                            // Move right of parsed + right
                            partial = partial.Substring(parsed.Length + right.Length);
                        }
                    }
                    catch
                    {
                    }
            }

            // Non-recursive
            else
            {
                if (useRegex)
                {
                    var pattern = BuildLRPattern(left, right);
                    var mc = Regex.Matches(partial, pattern);
                    if (mc.Count > 0) list.Add(mc[0].Value);
                }
                else
                {
                    try
                    {
                        pFrom = left == string.Empty ? 0 : partial.IndexOf(left) + left.Length;
                        partial = partial.Substring(pFrom);
                        pTo = right == string.Empty ? partial.Length : partial.IndexOf(right);
                        list.Add(partial.Substring(0, pTo));
                    }
                    catch
                    {
                    }
                }
            }

            return list;
        }
    }
}