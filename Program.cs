using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Masae.Services;
using Microsoft.Data.Sqlite;

namespace Masae
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private DbService _db;
        //private XP_Service _xp;


        public async Task Start()
        {
            Console.Title = $"Masae";
            _db = new DbService();
            _client = new DiscordSocketClient();
            _db.CreateTables();
            _db.GetPrefix();
            _handler = new CommandHandler(_client);
            //_xp = new XP_Service(_client);

            //try
            //{
            //    if (System.IO.File.Exists($"{_db.nameof}.db"))
            //    {
            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        Console.WriteLine("Database already exists, not created!");
            //        Console.ResetColor();
            //    }
            //    else
            //    {
            //        _db.DatabaseCreation();
            //    }
            //    _db.Tables();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"The database {_db.nameof} already exists!");
            //    Console.WriteLine($"{e}");
            //}
            await _client.LoginAsync(TokenType.Bot, "NDc4NjMyMTY5OTM0MTU5ODc4.DlNgew.emnsMh_kVQ3K2sKg2bcEEQsU-wc");
            Console.ForegroundColor = ConsoleColor.White;
            await _client.StartAsync();
            Console.WriteLine("Logged in!");
            Console.ResetColor();
            await Task.Delay(-1);
        }
    }
}