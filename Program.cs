using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySqlSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string connectionString = "Server=localhost; Database=test; User=root; Password=root";
            const string tableName = "tempDemoTable";
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    //Creo una tabella (se non esiste già)
                    cmd.CommandText = $"CREATE TEMPORARY TABLE {tableName} (id INT NOT NULL AUTO_INCREMENT, title VARCHAR(50) NOT NULL, description TEXT, CONSTRAINT pk_{tableName} PRIMARY KEY (id))";
                    await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"Ho creato la tabella temporanea {tableName}");

                    //Ci inserisco dentro una riga
                    cmd.CommandText = $"INSERT INTO {tableName} (title, description) VALUES (@title, @description)";
                    cmd.Parameters.AddWithValue("title", "Questo è il titolo");
                    cmd.Parameters.AddWithValue("description", "Questa è la descrizione");
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                    Console.WriteLine($"Ho inserito una riga nella tabella");

                    //Ne inserisco un'altra
                    cmd.CommandText = $"INSERT INTO {tableName} (title, description) VALUES (@title, @description)";
                    cmd.Parameters.AddWithValue("title", "Ecco un altro titolo");
                    cmd.Parameters.AddWithValue("description", "Altra descrizione");
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                    Console.WriteLine($"Ho inserito un'altra riga nella tabella");

                    //Quante righe ci sono nella tabella?
                    cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                    long count = (long) await cmd.ExecuteScalarAsync();
                    Console.WriteLine($"Nella tabella ci sono {count} record");

                    //Ora recupero le righe che ho appena inserito e le stampo in console
                    cmd.CommandText = $"SELECT id, title, description FROM {tableName}";
                    Console.WriteLine("Estraggo i record");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) {
                            int id = (int) reader["id"];
                            string title = (string) reader["title"];
                            string description = (string) reader["description"];

                            //Stampo i valori delle varie colonne separandoli con un tab
                            Console.WriteLine($"\t{id}\t{title}\t{description}");
                        }
                    }
                }
            }
            Console.WriteLine("Premi invio per uscire dal programma");
            Console.ReadLine();
        }
    }
}
