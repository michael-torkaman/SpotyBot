using Discord;
using Discord.WebSocket;
namespace SpotyBot{
    internal class Program{
        private readonly  DiscordSocketClient client; 


        private const string token = "";
        
        public Program()
        {
            this.client = new DiscordSocketClient();
        }


        static void Main(string[] args) {
            Console.WriteLine("Hello World");
        }
        
        public async Task StartBotAsync(){
        }

    }
}

