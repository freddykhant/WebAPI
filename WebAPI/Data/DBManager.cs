using System.Data.SQLite;
using WebAPI.Models;
using System.Collections.Generic; // Required for List<>
using System.IO; // Required for File operations
using System; // Required for Console and Exception

namespace WebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=clientsdatabase.db;Version=3;";

        public static bool CreateClientTable()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ClientTable (
                    Id INTEGER PRIMARY KEY,
                    IPAddress TEXT,
                    Port INTEGER,
                    JobsCompleted INTEGER
                )";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        public static bool InsertClient(Client client)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    INSERT INTO ClientTable (IPAddress, Port, JobsCompleted) 
                    VALUES (@IPAddress, @Port, @JobsCompleted)";
                    command.Parameters.AddWithValue("@IPAddress", client.IPAddress);
                    command.Parameters.AddWithValue("@Port", client.Port);
                    command.Parameters.AddWithValue("@JobsCompleted", client.CompletedJobsCount);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        public static List<Client> GetAllClients()
        {
            List<Client> clients = new List<Client>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM ClientTable", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(0),
                                IPAddress = reader.GetString(1),
                                Port = reader.GetInt32(2),
                                CompletedJobsCount = reader.GetInt32(3)
                            });
                        }
                    }
                }
                connection.Close();
            }
            return clients;
        }

        public static bool UpdateClientJobs(Client client)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE ClientTable 
                    SET JobsCompleted = @JobsCompleted 
                    WHERE IPAddress = @IPAddress AND Port = @Port";

                    command.Parameters.AddWithValue("@IPAddress", client.IPAddress);
                    command.Parameters.AddWithValue("@Port", client.Port);
                    command.Parameters.AddWithValue("@JobsCompleted", client.CompletedJobsCount);

                    int rowsUpdated = command.ExecuteNonQuery();
                    connection.Close();
                    return rowsUpdated > 0;
                }
            }
        }

        public static bool DeleteClient(Client client)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM ClientTable WHERE IPAddress = @IPAddress AND Port = @Port AND JobsCompleted = @JobsCompleted";
                    command.Parameters.AddWithValue("@IPAddress", client.IPAddress);
                    command.Parameters.AddWithValue("@Port", client.Port);
                    command.Parameters.AddWithValue("@JobsCompleted", client.CompletedJobsCount);

                    int rowsDeleted = command.ExecuteNonQuery();
                    connection.Close();
                    return rowsDeleted > 0;
                }
            }
        }


        public static void deleteDB()
        {
            string filePath = @"C:\Users\fredd\source\repos\WebAPI\WebAPI\clientsdatabase.db";

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Console.WriteLine("File deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }
    }
}
