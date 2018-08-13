using Discord;
using Discord.Commands;
using System;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using SixLabors.ImageSharp.Primitives;
using SixLabors.ImageSharp.Formats;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Discord.WebSocket;
using System.Buffers.Text;
using System.Text;
using Masae.Services;

namespace Masae.Modules.XP
{
    public class XP : ModuleBase<SocketCommandContext>
    {
        SqliteConnection _dbcon = new SqliteConnection($"Data Source=Data/Database/Masae.db");

        public string nameof = "Masae";
        public ulong otherid = 0;
        public ulong yourid = 0;
        public int otherxp = 0;
        public int yourxp = 0;
        public int otherlevel = 0;
        public int yourlevel = 0;
        public string demo_desc = "";
        public string got_desc = "";
        public int basexp = 35;

        public void CheckOtherXP()
        {
            CheckOther();
        }

        void CheckOther()
        {
            _dbcon.Open();

            var GetOthersXP = _dbcon.CreateCommand();
            GetOthersXP.CommandText = $"SELECT * FROM XPStats WHERE UserID = {otherid}";
            SqliteDataReader ReadOtherXP = GetOthersXP.ExecuteReader();
            ReadOtherXP.Read();
            otherxp = Convert.ToInt16(ReadOtherXP.GetValue(1));
            otherlevel = Convert.ToInt16(ReadOtherXP.GetValue(2));
        }

        public void CheckSelfXP()
        {
            CheckSelf();
        }

        void CheckSelf()
        {
            _dbcon.Open();

            var ReadOwn = _dbcon.CreateCommand();
            ReadOwn.CommandText = $"SELECT XP, LEVEL FROM XPStats WHERE UserID = {yourid}";
            SqliteDataReader ReadOwnXP = ReadOwn.ExecuteReader();
            ReadOwnXP.Read();
            yourxp = Int32.Parse(ReadOwnXP.GetValue(0).ToString());
            yourlevel = Int32.Parse(ReadOwnXP.GetValue(1).ToString());
        }

        public void MakeSelfIfNone()
        {
            MakeSelf();
        }

        void MakeSelf()
        {
            _dbcon.Open();

            var CreateXP = _dbcon.CreateCommand();
            CreateXP.CommandText = $"INSERT INTO XPStats (UserID, XP, LEVEL) VALUES ({yourid}, 0, 1)";
            CreateXP.ExecuteNonQuery();
        }


        [Command("XP"), Alias("exp", "experience")]
        public async Task CheckXP(IUser user = null)
        {
            if (user == null)
            {
                try
                {
                    yourid = Context.Message.Author.Id;
                    CheckSelfXP();
                    var required = (int)(basexp / 4 * yourlevel);
                    var current = (int)(yourxp);
                    var b = Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 128);

                    int namefont_size = 20;

                    Font namefont = SystemFonts.CreateFont("Arial", namefont_size, FontStyle.Bold);

                    if (Context.Message.Author.ToString().Length >= 5)
                    {
                        namefont_size = 17;
                    }
                    else if (Context.Message.Author.ToString().Length >= 10)
                    {
                        namefont_size = 15;
                    }
                    else if (Context.Message.Author.ToString().Length >= 15)
                    {
                        namefont_size = 13;
                    }
                    else if (Context.Message.Author.ToString().Length >= 20)
                    {
                        namefont_size = 11;
                    }
                    else if (Context.Message.Author.ToString().Length >= 25)
                    {
                        namefont_size = 9;
                    }
                    else if (Context.Message.Author.ToString().Length >= 30)
                    {
                        namefont_size = 7;
                    }

                    Font levelfont = SystemFonts.CreateFont("Arial", 22, FontStyle.Bold);
                    Font xpfont = SystemFonts.CreateFont("Arial", 22, FontStyle.Bold);
                    Font descfont = SystemFonts.CreateFont("Arial", 16, FontStyle.Bold);
                    if (System.IO.File.Exists($"Data/Images/Avatars/{Context.Message.Author.Id}_XPAvatar.png"))
                    {
                        //empty
                    }
                    else
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(b, $@"Data/Images/Avatars/{Context.Message.Author.Id}_XPAvatar.png");
                        }
                    }

                    using (Image<Rgba32> img = Image.Load(@"Data/Images/XP_Card.png"))
                    using (Image<Rgba32> imaaa = Image.Load($@"Data/Images/Avatars/{Context.Message.Author.Id}_XPAvatar.png"))
                    using (Image<Rgba32> badge = Image.Load($@"Data/Images/Badges/BetaBadge.png"))
                    using (Image<Rgba32> xpimg = new Image<Rgba32>(800, 400))
                    {
                        img.Mutate(ctx => ctx.DrawText($"@{Context.Message.Author.ToString()}", namefont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 700, img.Height - 300)));
                        //img.Mutate(ctx => ctx.DrawText($"{got_desc}", descfont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 755, img.Height - 195)));
                        img.Mutate(ctx => ctx.DrawText($"{yourlevel}", levelfont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 430, img.Height - 360)));
                        img.Mutate(ctx => ctx.DrawText($"{current}/{required}", xpfont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 430, img.Height - 290)));
                        imaaa.Mutate(ctx => ctx.Resize(new SixLabors.Primitives.Size(75, 75)));
                        badge.Mutate(ctx => ctx.Resize(new SixLabors.Primitives.Size(35, 35)));

                        xpimg.Mutate(ctx => ctx
                        .DrawImage(img, 1f, new SixLabors.Primitives.Point(0, 0))
                        .DrawImage(imaaa, 1f, new SixLabors.Primitives.Point(131, 17))
                        .DrawImage(badge, 1f, new SixLabors.Primitives.Point(45, 330))
                        );

                        xpimg.Save($@"Data/Images/{Context.Message.Author.Id}_XP.png");
                    }

                    await Context.Channel.TriggerTypingAsync().ConfigureAwait(false);
                    await Context.Channel.SendFileAsync($@"Data/Images/{Context.Message.Author.Id}_XP.png", $"{Context.Message.Author.Mention}, here's your XP card!");
                }
                catch
                {
                    var error = new EmbedBuilder();
                    error.WithDescription($"{Context.Message.Author.Mention}, you don't have XP stats!\nI automatically made them for you, say `.xp` again to check your XP stats!").WithColor(new Color(255, 0, 0));
                    MakeSelfIfNone();
                    await Context.Channel.SendMessageAsync("", false, error.Build());
                }
            }
            else
            {
                if (user.IsBot == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bots don't have XP stats!");
                    Console.ResetColor();
                    var error = new EmbedBuilder();
                    error.WithDescription($"{Context.Message.Author.Mention}, Bots don't have XP stats!").WithColor(new Color(255, 0, 0));
                    await Context.Channel.SendMessageAsync("", false, error.Build());
                }
                else
                {
                    try
                    {
                        otherid = user.Id;
                        CheckOtherXP();
                        var required = (int)(basexp / 4 * otherlevel);
                        var current = (int)(otherxp);
                        var b = user.GetAvatarUrl(ImageFormat.Png, 128);

                        int namefont_size = 20;

                        Font namefont = SystemFonts.CreateFont("Arial", namefont_size, FontStyle.Bold);

                        if (user.ToString().Length >= 5)
                        {
                            namefont_size = 17;
                        }
                        else if (user.ToString().Length >= 10)
                        {
                            namefont_size = 15;
                        }
                        else if (user.ToString().Length >= 15)
                        {
                            namefont_size = 13;
                        }
                        else if (user.ToString().Length >= 20)
                        {
                            namefont_size = 11;
                        }
                        else if (user.ToString().Length >= 25)
                        {
                            namefont_size = 9;
                        }
                        else if (user.ToString().Length >= 30)
                        {
                            namefont_size = 7;
                        }

                        Font levelfont = SystemFonts.CreateFont("Arial", 22, FontStyle.Bold);
                        Font xpfont = SystemFonts.CreateFont("Arial", 22, FontStyle.Bold);
                        Font descfont = SystemFonts.CreateFont("Arial", 16, FontStyle.Bold);
                        if (System.IO.File.Exists($"Data/Images/Avatars/{user.Id}_XPAvatar.png"))
                        {
                            //empty
                        }
                        else
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(b, $@"Data/Images/Avatars/{user.Id}_XPAvatar.png");
                            }
                        }

                        using (Image<Rgba32> img = Image.Load(@"Data/Images/XP_Card.png"))
                        using (Image<Rgba32> imaaa = Image.Load($@"Data/Images/Avatars/{user.Id}_XPAvatar.png"))
                        using (Image<Rgba32> badge = Image.Load($@"Data/Images/Badges/BetaBadge.png"))
                        using (Image<Rgba32> xpimg = new Image<Rgba32>(800, 400))
                        {
                            img.Mutate(ctx => ctx.DrawText($"@{user.ToString()}", namefont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 700, img.Height - 300)));
                            img.Mutate(ctx => ctx.DrawText($"{otherlevel}", levelfont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 430, img.Height - 360)));
                            img.Mutate(ctx => ctx.DrawText($"{current}/{required}", xpfont, Rgba32.Black, new SixLabors.Primitives.PointF(img.Width - 430, img.Height - 290)));
                            imaaa.Mutate(ctx => ctx.Resize(new SixLabors.Primitives.Size(75, 75)));
                            badge.Mutate(ctx => ctx.Resize(new SixLabors.Primitives.Size(35, 35)));

                            xpimg.Mutate(ctx => ctx
                            .DrawImage(img, 1f, new SixLabors.Primitives.Point(0, 0))
                            .DrawImage(imaaa, 1f, new SixLabors.Primitives.Point(131, 17))
                            .DrawImage(badge, 1f, new SixLabors.Primitives.Point(45, 330))
                            );

                            xpimg.Save($@"Data/Images/{user.Id}_XP.png");
                        }

                        await Context.Channel.TriggerTypingAsync().ConfigureAwait(false);
                        await Context.Channel.SendFileAsync($@"Data/Images/{user.Id}_XP.png", $"{Context.Message.Author.Mention}, here's {user.Username + "#" + user.Discriminator}'s XP card!");
                    }
                    catch
                    {
                        var error = new EmbedBuilder();
                        error.WithDescription($"{Context.Message.Author.Mention}, {user.Mention} doesn't have XP stats!").WithColor(new Color(255, 0, 0));
                        await Context.Channel.SendMessageAsync("", false, error.Build());
                    }
                }
            }
        }
    }
}