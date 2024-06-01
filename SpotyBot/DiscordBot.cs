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

    public DiscordBot(SpotifyService spotifyService){
        _discordClient = new DiscordSocketClient();
        spotifyService = spotifyService;
        _discordClient.MessageReceived += ReceiveMessage;
    }

    public async Task StartAsyncBot(string token){
        await this._discordClient.LoginAsync(Discord.TokenType.Bot, token); 

        await this._discordClient.StartAsync();
    }

    public async void  MessageHandler(SocketMessage message){
        if(message.Author.IsBot) return;

        //set the channel id
        if (channelId == null) SetCahannelId(message.Channel.Id);

        var trackId = _spotifyService.ExtractIDFromURL(message.ToString());

        if (trackId == string.Empty) { SendMessageAsync("Thats not a song "); return; }

        var trackInfo = await _spotifyService.GetTrackByID(trackId);

        SendMessageAsync(trackInfo.Name.ToString());
    }
    
    public async Task<string> ReceiveMessage(SocketMessage socketMessage){
        if(socketMessage.Author.IsBot) return null;

        await Task.CompletedTask;

        return socketMessage.ToString();
    }

    private void SetCahannelId(ulong channelId){
        this.channelId = channelId;
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
/// notes to self
/// would it make more sense to place the interaction between spotify and discord in the Main of the program or
/// have the Discord bot have a spotfy bot within it. 