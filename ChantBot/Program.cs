using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ChantBot.Models;
using Microsoft.Extensions.Hosting;

class Program
{
    public static CancellationTokenSource cts = new();
    public static TelegramBotClient bot;
    public static User _bot;
    public async static Task Main(string[] args)
    {
        try
        {
            Console.ResetColor();
            var app = Runtimes.GetAppInfo();

            bot = new(app.BotToken, cancellationToken: cts.Token);

            DatabaseManager _dbm = new(app.SQLConnectionString);
            _dbm.EnsureDatabaseAndTables();

            _bot = await bot.GetMe();

            //bot.OnError += HandleEvent.OnError;
            bot.OnMessage += HandleEvent.OnMessage;
            bot.OnUpdate += HandleEvent.OnUpdate;

            #region PrintInfo
            Console.Write($"APP Version: ");

            SetColor(ConsoleColor.Yellow);
            Console.Write(app.VersionName);

            SetColor(ConsoleColor.Green);
            Console.WriteLine(app.Version);

            SetColor(ConsoleColor.Yellow);
            Console.WriteLine($"@{_bot.Username}");
            Console.ResetColor();

            Console.WriteLine($"({_bot.FirstName} {_bot.LastName}) is running... " +
                              $"Enter \"help\" to see the help menu.");
            #endregion

            while (true)
            {
                var type = Console.ReadLine();
                switch (type?.ToLower())
                {
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "state":
                        Console.WriteLine(StaticString.StateInfo);
                        break;
                    case "help":
                        Console.WriteLine(StaticString.HelpInfo);
                        break;
                    case "version":
                        Console.WriteLine(StaticString.VersionInfo);
                        break;
                }
            }
        }catch (Exception ex)
        {
            await HandleEvent.OnError(ex, HandleErrorSource.HandleUpdateError);
        }
    }
    public static string SetColor(ConsoleColor color)
    {
        Console.ForegroundColor = color;
        return "";
    }

}


