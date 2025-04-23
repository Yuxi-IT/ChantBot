using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChantBot.Types;
namespace ChantBot.Models
{
    internal class ErrorModel
    {
        public static void AddError(ErrorEntry error)
        {
            string query = @"
    INSERT INTO Error (Date, Message, Source)
    VALUES (@Date, @Message, @Source)";
            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().DBConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", error.Date);
                    command.Parameters.AddWithValue("@Message", error.Message);
                    command.Parameters.AddWithValue("@Source", error.Source);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static List<ErrorEntry> GetAllErrors()
        {
            string query = "SELECT Date, Message, Source FROM Error";
            var errors = new List<ErrorEntry>();

            using (SqlConnection connection = new SqlConnection(Runtimes.GetAppInfo().DBConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        errors.Add(new ErrorEntry
                        {
                            Date = reader["Date"].ToString(),
                            Message = reader["Message"].ToString(),
                            Source = reader["Source"].ToString()
                        });
                    }
                }
            }

            return errors;
        }

    }
}
