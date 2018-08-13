using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Masae.Services;
using System.Collections.Concurrent;
using System.Threading;

namespace Masae.Services
{
    public class CommandHandler
    {
        public string Prefix = "";
        public string nameof = "Masae";
        public ulong id = 0;
        private DiscordSocketClient _client;
        private CommandService _service;
        private DbService _db;


        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommands;
        }

        private async Task HandleCommands(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            JObject o1 = JObject.Parse(File.ReadAllText(@"Data/credentials.json"));
            using (StreamReader file = File.OpenText(@"Data/credentials.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                Prefix = o2[$"Config"]["Prefix"].ToString();
            }

            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasStringPrefix(Prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                if (msg.Author.IsBot == true)
                {
                    return;
                }
                else
                {
                    var result = await _service.ExecuteAsync(context, argPos);
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        var errorEmbed = new EmbedBuilder();
                        errorEmbed.WithDescription(result.ErrorReason).WithColor(new Color(255, 0, 0));
                        await context.Channel.SendMessageAsync("", false, errorEmbed.Build());
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result.ErrorReason);
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}