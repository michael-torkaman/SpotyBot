using System.Runtime.CompilerServices;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;

namespace SpotyBot{
    internal class Program{
       
        static async Task Main(string[] args)
        {
            // load .env files containing discord token   
            DotNetEnv.Env.Load();

            // Discord bot token
            var DiscordToken = Environment.GetEnvironmentVariable("DISCORDBOT_TOKEN");
            if (DiscordToken == null) return;

            // setting spotify client_id and client_secret 
            var spotifyClientId = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_ID");
            var spotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_SECRET");

            if (spotifyClientId == null || spotifyClientSecret == null) return;

            //init discord bot 
            DiscordBot discordBot = new DiscordBot();
            await discordBot.StartAsyncBot(DiscordToken);
            await Task.Delay(-1);

            // init spotify bot
            SpotifyService spotifyBot = new SpotifyService(spotifyClientId, spotifyClientSecret);


        
        //     }

        //     var trackName = spotifyBot.GetTrackByID("1s6ux0lNiTziSrd7iUAADH");
        //     Console.WriteLine(trackName);
        //     await Task.Delay(-1);
        }
    }
}