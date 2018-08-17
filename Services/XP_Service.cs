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
using Microsoft.Data.Sqlite;

namespace Masae.Services
{
    public class XP_Service
    {
        private SqliteConnection _dbcon;
        private DiscordSocketClient _client;
        private CommandService _service;
        private DbService _db;

        public XP_Service(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _db = new DbService();
            _client.MessageReceived += GXP;
        }

        public async Task GXP(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            _db.ran_by_id = msg.Author.Id;
            if (msg.Channel is IDMChannel)
            {
                return;
            }
            else
            {
                if (msg.Author.IsBot || msg.Content.Length <= 20)
                {
                    return;
                }
                else
                {
                    try
                    {
                        if (msg.Author.IsBot)
                        {
                            return;
                        }
                        else
                        {
                            var time = s.Timestamp.UtcDateTime - DateTime.UtcNow;
                            if (time.TotalMilliseconds > int.MaxValue)
                                return;

                            if (time.TotalMinutes < 0)
                                time = TimeSpan.FromMinutes(1);

                            var timer = new Timer(UpdateXp, s, (int)time.TotalMinutes, Timeout.Infinite);
                        }
                    }
                    catch
                    {
                        if (msg.Author.IsBot)
                        {
                            return;
                        }
                        else
                        {
                            _db.MakeSelfIfNone();
                        }
                    }
                }
            }
        }
        
        private void UpdateXp(object xpobj)
        {
            try
            {
                _db.UXP();
            }
            catch
            {
                _db.MakeSelfIfNone();
            }
        }
    }
}
