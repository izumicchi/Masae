using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Masae.Modules.Owner
{
    public class Owner : ModuleBase<SocketCommandContext>
    {
        [Command("Shutdown"), Alias("sd"), RequireOwner]
        [Summary("Shuts down the bot.")]
        public async Task Shutdown()
        {
            var shutdownEmbed = new EmbedBuilder();
            shutdownEmbed.WithDescription("Shutting down.")
                .WithColor(new Color(45, 205, 110));
            await Context.Channel.SendMessageAsync("", false, shutdownEmbed.Build());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Shutting down..");
            Console.ResetColor();
            Environment.Exit(0);
        }

        [Command("Send"), RequireOwner]
        [Summary("Sends a DM to a user.")]
        public async Task Send(SocketGuildUser user, [Remainder] string message = null)
        {
            if (message == null)
            {
                var errorEmbed = new EmbedBuilder();
                errorEmbed.WithDescription($"Specify a message to send to {user.Mention}.")
                    .WithColor(Color.Red);
                await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
            }
            else
            {
                var sendembed = new EmbedBuilder();
                sendembed.AddField("Message From", $"{Context.Message.Author.Mention}")
                    .AddField("Message", $"{message}")
                    .WithColor(new Color(45, 205, 110));
                var confirmEmbed = new EmbedBuilder();
                confirmEmbed.WithDescription($"Sucessfully sent the message to {user.Mention}").WithColor(new Color(45, 205, 110));
                await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync("", false, sendembed.Build());
                await Context.Channel.SendMessageAsync("", false, confirmEmbed.Build());
            }
        }

        [Command("Setgame"), Alias("setg"), RequireOwner]
        [Summary("Sets the game")]
        public async Task Setgame([Remainder] string game = null)
        {
            if (game == null)
            {
                var errorEmbed = new EmbedBuilder();
                errorEmbed.WithDescription("Specify a game.").WithColor(Color.Red);
                await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
            }
            else
            {
                var successEmbed = new EmbedBuilder();
                successEmbed.WithDescription($"Successfully set the game to {Format.Bold(game)}.").WithColor(new Color(45, 205, 110));
                await Context.Client.SetGameAsync(game);
                await Context.Channel.SendMessageAsync("", false, successEmbed.Build());
            }
        }

        [Command("Changename"), Alias("cn"), RequireOwner]
        [Summary("Changes the bot's name.")]
        public async Task NameChange([Remainder] string new_name = null)
        {
            if (new_name != null)
            {
                try
                {
                    await Context.Client.CurrentUser.ModifyAsync(x =>
                    {
                        x.Username = new_name;
                    });
                    var embed = new EmbedBuilder();
                    embed.WithDescription($"Successfully changed name to **{new_name}**.").WithColor(new Color(45, 205, 110));
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{e}");
                    Console.ResetColor();
                }
            }
            else
            {
                var errorembed = new EmbedBuilder();
                errorembed.WithDescription("The name cannot be blank!").WithColor(new Color(255, 0, 0));
            }
        }

        [Command("Mods")]
        public async Task MODERATOR()
        {
            var online_mods = new List<string>();
            var idle_mods = new List<string>();
            var dnd_mods = new List<string>();
            var offline_mods = new List<string>();
            var streaming_mods = new List<string>();

            string bold = "**";
            string singlelist_on = "";
            string singlelist_id = "";
            string singlelist_dnd = "";
            string singlelist_of = "";
            string singlelist_stre = "";
            string deliminator = ", ";

            foreach (IGuildUser user in Context.Guild.Users)
            {
                if (user.IsBot == true)
                {
                    continue;
                }
                else
                {
                    if (user.GuildPermissions.KickMembers && user.Status == UserStatus.Online)
                    {
                        online_mods.Add(user.Username + "#" + user.Discriminator);
                    }
                    else if (user.GuildPermissions.KickMembers && user.Status == UserStatus.Idle)
                    {
                        idle_mods.Add(user.Username + "#" + user.Discriminator);
                    }
                    else if (user.GuildPermissions.KickMembers && user.Status == UserStatus.DoNotDisturb)
                    {
                        dnd_mods.Add(user.Username + "#" + user.Discriminator);
                    }
                    else if (user.GuildPermissions.KickMembers && user.Status == UserStatus.Offline)
                    {
                        offline_mods.Add(user.Username + "#" + user.Discriminator);
                    }
                }
            }
            
            if (online_mods.Count == 0)
            {
                singlelist_on += "None.";
            }
            foreach (var user in online_mods)
            {
                singlelist_on += bold + user + bold + deliminator;
            }


            if (idle_mods.Count == 0)
            {
                singlelist_id += "None.";
            }
            foreach (var user in idle_mods)
            {
                singlelist_id += bold + user + bold + deliminator;
            }

            if (dnd_mods.Count == 0)
            {
                singlelist_dnd += "None.";
            }
            foreach (var user in dnd_mods)
            {
                singlelist_dnd += bold + user + bold + deliminator;
            }

            if (offline_mods.Count == 0)
            {
                singlelist_of += "None.";
            }
            foreach (var user in offline_mods)
            {
                singlelist_of += bold + user + bold + deliminator;
            }

            if (streaming_mods.Count == 0)
            {
                singlelist_stre += "None.";
            }
            foreach (var user in streaming_mods)
            {
                singlelist_stre += bold + user + bold + deliminator;
            }

            int total_dnd_mods = 0;
            total_dnd_mods = dnd_mods.Count;
            var embed = new EmbedBuilder();
            embed.WithDescription($"{bold}List of available moderators:{bold}\n<:online:473530111463784489> {singlelist_on}\n<:away:473530111661178900> {singlelist_id}\n<:dnd:473530111732219905> {singlelist_dnd}\n<:streaming:473676101705793546> {singlelist_stre}\n<:offline:473530111254069250> {singlelist_of}").WithColor(new Color(45, 205, 110));
            //await Context.Channel.SendMessageAsync("", false, embed.Build());
            await Context.Channel.SendMessageAsync($"{bold}List of available moderators:{bold}\n<:online:473530111463784489> {singlelist_on}\n<:away:473530111661178900> {singlelist_id}\n<:dnd:473530111732219905> {singlelist_dnd}\n<:streaming:473676101705793546> {singlelist_stre}\n<:offline:473530111254069250> {singlelist_of}");
        }

        [Command("file"), RequireOwner]
        public async Task MakeNewFile([Remainder] string name)
        {
            System.IO.File.Create($"Services/{name}");
            var success = new EmbedBuilder();
            success.WithDescription($"Successfully created file {name}").WithColor(new Color(150, 110, 220));
            await Context.Channel.SendMessageAsync("", false, success.Build());
        }
    }
}