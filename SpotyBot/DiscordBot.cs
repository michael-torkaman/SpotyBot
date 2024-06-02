using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        _discordClient = new DiscordSocketClient();
        _spotifyService = spotifyService;

        _discordClient.MessageReceived += MessageHandler;
        _discordClient.Log += Log;
    }

    public async Task StartAsyncBot(string token){
        await this._discordClient.LoginAsync(Discord.TokenType.Bot, token); 

        await this._discordClient.StartAsync();
    }

    public async Task MessageHandler(SocketMessage message){
        if(message.Author.IsBot) return;

        //set the channel id
        if (channelId == null) SetCahannelId(message.Channel.Id);

        Console.WriteLine($"Received message: {message.Content}");

        var trackId = _spotifyService.ExtractIDFromURL(message.Content);

        if (trackId == string.Empty) { SendMessageAsync("Thats not a song "); return; }

        var trackInfo = await _spotifyService.GetTrackByID(trackId);

        SendMessageAsync(trackInfo.Name.ToString());
    }

    private Task Log(LogMessage logMessage)
    {
        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }


    private void SetCahannelId(ulong _channelId){
        this.channelId = _channelId;
    }

    public async void SendMessageAsync(string message)
    {
        var channel = _discordClient.GetChannel(channelId.Value) as IMessageChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }
} 