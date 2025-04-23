using ChantBot.Models;
using ChantBot.Types;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Game = ChantBot.Types.Game;

namespace ChantBot.Commands
{
    public class PlayerType
    {

        public static Player GetPlayerById(string playerId)
        {
            return Runtimes.mongoDB.PlayerCollection.Find(p => p.ID == playerId).FirstOrDefault();
        }


        public static List<Game> GetGamesByPlayerId(string playerId)
        {
            return Runtimes.mongoDB.GameCollection.Find(g => g.Players.Contains(playerId)).ToList();
        }


        public static void AddPlayer(Player player)
        {
            player.LastCheckIn = DateTime.Now.ToString("yyyyMMdd");
            player.RegDate = DateTime.Now.ToString("yyyyMMdd");
            player.Invites ??= Array.Empty<string>();
            player.AmountChange ??= Array.Empty<string>();
            Runtimes.mongoDB.PlayerCollection.InsertOne(player);
        }


        public static void UpdatePlayer(Player player)
        {
            var result = Runtimes.mongoDB.PlayerCollection.ReplaceOne(p => p.ID == player.ID, player);
            if (result.MatchedCount == 0)
            {
                Console.WriteLine("未找到对应用户，替换失败。");
            }
            else if (result.ModifiedCount == 0)
            {
                Console.WriteLine("找到用户，但内容相同，没有修改。");
            }
            else
            {
                Console.WriteLine("用户信息更新成功。");
            }

        }


        public static bool PlayerExists(string playerId)
        {
            return Runtimes.mongoDB.PlayerCollection.Find(p => p.ID == playerId).Any();
        }


        public static string GetInfoMenu(Player player)
        {
            if (player == null)
            {
                return "无法判断用户身份.";
            }
            else
            {
                return $"🎇Welcome back 🎇\n\n" +
                   $"👤ID: <code>{player.ID}</code>\n" +
                   $"💰余额: <code>{player.Amount}</code>\n" +
                   $"💕等级: <code>{GetLevelName(player.Level)}</code>\n" +
                   $"💎会员等级: <code>{GetVipName(player.VIPLevel)}</code>\n" +
                   $"📕上次签到: <code>{DateTime.ParseExact(player.LastCheckIn, "yyyyMMdd", null).ToString("yyyy年MM月dd日")}</code>\n" +
                   $"👥邀请用户: <code>{string.Join(", ", player.Invites)}</code>\n" +
                   $"💴收款地址: <code>{player.PaymentAddress}</code>\n" +
                   $"❤️邀请码: <code>{player.InviteCode}</code>";
            }
        }

        public static string GetLevelName(int level)
        {
            return level switch
            {
                1 => "👤普通用户",
                2 => "🧊进阶用户",
                3 => "🔥核心用户",
                4 => "🎯高阶用户",
                5 => "🛡精英用户",
                6 => "⚔特权用户",
                7 => "🚀尊享用户",
                8 => "🌪终极用户",
                -1 => "🛠管理员",
                _ => "❓未知等级",
            };
        }

        public static string GetVipName(int vip)
        {
            return vip switch
            {
                1 => "💲铂金会员",
                2 => "🏅黄金会员",
                3 => "✨闪星会员",
                4 => "💎钻石会员",
                5 => "👑黑金会员",
                _ => "无会员",
            };
        }


        public static async Task<int> CheckIn(string userID)
        {
            var player = GetPlayerById(userID);
            if (player == null) return -2;

            var lastCheckInDate = DateTime.ParseExact(player.LastCheckIn, "yyyyMMdd", null);

            if (lastCheckInDate.Date < DateTime.Now.Date)
            {
                player.LastCheckIn = DateTime.Now.ToString("yyyyMMdd");
                player.Amount += 100;
                var list = player.AmountChange?.ToList() ?? new List<string>();
                list.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{player.Amount}|100|每日签到 +100");
                player.AmountChange = list.OrderByDescending(x => DateTime.ParseExact(x.Split('|')[0], "yyyy-MM-dd HH:mm:ss", null)).ToArray();

                UpdatePlayer(player);
                return 1;
            }
            return 2;
        }

        public static string GenerateUuid()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N");
            return $"{timestamp}{guid}";
        }
    }
}
