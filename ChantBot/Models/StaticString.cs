using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NovaPanel.Model.ServerMonitorM;
namespace ChantBot.Models
{
    public static class StaticString
    {
        public static string HelpInfo { get; set; } = $"Chant 浅吟 帮助菜单\n" +
                                                      $"命令:\n" +
                                                      $" help  - 帮助信息\n" +
                                                      $" info  - 展示软件版本信息\n" +
                                                      $" state - 展示机器人与系统硬件信息";
        public static string VersionInfo { get; set; } = $"(@{Program._bot.Username}) 软件版本 {Runtimes.GetAppInfo().Version} - {Runtimes.GetAppInfo().VersionName}";
        public static string StateInfo { get; set; } = $@"
{Program._bot.FirstName}{Program._bot.LastName} is currently running..
机器人ID: @{Program._bot.Username}
启动时间: <code>{Runtimes.StartDate.ToString("yyyy-MM-dd HH:mm")}</code>
CPU信息: <code>{WMIModels.GetCpuInfo()}</code>
GPU信息: <code>{WMIModels.GetGpuInfo()}</code>
内存占用: <code>{RamUsage.FormatSize(RamUsage.GetTotalPhys() - RamUsage.GetUsedPhys())}可用  共{RamUsage.FormatSize(RamUsage.GetTotalPhys())}</code>
处理器占用: <code>{CpuUsage.GetCpuUsage()}%</code>
";
    }
}
