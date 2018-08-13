using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Masae;
using Masae.Modules;
using Masae.Services;

namespace Masae.Modules.Help
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        DbService _db;

        [Command("Help"), Alias("h")]
        [Summary("Shows information about a command.")]
        public async Task HelpCMD([Remainder] string command = null)
        {
            var helpEmbed = new EmbedBuilder();
            helpEmbed.WithAuthor(Context.Message.Author.ToString(), Context.Message.Author.GetAvatarUrl().ToString())
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl().ToString())
                .WithTitle(Context.Client.CurrentUser.Username + " Beta")
                .WithDescription($"Hello there, I'm {Context.Client.CurrentUser.Username}, a bot developed by <@!145878866429345792> in C#!\nThank you for using me. :smile:\n\nPrefix: **{_db.prefix_from_db}** / **{Context.Client.CurrentUser.Mention}**\nModules: **Help, XP**\n**{_db.prefix_from_db}cmds <module>**, to see all commands in that module.\n\n[Add me](https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot&permissions=66186303) to your server!\n\n")
                .WithTimestamp(System.DateTimeOffset.UtcNow)
                .WithColor(new Color(45, 205, 110));

            if (command == null)
            {
                var successEmbed = new EmbedBuilder();
                successEmbed.WithDescription($"{Context.Message.Author.Mention}, check your DMs!").WithColor(new Color(45, 205, 110));
                try
                {
                    await (await Context.Message.Author.GetOrCreateDMChannelAsync()).SendMessageAsync("", false, helpEmbed.Build());
                    await Context.Channel.SendMessageAsync("", false, successEmbed.Build());
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("", false, helpEmbed.Build());
                }
            }
            else
            {
                try
                {
                    string tit = "";
                    string desc = "";
                    string usg = "";
                    string f1 = "";
                    string f2 = "";

                    JObject o1 = JObject.Parse(File.ReadAllText(@"Data/responses.json"));
                    using (StreamReader file = File.OpenText(@"Data/responses.json"))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        tit = o2[$"{command}"]["Title"].ToString();
                        desc = o2[$"{command}"]["Description"].ToString();
                        usg = o2[$"{command}"]["Usage1"].ToString();
                        f1 = o2[$"{command}"]["UPerms1"].ToString();
                        f2 = o2[$"{command}"]["BPerms1"].ToString();
                    }

                    var successEmbed = new EmbedBuilder();
                    successEmbed.WithTitle($"{tit}")
                        .WithDescription($"{desc}")
                        .AddField("Usage", $"{usg}")
                        .AddField("User Permissions", $"{f1}", true)
                        .AddField("Bot Permissions", $"{f2}", true)
                        .WithColor(new Color(45, 205, 110));
                    await Context.Channel.SendMessageAsync("", false, successEmbed.Build());
                }
                catch (System.NullReferenceException)
                {
                    var errorEmbed = new EmbedBuilder();
                    errorEmbed.WithDescription($"Command not found.").WithColor(Color.Red);
                    await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
                }
            }
        }

        [Command("commands"), Alias("cmds", "cmd")]
        public async Task ShowAllModules([Remainder] string module = null)
        {
            if (module == null)
            {
                var error = new EmbedBuilder();
                error.WithDescription($"You need to specify a module to see it's commands.\n\nAvailable Modules:\nXP,\nHelp").WithColor(new Color(45, 205, 110));
                await Context.Channel.SendMessageAsync("", false, error.Build());
            }
            else
            {
                try
                {
                    string CommandsFromModule = "";

                    JObject o1 = JObject.Parse(File.ReadAllText(@"Data/responses.json"));
                    using (StreamReader file = File.OpenText(@"Data/responses.json"))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        CommandsFromModule = o2[$"{module}"]["Title"].ToString();
                    }

                    var successEmbed = new EmbedBuilder();
                    successEmbed.WithAuthor(Context.Message.Author.ToString(), Context.Message.Author.GetAvatarUrl(ImageFormat.Png).ToString())
                        .WithDescription($"Here are all available commands in the module **{module}**:\n\n**-** {CommandsFromModule}")
                        .WithColor(new Color(45, 205, 110));
                    await Context.Channel.SendMessageAsync("", false, successEmbed.Build());
                }
                catch (System.NullReferenceException)
                {
                    var errorEmbed = new EmbedBuilder();
                    errorEmbed.WithDescription($"Module not found.").WithColor(new Color(255, 0, 0));
                    await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
                }
            }
        }
    }
}