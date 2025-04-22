using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Models
{
    public static class StaticString
    {
        public static string HelpInfo { get; set; } = $"Chant 浅吟 Help Menu\n" +
                                                      $"Commands:\n" +
                                                      $" help  - Show help information\n" +
                                                      $" info  - Show version information\n" +
                                                      $" state - Show bot & application state information";
        public static string VersionInfo { get; set; } = $"(@{Program._bot.Username}) Bot Version {Runtimes.GetAppInfo().Version} - {Runtimes.GetAppInfo().VersionName}";
        public static string StateInfo { get; set; } = $@"
{Program._bot.FirstName}{Program._bot.LastName} is currently running..
Bot Username: @{Program._bot.Username}
StartUp Date: <code>{Runtimes.StartDate.ToString("yyyy-MM-dd HH:mm")}</code>
RAM Usage: <code>{{MEMORY}} MB</code>
CPU Usage: <code>{Environment.ProcessorCount} Core(s)</code>
";
    }
}
