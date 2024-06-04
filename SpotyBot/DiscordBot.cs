using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Discord;
using Discord.WebSocket;
using DotNetEnv;

namespace SpotyBot;
public class DiscordBot{
    private readonly DiscordSocketClient _discordClient; 
    private ulong? channelId;
    private SpotifyService _spotifyService;

    public DiscordBot(SpotifyService spotifyService)
    {
        _discordClient = new DiscordSocketClient(new DiscordSocketConfig{
            LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });
        _spotifyService = spotifyService;

        _discordClient.MessageReceived += MessageHandler;
        _discordClient.Log += Log;
    }

    public async Task StartAsyncBot(string token){
        await this._discordClient.LoginAsync(Discord.TokenType.Bot, token); 

        await this._discordClient.StartAsync();
    }

    public async Task SimpleMessageHandler(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (channelId == null) SetCahannelId(message.Channel.Id);

        // Log received message
        Console.WriteLine($"Received message: {message.Content}");
        Console.WriteLine($"Author: {message.Author}");
        Console.WriteLine($"Channel: {message.Channel.Name}");
        // Echo the message back to the channel
        await SendMessageAsync($"Echo: {message.Content}");

    }

    public async Task MessageHandler(SocketMessage message){
        if(message.Author.IsBot) return;

        //set the channel id
        if (channelId == null) SetCahannelId(message.Channel.Id);

        Console.WriteLine($"Received message: {message.Author}");

        var trackId = _spotifyService.ExtractIDFromURL(message.Content);

        if (trackId == string.Empty) { await SendMessageAsync("Thats not a song "); return; }

        var trackInfo = await _spotifyService.GetTrackByID(trackId);

        await SendMessageAsync(await TrackInfoToString(trackInfo));

        if(await _spotifyService.UserHasPlaylist() == false){

            await _spotifyService.CreateNewPublicPlaylist();
        }
        
    }

    public async Task<string> TrackInfoToString(SpotifyAPI.Web.FullTrack track){
        var sb = new StringBuilder();

        sb.Append("Track name: " + track.Name.ToString() + "\n"); 
        sb.Append("Artist: " + track.Album.Name.ToString());
        return sb.ToString();
    }

    private Task Log(LogMessage logMessage)
    {
        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }


    private void SetCahannelId(ulong _channelId){
        this.channelId = _channelId;
    }

    public async Task SendMessageAsync(string message)
    {
        var channel = _discordClient.GetChannel(channelId.Value) as IMessageChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }
} 