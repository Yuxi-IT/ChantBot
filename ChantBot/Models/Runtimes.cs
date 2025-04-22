using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChantBot.Models
{
    public class Runtimes
    {
        public static BotSetting GetCurrentBotSetting()
        {
            var botSetting = new BotSetting();
            var connectS = GetAppInfo().SQLConnectionString;
            var botO = JsonConvert.DeserializeObject<BotSetting>(LoadDataFile("BotSetting.json").ToString());

            botSetting.Own = botO.Own;
            botSetting.WebApp = botO.WebApp;
            botSetting.Customer = DatabaseManager.LoadCustomerData(connectS);
            botSetting.Admins = DatabaseManager.LoadAdminsData(connectS);
            botSetting.Groups = DatabaseManager.LoadGroupData(connectS);

            return botSetting;
        }

        public static DateTime StartDate = DateTime.Now;
        
        public static int IsAdmin(string id)
        {
            if (GetCurrentBotSetting().Admins.Any(x => x.ID == id)) return 1;
            if (GetCurrentBotSetting().Own.ID == id) return 2;

            return 0;
        }

        public static object LoadDataFile(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"ChantBot.Resource.{file}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static AppInfo GetAppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"ChantBot.Resource.AppInfo.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<AppInfo>(reader.ReadToEnd()); ;
            }
        }

    }
    public class AppInfo
    {
        public string? Version { get; set; }
        public string? VersionName { get; set; }
        public string? BotToken { get; set; }
        public string? SQLConnectionString { get; set; }
    }
}