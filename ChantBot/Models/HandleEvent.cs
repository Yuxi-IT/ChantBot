using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Diagnostics;
using ChantBot.Commands;
using ChantBot.Types;
using System.Collections;

namespace ChantBot.Models
{
    public static class HandleEvent
    {
        public static async Task OnError(Exception exception, HandleErrorSource source)
        {
            if(exception.Message != "Exception during making request")
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Error.WriteLine($"Error: {exception.Message}\nSource: {source}");
                Console.ResetColor();
                await Program.bot.SendMessage(Runtimes.GetCurrentBotSetting().Own.ID,
                    $"控制台报错\n" +
                    $"Date:<code>{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}</code>\n" +
                    $"Error: <code>{exception.Message}</code>\n" +
                    $"Source: <code>{source}</code>\n" +
                    $"<a href=\"https://www.google.com/search?q=Telegram+Bot+{exception.Message}/\">谷歌搜索问题</a>",
                    parseMode: ParseMode.Html);
            }

        }

        public static async Task OnMessage(Message msg, UpdateType type)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            switch (msg.Type)
            {
                case MessageType.Text:
                    Console.WriteLine($"[TextMessage] User: {msg.From.FirstName} {msg.From.LastName} ({msg.From.Username}: {msg.From.Id}) ({msg.Chat.Username}: {msg.Chat.Id})");
                    Console.WriteLine($"Text : {msg.Text}");
                    TextMessage.HanleText(msg);
                    break;

                case MessageType.NewChatMembers:
                    var newUser = msg.NewChatMembers[0];
                    Console.WriteLine($"[New member]: @{newUser.Username} [{newUser.Id}] - {newUser.FirstName} {newUser.LastName} ({msg.Chat.Username}: {msg.Chat.Id})");
                    break;

                case MessageType.LeftChatMember:
                    Console.WriteLine($"[Left member]: @{msg.LeftChatMember.Username}[{msg.LeftChatMember.Id}] - {msg.LeftChatMember.FirstName}{msg.LeftChatMember.LastName} ({msg.Chat.Username}: {msg.Chat.Id})");
                    break;

                case MessageType.NewChatTitle:
                    Console.WriteLine($"[NewTitle] SetUser: {msg.From.FirstName} {msg.From.LastName} (ID: {msg.From.Id})");
                    Console.WriteLine($"[Title] : {msg.NewChatTitle}");
                    break;

                default:
                    break;
            }
            Console.ResetColor();
        }

        public static async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Callback] User: {query.From.FirstName} {query.From.LastName} ({query.From.Username}: {query.From.Id}) ({query.Message.Chat.Username}: {query.Message.Chat.Id})");
                Console.WriteLine($"Data: {query.Data}");
                Console.ResetColor();

                CallbackMessage.HanleData(query);
            }
        }
    }
}
