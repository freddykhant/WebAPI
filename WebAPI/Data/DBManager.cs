using System.Data.SQLite;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=clientsdatabase.db;Version=3;";

        // Create the Client table if it doesn't exist
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
                            Port INTEGER
                        )";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        // Insert a new client into the database
        public static bool InsertClient(Client client)
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
                                Port = reader.GetInt32(2)
                            });
                        }
                    }
                }
                connection.Close();
            }
            return clients;
        }

        // Get a client by its Id
        public static Client GetClientById(int id)
        {
            Client client = null;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM ClientTable WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            client = new Client
                            {
                                Id = reader.GetInt32(0),
                                IPAddress = reader.GetString(1),
                                Port = reader.GetInt32(2)
                            };
                        }
                    }
                }
                connection.Close();
            }
            return client;
        }


        // Update a client's details in the database
        public static bool UpdateClient(Client updatedClient)
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
        public static bool DeleteClient(int id)
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

        // Create the Job table if it doesn't exist
        public static bool CreateJobTable()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    DROP TABLE IF EXISTS JobTable;
                    CREATE TABLE IF NOT EXISTS JobTable (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClientId INTEGER,
                        Status TEXT,
                        Code TEXT,
                        CompletionTime DATETIME
                    )";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        public static bool InsertJob(Job job)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    INSERT INTO JobTable (ClientId, Status, Code, CompletionTime) 
                    VALUES (@ClientId, @Status, @Code, @CompletionTime)";
                    command.Parameters.AddWithValue("@ClientId", job.ClientId);
                    command.Parameters.AddWithValue("@Status", job.Status);
                    command.Parameters.AddWithValue("@Code", job.Code);
                    command.Parameters.AddWithValue("@CompletionTime", job.CompletionTime);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        public static List<Job> GetAllJobs()
        {
            List<Job> jobs = new List<Job>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM JobTable", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new Job
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Status = reader.GetString(2),
                                Code = reader.GetString(3), // Fetching the Code
                                CompletionTime = reader.GetDateTime(4)
                            });
                        }
                    }
                }
                connection.Close();
            }
            return jobs;
        }


        public static Job GetJobById(int id)
        {
            Job job = null;
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM JobTable WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            job = new Job
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Status = reader.GetString(2),
                                Code = reader.GetString(3), // Fetching the Code
                                CompletionTime = reader.GetDateTime(4)
                            };
                        }
                    }
                }
                connection.Close();
            }
            return job;
        }


        public static bool UpdateJob(Job job)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    UPDATE JobTable 
                    SET ClientId = @ClientId, Status = @Status, Code = @Code, CompletionTime = @CompletionTime 
                    WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", job.Id);
                    command.Parameters.AddWithValue("@ClientId", job.ClientId);
                    command.Parameters.AddWithValue("@Status", job.Status);
                    command.Parameters.AddWithValue("@Code", job.Code);
                    command.Parameters.AddWithValue("@CompletionTime", job.CompletionTime);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

        public static bool DeleteJob(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM JobTable WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return true;
        }

    }
}