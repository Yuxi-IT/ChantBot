using ChantBot.Models;
using ChantBot.Types;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChantBot.Commands
{
    internal class TextMessage
    {
        public static async void HanleText(Message msg)
        {
            //try{
            var cmds = msg.Text.Replace($"@{Program._bot.Username}", "").Split(" ");
            switch (cmds[0].ToLower())
            {
                case "/start":
                    if (msg.From.Id == msg.Chat.Id)
                        InfoMenu(msg);
                    else
                        await Program.bot.SendMessage(msg.Chat, "请在与机器人的私聊页面使用该指令.", ParseMode.Html);
                    break;
                case "/info":
                    InfoMenu(msg);
                    break;
                case "/help":
                    await Program.bot.SendMessage(msg.Chat, PlayerType.GetInfoMenu(PlayerType.GetPlayerById(msg.From.Id.ToString())), ParseMode.Html, replyMarkup: ReplyMarkupModels.HelpMenu(msg.Chat.Id.ToString(), msg.From.Id.ToString()));
                    break;
                case "/admin":
                    AdminMenu(msg, cmds);
                    break;
                case "/addre":
                    if(cmds.Length < 2)
                        await Program.bot.SendMessage(msg.Chat, "正确用法：<code>/addre [USDT收款地址]</code>.", ParseMode.Html);
                    else
                    {
                        var user = PlayerType.GetPlayerById(msg.From.Id.ToString());
                        user.PaymentAddress = cmds[1];
                        PlayerType.UpdatePlayer(user);
                        await Program.bot.SendMessage(msg.Chat, $"更改成功，您当前的USDT收款地址为：<code>{PlayerType.GetPlayerById(msg.From.Id.ToString()).PaymentAddress}</code>.", ParseMode.Html);
                    }
                    
                    break;

                case "/code":
                    if (cmds.Length < 2)
                    {
                        await Program.bot.SendMessage(msg.Chat, "正确用法：<code>/code [邀请者的ID]</code>", ParseMode.Html);
                    }
                    else
                    {
                        var user = PlayerType.GetPlayerById(msg.From.Id.ToString());

                        if (!string.IsNullOrEmpty(user.RegCode))
                        {
                            await Program.bot.SendMessage(msg.Chat, $"您已经使用过邀请码，不能再次使用", ParseMode.Html);
                            return;
                        }

                        if (cmds[1] == user.InviteCode)
                        {
                            await Program.bot.SendMessage(msg.Chat, $"不能使用自己的邀请码", ParseMode.Html);
                            return;
                        }

                        var inviter = Runtimes.mongoDB.PlayerCollection.Find(p => p.InviteCode == cmds[1]).FirstOrDefault();
                        if (inviter == null)
                        {
                            await Program.bot.SendMessage(msg.Chat, $"邀请码无效，请确认后重新输入", ParseMode.Html);
                            return;
                        }

                        user.RegCode = cmds[1];
                        var userChanges = user.AmountChange.ToList();
                        user.Amount += 1000;
                        userChanges.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{user.Amount - 1000}|1000|使用邀请码+1000");
                        user.AmountChange = userChanges.ToArray();
                        PlayerType.UpdatePlayer(user);

                        var inviterChanges = inviter.AmountChange.ToList();
                        inviter.Amount += 1000;
                        inviterChanges.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{inviter.Amount - 1000}|1000|邀请他人注册+1000");
                        inviter.AmountChange = inviterChanges.ToArray();
                        PlayerType.UpdatePlayer(inviter);

                        await Program.bot.SendMessage(msg.Chat, $"邀请码校验成功，您与邀请者都已获得1000积分的奖励", ParseMode.Html);
                    }


                    break;

            }
            //}catch(Exception ex)
            //{
            //    await HandleEvent.OnError(ex, HandleErrorSource.HandleUpdateError);
            //}
        }

        public static async void InfoMenu(Message msg)
        {
            if (!PlayerType.PlayerExists(msg.From.Id.ToString()))
            {
                var player = new Types.Player
                {
                    ID = msg.From.Id.ToString(),
                    Amount = 100,
                    Level = 1,
                    VIPLevel = 0,
                    Invites = new string[] { },
                    PaymentAddress = "",
                    LastCheckIn = DateTime.Now.ToString("yyyyMMdd"),
                    RegDate = DateTime.Now.ToString("yyyyMMdd"),
                    InviteCode = PlayerType.GenerateUuid()
                };
                PlayerType.AddPlayer(player);
                await Program.bot.SendMessage(msg.Chat, "创建账户成功.");
                await Program.bot.SendMessage(msg.Chat, PlayerType.GetInfoMenu(player), ParseMode.Html, replyMarkup: ReplyMarkupModels.MainMenu(msg.From.Id.ToString()));
            }
            else await Program.bot.SendMessage(msg.Chat, PlayerType.GetInfoMenu(PlayerType.GetPlayerById(msg.From.Id.ToString())), ParseMode.Html, replyMarkup: ReplyMarkupModels.MainMenu(msg.From.Id.ToString()));
        }

        public static async void AdminMenu(Message msg, string[] cmds)
        {
            var role = "";
            var admin = Runtimes.IsAdmin(msg.From.Id.ToString());

            if (admin == 2) role = "Owner";
            else role = "Admin";

            if (admin != 0)
            {
                if (cmds.Length > 1)
                {
                    switch (cmds[1].ToLower())
                    {
                        case "version":
                            await Program.bot.SendMessage(msg.Chat, $"Hi ,{role}\n{StaticString.VersionInfo}", ParseMode.Html, replyMarkup: ReplyMarkupModels.MainMenu(msg.From.Id.ToString()));
                            break;
                        case "state":
                            await Program.bot.SendMessage(msg.Chat, $"Hi ,{role}\n{StaticString.StateInfo.Replace("{MEMORY}", $"{GC.GetTotalMemory(false) / 1024 / 1024}")}", ParseMode.Html, replyMarkup: ReplyMarkupModels.MainMenu(msg.From.Id.ToString()));
                            break;
                        case "help":
                            await Program.bot.SendMessage(msg.Chat, $"Hi ,{role}\n{StaticString.HelpInfo}", ParseMode.Html, replyMarkup: ReplyMarkupModels.MainMenu(msg.From.Id.ToString()));
                            break;
                        default:
                            await Program.bot.SendMessage(msg.Chat, $"指令<code>{msg.Text}</code>格式不正确", ParseMode.Html, replyMarkup: ReplyMarkupModels.AdminMenu());
                            break;
                    }
                }
                else await Program.bot.SendMessage(msg.Chat, $"指令<code>{msg.Text}</code>格式不正确", ParseMode.Html, replyMarkup: ReplyMarkupModels.AdminMenu());
            }
        }
    }
}
