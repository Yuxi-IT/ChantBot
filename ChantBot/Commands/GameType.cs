using ChantBot.Models;
using ChantBot.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Commands
{
    internal class GameType
    {
        public static Game GetGameById(long gameId)
        {
            string query = "SELECT * FROM Games WHERE GameID = @GameID";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();  // 显式打开连接
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GameID", gameId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Game
                            {
                                GameID = Convert.ToInt64(reader["GameID"]),
                                StartDate = reader["StartDate"].ToString(),
                                Players = JsonConvert.DeserializeObject<string[]>(reader["Players"].ToString()),
                                GameType = Convert.ToInt32(reader["GameType"]),
                                MinAmount = Convert.ToInt32(reader["MinAmount"]),
                                GameResult = reader["GameResult"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void AddGame(Game game)
        {
            string query = @"
            INSERT INTO Games (GameID, StartDate, Players, GameType, MinAmount, GameResult)
            VALUES (@GameID, @StartDate, @Players, @GameType, @MinAmount, @GameResult)";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();  // 显式打开连接
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GameID", game.GameID);
                    command.Parameters.AddWithValue("@StartDate", game.StartDate);
                    command.Parameters.AddWithValue("@Players", "[]");  // 默认空数组
                    command.Parameters.AddWithValue("@GameType", game.GameType);
                    command.Parameters.AddWithValue("@MinAmount", game.MinAmount);
                    command.Parameters.AddWithValue("@GameResult", game.GameResult);
                    command.ExecuteNonQuery();  // 执行插入操作
                }
            }
        }

        public static void UpdateGame(Game game)
        {
            string query = @"
            UPDATE Games
            SET StartDate = @StartDate, Players = @Players, GameType = @GameType, 
                MinAmount = @MinAmount, GameResult = @GameResult
            WHERE GameID = @GameID";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();  // 显式打开连接
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GameID", game.GameID);
                    command.Parameters.AddWithValue("@StartDate", game.StartDate);
                    command.Parameters.AddWithValue("@Players", JsonConvert.SerializeObject(game.Players));  // 转换玩家列表
                    command.Parameters.AddWithValue("@GameType", game.GameType);
                    command.Parameters.AddWithValue("@MinAmount", game.MinAmount);
                    command.Parameters.AddWithValue("@GameResult", game.GameResult);
                    command.ExecuteNonQuery();  // 执行更新操作
                }
            }
        }
    }
}
