using System.ComponentModel;
using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;
using DotNetEnv;

namespace SpotyBot;
public class DiscordBot{
    private readonly DiscordSocketClient _client; 
    private ulong? channelId;
    private SpotifyService spotifyService;

    public DiscordBot(){
        _client = new DiscordSocketClient();
        _client.MessageReceived += ReceiveMessage;
    }

    public async Task StartAsyncBot(string token){
        await this._client.LoginAsync(Discord.TokenType.Bot, token); 
        await this._client.StartAsync();
    }

    public async Task<string> ReceiveMessage(SocketMessage socketMessage){
        if(socketMessage.Author.IsBot) return null;
        if(channelId == null) SetCahannelId(socketMessage.Channel.Id);

        await Task.CompletedTask;

        return socketMessage.ToString();
    }

    private void SetCahannelId(ulong channelId){
        this.channelId = channelId;
    }
    public async void SendMessageAsync(string message)
    {
        var channel = _client.GetChannel(channelId.Value) as IMessageChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }


} 
/// notes to self
/// would it make more sense to place the interaction between spotify and discord in the Main of the program or
/// have the Discord bot have a spotfy bot within it. 