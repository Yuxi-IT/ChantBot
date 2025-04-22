using ChantBot.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChantBot.Commands
{
    internal class CallbackMessage
    {
        public static async void HanleData(CallbackQuery query)
        {
            //try
            //{
            var msg = query.Message;
            var chatID = new ChatId(msg.Chat.Id);
            var msgID = msg.Id;

            var cmds = query.Data.Split(" ");

            var player_M = PlayerType.GetPlayerById(query.From.Id.ToString());
            switch (cmds[0].ToLower())
            {
                case "language":
                    await Program.bot.EditMessageText(chatID, msgID, "设置语言 | Change language", replyMarkup: ReplyMarkupModels.LanguageMenu());
                    break;

                case "mainmenu":
                    
                    if (player_M != null)
                        await Program.bot.EditMessageText(chatID, msgID, PlayerType.GetInfoMenu(PlayerType.GetPlayerById(query.From.Id.ToString())), replyMarkup: ReplyMarkupModels.MainMenu(query.From.Id.ToString()), parseMode: ParseMode.Html);
                    break;

                case "adminmenu":
                    await Program.bot.EditMessageText(chatID, msgID, "管理员菜单 | Admin Menu", replyMarkup: ReplyMarkupModels.AdminMenu(), parseMode: ParseMode.Html);
                    break;

                case "/help":
                    await Program.bot.EditMessageText(chatID, msgID, "帮助菜单 | Help menu", replyMarkup: ReplyMarkupModels.HelpMenu(msg.Chat.Id.ToString(), query.From.Id.ToString()));
                    break;

                case "amountchange":
                    AmountChangeList(query, cmds);
                    break;

                case "admin":
                    AdminHandle(cmds, query);
                    break;

                case "checkin":

                    if (player_M != null)
                    {
                        var result = await PlayerType.CheckIn(query.From.Id.ToString());

                        if (result == 1)
                        {
                            await Program.bot.EditMessageText(chatID, msgID, PlayerType.GetInfoMenu(PlayerType.GetPlayerById(query.From.Id.ToString())), replyMarkup: ReplyMarkupModels.MainMenu(query.From.Id.ToString()), parseMode: ParseMode.Html);
                            await Program.bot.AnswerCallbackQuery(query.Id, $"签到成功✔，获得100⭐余额");
                        }

                        else if (result == -1)
                            await Program.bot.AnswerCallbackQuery(query.Id, $"签到失败❌，请联系管理员");
                        else if (result == 2)
                            await Program.bot.AnswerCallbackQuery(query.Id, $"❌今日已签到");
                    }

                    break;
                default:
                    break;
            }

            //await Program.bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
            //await Program.bot.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            //}
            //catch(Exception ex)
            //{
            //    await HandleEvent.OnError(ex, HandleErrorSource.HandleUpdateError);
            //}
        }

        public static async void AmountChangeList(CallbackQuery query, string[] cmds)
        {
            var player = PlayerType.GetPlayerById(query.From.Id.ToString());
            if (player == null)
            {
                await Program.bot.AnswerCallbackQuery(query.Id, $"请先注册");
                return;
            }
            var amountChanges = player.AmountChange;

            int currentPage = 1;
            if (query.Data.Contains(" "))
            {
                int.TryParse(query.Data.Split(" ")[1], out currentPage);
            }

            int pageSize = 20;
            int totalPages = (int)Math.Ceiling((double)amountChanges.Count() / pageSize);
            currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

            var currentPageItems = amountChanges.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            var changes = "  更改时间  | 余额 | 变更额 | 变更原因\n";
            foreach (var item in currentPageItems)
            {
                changes += ($"<code>{item}</code>\n");
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("上一页", $"amountchange {currentPage - 1}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("下一页", $"amountchange {currentPage + 1}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("跳转到页码", $"amountchange jump")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("◀ 返回主菜单", $"MAINMENU")
                }
            });

            if (currentPage == 1)
            {
                inlineKeyboard = new InlineKeyboardMarkup(new[] {
                    new[] {
                        InlineKeyboardButton.WithCallbackData("下一页", $"amountchange {currentPage + 1}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("跳转到页码", $"amountchange jump")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("◀ 返回主菜单", $"MAINMENU")
                    }
                });
            }

            if (currentPage == totalPages)
            {
                inlineKeyboard = new InlineKeyboardMarkup(new[] {
                    new[] {
                        InlineKeyboardButton.WithCallbackData("上一页", $"amountchange {currentPage - 1}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("跳转到页码", $"amountchange jump")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("◀ 返回主菜单", $"MAINMENU")
                    }
                });
            }

            if (totalPages == 1)
            {
                inlineKeyboard = new InlineKeyboardMarkup(new[] {
                    new[] {
                        InlineKeyboardButton.WithCallbackData("◀ 返回主菜单", $"MAINMENU")
                    }
                });
            }

            if (cmds.Length > 1 && cmds[1] == "jump")
            {
                var pageButtons = new List<InlineKeyboardButton[]>();

                var rowButtons = new List<InlineKeyboardButton>();
                for (int i = 1; i <= totalPages; i++)
                {
                    rowButtons.Add(InlineKeyboardButton.WithCallbackData(i.ToString(), $"amountchange {i}"));

                    if (rowButtons.Count == 5)
                    {
                        pageButtons.Add(rowButtons.ToArray());
                        rowButtons.Clear();
                    }
                }

                if (rowButtons.Count > 0)
                {
                    pageButtons.Add(rowButtons.ToArray());
                }

                // 添加返回按钮
                pageButtons.Add(new[] {
                    InlineKeyboardButton.WithCallbackData("◀ 返回", $"amountchange {currentPage}")
                });

                inlineKeyboard = new InlineKeyboardMarkup(pageButtons);
            }


            await Program.bot.EditMessageText(query.Message.Chat.Id, query.Message.Id, changes, replyMarkup: inlineKeyboard, parseMode: ParseMode.Html);
        }


        public static async void AdminHandle(string[] cmds, CallbackQuery query)
        {
            var msg = query.Message;
            var msgID = msg.Id;

            var role = "";
            var admin = Runtimes.IsAdmin(query.From.Id.ToString());

            if (admin == 2) role = "Owner";
            else role = "Admin";

            if (admin != 0)
            {
                if (cmds.Length > 1)
                {
                    switch (cmds[1].ToLower())
                    {
                        case "version":
                            await Program.bot.EditMessageText(msg.Chat, msgID, $"Hi ,{role}\n{StaticString.VersionInfo}", ParseMode.Html, replyMarkup: ReplyMarkupModels.AdminMenu());
                            break;
                        case "state":
                            await Program.bot.EditMessageText(msg.Chat, msgID, $"Hi ,{role}\n{StaticString.StateInfo.Replace("{MEMORY}", $"{GC.GetTotalMemory(false) / 1024 / 1024}")}", ParseMode.Html, replyMarkup: ReplyMarkupModels.AdminMenu());
                            break;
                        case "help":
                            await Program.bot.EditMessageText(msg.Chat, msgID, $"Hi ,{role}\n{StaticString.HelpInfo}", ParseMode.Html, replyMarkup: ReplyMarkupModels.AdminMenu());
                            break;
                    }
                }
            }

        }
    }
}
