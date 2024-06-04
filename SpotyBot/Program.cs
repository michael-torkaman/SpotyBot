using System.Runtime.CompilerServices;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SpotyBot
{
    public class Program
    {
        // SpotifyApi supplied server to validate client credentials
        private static EmbedIOAuthServer _server;
        
        //client secrets and id's for both spotify and discord
        private static string discordToken;
        private static string spotifyClientId;
        private static string spotifyClientSecret;
        private static string spotifyRedirectUri;
        
        //spotify user session
        private static SpotifyService _spotifyClient;
        
        // discord bot
        private static DiscordBot _discordBot;

        static async Task Main(string[] args)
        {
            // Load .env file containing Discord token
            DotNetEnv.Env.Load();

            // Discord bot token
            discordToken = Environment.GetEnvironmentVariable("DISCORDBOT_TOKEN");
            if (discordToken == null)
            {
                Console.WriteLine("Discord token not found");
                return;
            }

            // Setting Spotify client_id and client_secret and redirect uri
            spotifyClientId = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_ID");
            spotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_SECRET");
            spotifyRedirectUri = Environment.GetEnvironmentVariable("SPOTIFYSERVICE_URI");

            if (spotifyClientId == null || spotifyClientSecret == null || spotifyRedirectUri == null)
            {
                Console.WriteLine("Spotify credentials not found");
                return;
            }

            _server = new EmbedIOAuthServer(new Uri(spotifyRedirectUri), 5543);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, spotifyClientId, LoginRequest.ResponseType.Code)
            {
                //scopes for spotify session 
                Scope = new List<string> {
                    Scopes.PlaylistModifyPublic,
                    Scopes.PlaylistModifyPrivate,
                    Scopes.PlaylistReadPrivate,
                    Scopes.UserLibraryRead,
                    Scopes.UserLibraryModify
                }
            };
            BrowserUtil.Open(request.ToUri());

            await Task.Delay(-1);
        }

        /// <summary>
        /// Oauth for spotify client. On successful retrieval of token initializes
        /// Spotify Client session and starts discord bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();

            var config = SpotifyClientConfig.CreateDefault();
            var tokenResponse = await new OAuthClient(config).RequestToken(
                new AuthorizationCodeTokenRequest(
                    spotifyClientId, spotifyClientSecret, response.Code, new Uri(spotifyRedirectUri)
                )
            );
            
            _spotifyClient = new SpotifyService(tokenResponse.AccessToken);
            
            _discordBot = new DiscordBot(_spotifyClient);
            await _discordBot.StartAsyncBot(discordToken);
        }

        /// <summary>
        /// error handlilng for Oauth authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        /// <param name="state"></param>
        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }
    }
}