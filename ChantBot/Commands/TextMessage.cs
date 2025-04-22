using ChantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;

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
                    PaymentCode = "",
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
