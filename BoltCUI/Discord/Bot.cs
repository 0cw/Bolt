using System.Threading;
using System.Threading.Tasks;
using BoltCUI;
using BoltCUI.Tools;
using Colorful;
using Discord;
using Discord.WebSocket;
using Color = System.Drawing.Color;

namespace Bolt_AIO
{
    internal class Bot
    {
        public static string botToken = "";
        public static string Prefix = "";
        public static bool BotRunning = false;
        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await _client.SetGameAsync($"{Prefix}help");
            Console.Write("    [", Color.White);
            Console.Write("Starting Bot...", Color.Purple);
            Console.Write("]\n", Color.White);
            Thread.Sleep(250);
            Console.Write("    [", Color.White);
            Console.Write("Bot Started! Redirecting to Settings Menu!", Color.Purple);
            Console.Write("]\n", Color.White);
            Thread.Sleep(250);
            SettingsTools.Settings0();
            await Task.Delay(-1);
        }

        private static Task CommandHandler(SocketMessage message)
        {
            var command = "";
            var lengthOfCommand = -1;

            if (!message.Content.StartsWith(Prefix))
                return Task.CompletedTask;

            if (message.Author.IsBot)
                return Task.CompletedTask;

            if (message.Content.Contains(" "))
                lengthOfCommand = message.Content.IndexOf(' ');
            else
                lengthOfCommand = message.Content.Length;

            command = message.Content.Substring(1, lengthOfCommand - 1).ToLower();

            if (command.Equals("stats"))
            {
                var DiscordREALID = message.Author.Id.ToString();
                if (Settings.DiscordID == DiscordREALID)
                {
                    var builder = new EmbedBuilder();
                    builder.WithTitle("Bolt AIO • Stats ");
                    builder.WithDescription(
                        $"Hits: {Program.Hits}\nFrees: {Program.Frees}\nOthers: {Program.Others}\nFails: {Program.Fails}\nErrors: {Program.Errors}\nChecked: {Program.TotalChecks} / {Program.Combostotal}\n");
                    builder.WithColor(Discord.Color.Purple);
                    builder.WithFooter("Coded By Quanotics#3931 • Bolt Discord -> https://discord.gg/A2jCUZsAYC",
                        "https://cdn.discordapp.com/attachments/708528132327735427/781294769375674378/Bolt_Logo.png");
                    message.Channel.SendMessageAsync("", false, builder.Build());
                }
            }

            if (command.Equals("help"))
            {
                var DiscordREALID = message.Author.Id.ToString();
                if (Settings.DiscordID == DiscordREALID)
                {
                    var builder = new EmbedBuilder();
                    builder.WithTitle("Bolt AIO • Help");
                    builder.WithDescription(
                        "**Help** - Shows a list of Commands!\n**Stats** - Shows Checker Stats. Hits, Fails, ect.");
                    builder.WithColor(Discord.Color.Purple);
                    builder.WithFooter("Coded By Quanotics#3931 • Bolt Discord -> https://discord.gg/A2jCUZsAYC",
                        "https://cdn.discordapp.com/attachments/708528132327735427/781294769375674378/Bolt_Logo.png");
                    message.Channel.SendMessageAsync("", false, builder.Build());
                }
            }

            return Task.CompletedTask;
        }
    }
}