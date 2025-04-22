using System;
using System.Data.SqlClient;

namespace ChantBot.Models
{

    public class DatabaseManager
    {
        private readonly SqlConnection _connection;

        public DatabaseManager(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public void EnsureDatabaseAndTables()
        {

            _connection.Open();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connected to SQL Server.");

            if (!DatabaseExists("ChantBot"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Database ChantBot does not exist. Creating...");
                Console.ForegroundColor = ConsoleColor.Green;
                ExecuteNonQuery("CREATE DATABASE ChantBot");
            }
            else
            {
                Console.WriteLine("Database ChantBot already exists.");
            }

            _connection.ChangeDatabase("ChantBot");


            if (!TableExists("Games"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Games does not exist. Creating...");
                string createGamesTable = @"
                    CREATE TABLE Games (
                        GameID BIGINT NOT NULL PRIMARY KEY,
                        StartDate NVARCHAR(MAX) NOT NULL,
                        Players NVARCHAR(MAX) NOT NULL,
                        GameType INT NOT NULL,
                        MinAmount INT NOT NULL,
                        GameResult NVARCHAR(MAX) NOT NULL
                    )";
                ExecuteNonQuery(createGamesTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table Games already exists.");
            }

            if (!TableExists("PlayerData"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table PlayerData does not exist. Creating...");
                string createPlayerDataTable = @"
                    CREATE TABLE PlayerData (
                        ID NVARCHAR(MAX) NOT NULL,
                        Amount BIGINT NOT NULL,
                        Level INT NOT NULL,
                        VIPLevel INT NOT NULL,
                        Invites NVARCHAR(MAX) NOT NULL DEFAULT '[]',
                        PaymentCode NVARCHAR(MAX) NULL，
                        LastCheckIn NVARCHAR(MAX) NOT NULL
                        RegDate NVARCHAR(MAX) NULL,
                        InviteCode NVARCHAR(MAX) NULL
                    )";
                ExecuteNonQuery(createPlayerDataTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table PlayerData already exists.");
            }

            if (!TableExists("Customer"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Customer does not exist. Creating...");
                string createCustomerDataTable = @"
                    CREATE TABLE Customer (
                        ID NVARCHAR(MAX) NOT NULL,
                        Name NVARCHAR(MAX) NULL
                    )";
                ExecuteNonQuery(createCustomerDataTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table Customer already exists.");
            }

            if (!TableExists("Group"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Group does not exist. Creating...");
                string createGroupDataTable = @"
                    CREATE TABLE [Group] (
                        Name NVARCHAR(MAX)
                    )";
                ExecuteNonQuery(createGroupDataTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table Group already exists.");
            }

            if (!TableExists("Admins"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Admins does not exist. Creating...");
                string createGroupDataTable = @"
                    CREATE TABLE Admins (
                        ID NVARCHAR(MAX) NOT NULL,
                        Name NVARCHAR(MAX) NULL
                    )";
                ExecuteNonQuery(createGroupDataTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table Admins already exists.");
            }

            if (!TableExists("Log"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Log does not exist. Creating...");
                string createLogTable = @"
        CREATE TABLE [Log] (
            [Date] NVARCHAR(MAX) NULL,
            [Chat] NVARCHAR(MAX) NULL,
            [From] NVARCHAR(MAX) NULL,
            [Message] NVARCHAR(MAX) NULL
        )";
                ExecuteNonQuery(createLogTable);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine("Table Log already exists.");
            }

            if (!TableExists("Error"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Table Error does not exist. Creating...");
                string createErrorTable = @"
        CREATE TABLE [Error] (
            [Date] NVARCHAR(MAX) NULL,
            [Message] NVARCHAR(MAX) NULL,
            [Source] NVARCHAR(MAX) NULL
        )";
                ExecuteNonQuery(createErrorTable);
            }
            else
            {
                Console.WriteLine("Table Error already exists.");
            }

            Console.ResetColor();
            _connection.Close();
            Console.WriteLine("Connection closed.");
        }

        private bool DatabaseExists(string databaseName)
        {
            string query = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
            return ExecuteScalar(query) > 0;
        }

        private bool TableExists(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            return ExecuteScalar(query) > 0;
        }

        private int ExecuteScalar(string query)
        {
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static OfficalUser[] LoadCustomerData(string connectS)
        {
            var customers = new List<OfficalUser>();
            using (SqlConnection connection = new SqlConnection(connectS))
            {
                connection.Open();
                string query = "SELECT ID, Name FROM Customer";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new OfficalUser
                        {
                            ID = reader["ID"].ToString(),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            return customers.ToArray();
        }

        public static OfficalUser[] LoadAdminsData(string connectS)
        {
            var admins = new List<OfficalUser>();
            using (SqlConnection connection = new SqlConnection(connectS))
            {
                connection.Open();
                string query = "SELECT ID, Name FROM Admins";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        admins.Add(new OfficalUser
                        {
                            ID = reader["ID"].ToString(),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            return admins.ToArray();
        }

        public static string[] LoadGroupData(string connectS)
        {
            var groups = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectS))
            {
                connection.Open();
                string query = "SELECT Name FROM [Group]";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(reader["Name"].ToString());
                    }
                }
            }
            return groups.ToArray();
        }

    }

}