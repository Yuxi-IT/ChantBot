using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChantBot.Types;

namespace ChantBot.Models
{
    internal class LoggerModel
    {
        public static void AddLog(LogEntry log)
        {
            string query = @"
    INSERT INTO Log (Date, Chat, From, Message)
    VALUES (@Date, @Chat, @From, @Message)";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", log.Date);
                    command.Parameters.AddWithValue("@Chat", log.Chat);
                    command.Parameters.AddWithValue("@From", log.From);
                    command.Parameters.AddWithValue("@Message", log.Message);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static List<LogEntry> GetAllLogs()
        {
            string query = "SELECT Date, Chat, From, Message FROM Log";
            var logs = new List<LogEntry>();

            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().SQLConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new LogEntry
                        {
                            Date = reader["Date"].ToString(),
                            Chat = reader["Chat"].ToString(),
                            From = reader["From"].ToString(),
                            Message = reader["Message"].ToString()
                        });
                    }
                }
            }

            return logs;
        }


    }
}
