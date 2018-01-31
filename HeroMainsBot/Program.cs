using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HeroMainsBot
{
    class Program
    {
        public static DiscordSocketClient client;

        private CommandService commands;
        private IServiceProvider services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            commands = new CommandService();

            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageRecieved;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            services = new ServiceCollection().AddSingleton(client).AddSingleton(commands).BuildServiceProvider();

            using(StreamReader sr = new StreamReader("botcode.txt"))
            {
                await client.LoginAsync(TokenType.Bot, sr.ReadLine());
            }

            await client.StartAsync();

            string line;
            while(true)
            {
                line = Console.ReadLine();
                if(line == "exit")
                {
                    break;
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }

        private async Task MessageRecieved(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            if(message == null)
                return;
            if(msg.Author.IsBot)
                return;

            int argPos = 0;
            if(msg.HasCharPrefix('-', ref argPos))
            {
                var context = new SocketCommandContext(client, msg);
                var result = await commands.ExecuteAsync(context, argPos, services);
                if(!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}
