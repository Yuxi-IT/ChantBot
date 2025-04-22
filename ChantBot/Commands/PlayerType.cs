using ChantBot.Models;
using ChantBot.Types;
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
            if (int.TryParse(playerId, out int result))
            {
                string query = "SELECT * FROM PlayerData WHERE ID = @ID";
                using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", playerId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Player
                                {
                                    ID = reader["ID"].ToString(),
                                    Amount = Convert.ToInt64(reader["Amount"]),
                                    Level = Convert.ToInt32(reader["Level"]),
                                    VIPLevel = Convert.ToInt32(reader["VIPLevel"]),
                                    Invites = JsonConvert.DeserializeObject<string[]>(reader["Invites"].ToString()),
                                    PaymentCode = reader["PaymentCode"].ToString(),
                                    LastCheckIn = DateTime.ParseExact(reader["LastCheckIn"].ToString(), "yyyyMMdd", null).ToString("yyyyMMdd"),
                                    AmountChange = reader["AmountChange"].ToString().Split("\n").ToArray(),
                                    RegDate = reader["RegDate"].ToString(),
                                    InviteCode = reader["InviteCode"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static List<Game> GetGamesByPlayerId(string playerId)
        {
            string query = "SELECT * FROM Games WHERE Players LIKE @PlayerId";
            List<Game> games = new List<Game>();
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlayerId", $"%\"ID\":\"{playerId}\"%");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            games.Add(new Game
                            {
                                GameID = Convert.ToInt64(reader["GameID"]),
                                StartDate = reader["StartDate"].ToString(),
                                Players = JsonConvert.DeserializeObject<string[]>(reader["Players"].ToString()),
                                GameType = Convert.ToInt32(reader["GameType"]),
                                MinAmount = Convert.ToInt32(reader["MinAmount"]),
                                GameResult = reader["GameResult"].ToString()
                            });
                        }
                    }
                }
            }
            return games;
        }

        public static void AddPlayer(Player player)
        {
            string query = @"
                INSERT INTO PlayerData (ID, Amount, Level, VIPLevel, Invites, PaymentCode, LastCheckIn, RegDate, InviteCode)
                VALUES (@ID, @Amount, @Level, @VIPLevel, @Invites, @PaymentCode, @LastCheckIn, @RegDate, @InviteCode)";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", player.ID);
                    command.Parameters.AddWithValue("@Amount", player.Amount);
                    command.Parameters.AddWithValue("@Level", player.Level);
                    command.Parameters.AddWithValue("@VIPLevel", player.VIPLevel);
                    command.Parameters.AddWithValue("@Invites", "[]");
                    command.Parameters.AddWithValue("@PaymentCode", player.PaymentCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastCheckIn", DateTime.Now.ToString("yyyyMMdd"));
                    command.Parameters.AddWithValue("@RegDate", DateTime.Now.ToString("yyyyMMdd"));
                    command.Parameters.AddWithValue("@InviteCode", player.InviteCode);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePlayer(Player player)
        {
            string query = @"
                UPDATE PlayerData
                SET Amount = @Amount, Level = @Level, VIPLevel = @VIPLevel, 
                    Invites = @Invites, PaymentCode = @PaymentCode,
                    LastCheckIn = @LastCheckIn,AmountChange = @AmountChange
                WHERE ID = @ID";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", player.ID);
                    command.Parameters.AddWithValue("@Amount", player.Amount);
                    command.Parameters.AddWithValue("@Level", player.Level);
                    command.Parameters.AddWithValue("@VIPLevel", player.VIPLevel);
                    command.Parameters.AddWithValue("@Invites", JsonConvert.SerializeObject(player.Invites));
                    command.Parameters.AddWithValue("@PaymentCode", player.PaymentCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastCheckIn", player.LastCheckIn);
                    command.Parameters.AddWithValue("@AmountChange", string.Join("\n", player.AmountChange));
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool PlayerExists(string playerId)
        {
            string query = "SELECT COUNT(*) FROM PlayerData WHERE ID = @ID";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", playerId);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
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
                   $"💰Amount: <code>{player.Amount}</code>\n" +
                   $"💕Level: <code>{GetLevelName(player.Level)}</code>\n" +
                   $"💎VIP Level: <code>{GetVipName(player.VIPLevel)}</code>\n" +
                   $"📕Last CheckIn: <code>{DateTime.ParseExact(player.LastCheckIn, "yyyyMMdd", null).ToString("yyyy年MM月dd日")}</code>\n" +
                   $"👥Invites: <code>{string.Join(", ", player.Invites)}</code>\n" +
                   $"💴Payment Code: <code>{player.PaymentCode}</code>\n" +
                   $"❤️Invite Code: <code>{player.InviteCode}</code>";
            }
        }

        public static string GetLevelName(int level)
        {
            switch (level)
            {
                case 1:
                    return "⭐赌徒⭐";
                case 2:
                    return "🌟赌棍🌟";
                case 3:
                    return "✨赌侠✨";
                case 4:
                    return "👹赌怪👹";
                case 5:
                    return "👑赌王👑";
                case 6:
                    return "🗡赌圣🗡";
                case 7:
                    return "💎赌神💎";
                case 8:
                    return "🌩️赌仙🌩️";

                case -1:
                    return "🌩️场务管理员🌩️";
                default:
                    return "未知";
            }

        }

        public static string GetVipName(int vip)
        {
            switch (vip)
            {
                case 1:
                    return "💲铂金会员💲";
                case 2:
                    return "🏅黄金会员🏅";
                case 3:
                    return "✨闪星会员✨";
                case 4:
                    return "💎钻石会员💎";
                case 5:
                    return "👑黑金会员👑";

                default:
                    return "👤用户👤";
            }

        }

        public static async Task<int> CheckIn(string userID)
        {
            //try
            //{
                var player = GetPlayerById(userID);
                if (player == null)
                    return -2;

                var lastCheckInDate = DateTime.ParseExact(player.LastCheckIn, "yyyyMMdd", null);

                if (lastCheckInDate.Date < DateTime.Now.Date)
                {
                    player.LastCheckIn = DateTime.Now.ToString("yyyyMMdd");
                    player.Amount += 100;
                    var list = player.AmountChange.ToList();
                    list.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}|{player.Amount}|100|每日签到 +100");
                    player.AmountChange = list.OrderByDescending(x => DateTime.ParseExact(x.Split('|')[0], "yyyy-MM-dd HH:mm:ss", null)).ToArray();
                    
                    UpdatePlayer(player);

                    return 1;
                }
                else
                {
                    return 2;
                }
            //}
            //catch
            //{
            //    return -1;
            //}
        }
        public static string GenerateUuid()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N");
            return $"{timestamp}{guid}";
        }
    }
}
