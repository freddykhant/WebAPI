using System.Data.SQLite;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=clientsdatabase.db;Version=3;";

        // Create the Client table if it doesn't exist
        public static bool CreateTable()
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
                            Port INTEGER
                        )";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        // Insert a new client into the database
        public static bool Insert(Client client)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO ClientTable (IPAddress, Port) 
                        VALUES (@IPAddress, @Port)";
                    command.Parameters.AddWithValue("@IPAddress", client.IPAddress);
                    command.Parameters.AddWithValue("@Port", client.Port);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        // Retrieve all clients from the database
        public static List<Client> GetAll()
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
                                Port = reader.GetInt32(2)
                            });
                        }
                    }
                }
                connection.Close();
            }
            return clients;
        }

        // Update a client's details in the database
        public static bool Update(Client updatedClient)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                UPDATE ClientTable 
                SET IPAddress = @IPAddress, Port = @Port 
                WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", updatedClient.Id);
                    command.Parameters.AddWithValue("@IPAddress", updatedClient.IPAddress);
                    command.Parameters.AddWithValue("@Port", updatedClient.Port);

                    int affectedRows = command.ExecuteNonQuery();
                    connection.Close();

                    return affectedRows > 0; // Return true if any rows were updated
                }
            }
        }


        // Delete a client from the database
        public static bool Delete(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM ClientTable WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }
    }
}