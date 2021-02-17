using DiscordRPC;

namespace BoltCUI
{
    internal class DiscordRPC1
    {
        public static DiscordRpcClient client;

        public static void Initialize()
        {
            client = new DiscordRpcClient("781066895029436456");
            client.Initialize();
            client.SetPresence(new RichPresence
            {
                Details = "discord.gg/A2jCUZsAYC",
                State = "Bolt AIO",
                Timestamps = Timestamps.Now,
                Assets = new Assets
                {
                    LargeImageKey = "bolt_logo",
                    LargeImageText = "discord.gg/A2jCUZsAYC"
                }
            });
        }
    }
}