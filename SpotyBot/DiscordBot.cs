using System.ComponentModel;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using DotNetEnv;

namespace SpotyBot;
public class DiscordBot{
    private readonly DiscordSocketClient client; 

    public DiscordBot(){
        client = new DiscordSocketClient();
        client.MessageReceived += ReceiveMessage;
    }

    public async Task StartAsyncBot(string token){
        await this.client.LoginAsync(Discord.TokenType.Bot, token); 
        await this.client.StartAsync();
    }

    public async Task ReceiveMessage(SocketMessage socketMessage){
        if(socketMessage.Author.IsBot) return;

        await Task.CompletedTask;
        await socketMessage.Channel.SendMessageAsync("Hello Friend");
    }
} 