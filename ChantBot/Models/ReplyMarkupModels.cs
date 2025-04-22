using ChantBot.Commands;
using ChantBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChantBot.Models
{
    internal class ReplyMarkupModels
    {
        public static InlineKeyboardButton[] BackBtn = new[] { InlineKeyboardButton.WithCallbackData("◀ 返回", "MAINMENU") };
        public static InlineKeyboardButton[] BackAdminBtn = new[] { InlineKeyboardButton.WithCallbackData("◀ 管理员菜单", "ADMINMENU") };

        public static InlineKeyboardButton Admin(string fromID)
        {
            if (Runtimes.IsAdmin(fromID) != 0)
            {
                return InlineKeyboardButton.WithCallbackData("◀ 管理员菜单", "ADMINMENU");
            }
            return InlineKeyboardButton.WithUrl("🤖 浅吟Bot", $"http://t.me/{Program._bot.Username}"); ;
        }

        public static InlineKeyboardButton[] PriOrPub(string chatID, string fromID)
        {
            if (chatID == fromID)
            {
                var groupButtons = Runtimes.GetCurrentBotSetting().Groups
                    .Select((group, index) => InlineKeyboardButton.WithUrl($"👥 群组{index + 1}", $"http://t.me/{group}"))
                    .ToArray();

                var groupButtonRows = new List<InlineKeyboardButton[]>();
                for (int i = 0; i < groupButtons.Length; i += 3)
                {
                    groupButtonRows.Add(groupButtons.Skip(i).Take(3).ToArray());
                }

                return groupButtonRows.SelectMany(row => row).ToArray();
            }
            else
            {
                return new[]
                {
                    InlineKeyboardButton.WithUrl("🤖 使用浅吟", $"http://t.me/{Program._bot.Username}")
                };
            }
        }

        public static InlineKeyboardMarkup MainMenu(string fromID)
        {
            return new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("❓ 帮助", "/help"),
                        InlineKeyboardButton.WithCallbackData("📫 语言", "LANGUAGE")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("💼 业务联系", $"http://t.me/{Runtimes.GetCurrentBotSetting().Own.Name}"),
                        Admin(fromID)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✅ 每日签到", "CHECKIN"),
                        InlineKeyboardButton.WithCallbackData("📓 余额流水", "AMOUNTCHANGE")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("🧑‍💻 打开小程序", Runtimes.GetCurrentBotSetting().WebApp),
                    }
                });
        }

        public static InlineKeyboardMarkup LanguageMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("🇨🇳 中文", "https://t.me/setlanguage/zh-hans-beta"),
                        InlineKeyboardButton.WithUrl("🇹🇼 繁體中文", "https://t.me/setlanguage/zh-hant-beta")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("🇯🇵 日本語", "https://t.me/setlanguage/ja-beta"),
                        InlineKeyboardButton.WithUrl("🇰🇷 한국어", "https://t.me/setlanguage/ko")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("🇷🇺 Русский", "http://t.me/setlanguage/ru"),
                        InlineKeyboardButton.WithUrl("🇪🇸 Español", "http://t.me/setlanguage/es")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("🇫🇷 Français", "http://t.me/setlanguage/fr"),
                        InlineKeyboardButton.WithUrl("🇺🇸 English", "http://t.me/setlanguage/en"),
                    },
                    BackBtn
                });
        }

        public static InlineKeyboardMarkup HelpMenu(string chatID, string fromID)
        {
            var customerButtons = Runtimes.GetCurrentBotSetting().Customer.Select(customer =>
                InlineKeyboardButton.WithUrl($"💁‍ 客服 {customer.Name}", $"http://t.me/{customer.Name}")
            ).ToArray();

            var customerButtonRows = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < customerButtons.Length; i += 2)
            {
                customerButtonRows.Add(customerButtons.Skip(i).Take(2).ToArray());
            }

            var buttons = new List<InlineKeyboardButton[]> { };
            buttons.AddRange(customerButtonRows);
            buttons.Add(PriOrPub(chatID, fromID));
            buttons.Add(BackBtn);

            return new InlineKeyboardMarkup(buttons);
        }

        public static InlineKeyboardMarkup AdminMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔍 状态", "admin state"),
                    InlineKeyboardButton.WithCallbackData("🔧 版本", "admin version")
                },new[]
                {
                    InlineKeyboardButton.WithCallbackData("❓ 帮助", "admin help")
                },
                BackBtn
            });
        }
    }
}
