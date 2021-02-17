using System;
using System.Diagnostics;
using System.Threading;
using Discord;
using Discord.Gateway;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace BoltCUI.Tools
{
    public class Sniper
    {
        public static void Sniper0()
        {
            Console.Title =
                "                                                                                                      [>] BoltAIO | Nitro Sniper | godpow#7468 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            Console.Write("                                            [", Color.White);
            Console.Write("Created By ", Color.White);
            Console.Write("godpow#7468", Color.Purple);
            Console.Write("] \n", Color.White);
            System.Console.WriteLine();
            System.Console.WriteLine();
            Program.prefix("Enter Discord Token", "\n");
            Program.prefix("X", "Go Back\n");
            Program.prefix(">", "");
            var token = Console.ReadLine().ToLower();

            switch (token)
            {
                case "x":
                    Program.Menu0();
                    break;
                default:
                    var client = new DiscordSocketClient();
                    try
                    {
                        client.Login(token);
                        client.OnLoggedIn += Client_OnLoggedIn;
                        Thread.Sleep(-1);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid token :/", Color.Purple);
                        Sniper0();
                    }
                    break;

            }
        }

        private static void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            Console.Write("    [", Color.Purple);
            Console.Write("Username", Color.White);
            Console.Write("] ", Color.Purple);
            Console.Write(client.User.Username + Environment.NewLine, Color.White);


            Console.Write("    [", Color.Purple);
            Console.Write("ID", Color.White);
            Console.Write("] ", Color.Purple);
            Console.Write(client.User.Id + Environment.NewLine, Color.White);

            Console.Write("    [", Color.Purple);
            Console.Write("Email", Color.White);
            Console.Write("] ", Color.Purple);
            Console.Write(client.User.Email + Environment.NewLine, Color.White);


            Console.Write("    [", Color.Purple);
            Console.Write("Creation Date", Color.White);
            Console.Write("] ", Color.Purple);
            Console.Write(client.User.CreatedAt + Environment.NewLine, Color.White);

            Console.Write(Environment.NewLine + Environment.NewLine + Environment.NewLine);
            client.OnMessageReceived += Client_OnMessageReceived;
            Thread.Sleep(-1);
        }

        private static void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs msg)
        {
            var message = msg.Message.Content;
            if (!message.Contains("gift/")) return;
            var gift = Removebefore(message);
            var finalgift = gift.Split()[0];
            finalgift = finalgift.Replace("gift/", "");

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                client.RedeemGift(finalgift);
                sw.Stop();
                Console.Write("    [", Color.Purple);
                Console.Write(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                    Color.White);
                Console.Write("] ", Color.Purple);
                Console.Write("Successfully redeemed gift  - discord.gift/" + finalgift + " (" +
                              sw.ElapsedMilliseconds + " ms)", Color.White);
            }
            catch (Exception)
            {
                try
                {
                    if (client.GetGift(finalgift).Consumed)
                    {
                        Console.Write("    [", Color.Purple);
                        Console.Write(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                            Color.White);
                        Console.Write("] ", Color.Purple);
                        Console.Write("Gift already claimed from - discord.gift/" + finalgift +
                                      Environment.NewLine, Color.White);
                    }
                    else
                    {
                        Console.Write("    [", Color.Purple);
                        Console.Write(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                            Color.White);
                        Console.Write("] ", Color.Purple);
                        Console.Write("Error redeeming gift - discord.gift/" + finalgift + Environment.NewLine,
                            Color.White);
                    }
                }
                catch (Exception)
                {
                    Console.Write("    [", Color.Purple);
                    Console.Write(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                        Color.White);
                    Console.Write("] ", Color.Purple);
                    Console.Write("Unknown Gift - discord.gift/" + finalgift + Environment.NewLine,
                        Color.White);
                }
            }
        }

        private static string Removebefore(string text)
        {
            var orgText = text;
            var i = orgText.IndexOf("gift/", StringComparison.Ordinal);
            if (i != -1) text = orgText.Remove(0, i);
            return text;
        }
    }
}