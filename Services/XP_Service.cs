﻿using Discord;
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
        public Task GiveXpTask;

        public ulong id = 0;
        public int basexp = 35;
        public ulong ran_by_id = 0;
        public int got_from_db_xp = 0;
        public int got_from_db_level = 0;
        public int level_up = 0;

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
            ran_by_id = msg.Author.Id;
            _db.ran_by_id = ran_by_id;
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

                            if (time.TotalMilliseconds < 0)
                                time = TimeSpan.FromMinutes(1);

                            var timer = new Timer(UpdateXp, s, (int)time.TotalMilliseconds, Timeout.Infinite);
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