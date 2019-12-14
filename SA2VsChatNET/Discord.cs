using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SA2VsChatNET
{
    public class Discord
    {
        public static DiscordSocketClient DiscordClient;

        public static void StartDiscord(Func<SocketMessage, Task> callback)
        {
            new Discord().MainAsync(callback).GetAwaiter().GetResult();
        }
        public static string TempTokenToLoad = "";
        public async Task MainAsync(Func<SocketMessage, Task> callback)
        {
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 2000,
                AlwaysDownloadUsers = true
            });

            // Make sure to keep this private
            string token = TempTokenToLoad;
            
            await DiscordClient.LoginAsync(TokenType.Bot, token);
            await DiscordClient.StartAsync();

            DiscordClient.MessageReceived += callback;

            DiscordClient.Ready += () =>
            {
                Console.WriteLine("[Discord] Ready!");
                return Task.CompletedTask;
            };


        }
    }
}