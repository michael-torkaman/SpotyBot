using System.Runtime.CompilerServices;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;

namespace SpotyBot{
    internal class Program{
       
        static async Task Main(string[] args)
        {
            // load .env files containing discord token   
            DotNetEnv.Env.Load();

            // var DiscordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            // if(DiscordToken == null) return;

            // DiscordBot discordBot = new DiscordBot();
            // await discordBot.StartAsyncBot(DiscordToken);
            // await Task.Delay(-1);

            var spotifyClientId =  Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_ID");
            var spotifyClientSecret =  Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_SECRET");

            if(spotifyClientId == null || spotifyClientSecret == null) return;

            SpotifyBot spotifyBot = new SpotifyBot(spotifyClientId, spotifyClientSecret);
            spotifyBot.GetTrackByID("1s6ux0lNiTziSrd7iUAADH");
            await Task.Delay(-1);
        }
    }
}