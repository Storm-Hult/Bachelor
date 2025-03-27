using System;
using System.Diagnostics;
using Npgsql;

namespace BO25EB_47
{
    public class DBInitialize
    {
        public static void InitializeDB()
        {
            // Start PostgreSQL hvis den ikke kjører
            StartPostgreSQL();

            if (DatabaseExists("sjakkdb"))
            {
                // Dersom databasen allerede finnes, slett eventuelle eksisterende tabeller og opprett nye.
                //ResetTables();
                //CreateTables();
            }
            else
            {
                // Dersom databasen ikke finnes, opprett databasen og tabellene.
                CreateDatabase();
                CreateTables();
            }
        }

        private static void StartPostgreSQL()
        {
            string startCommand = "net start postgresql-x64-17";
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {startCommand}",
                Verb = "runas",        // Kjør som administrator
                UseShellExecute = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new Exception("Kunne ikke starte PostgreSQL. Exit code: " + process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kunne ikke starte PostgreSQL: " + ex.Message);
            }
        }

        // Stopper PostgreSQL som administrator ved applikasjonsavslutning
        public static void StopPostgreSQL()
        {
            string stopCommand = "net stop postgresql-x64-17";
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {stopCommand}",
                Verb = "runas",        // Kjør som administrator
                UseShellExecute = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    Console.WriteLine("PostgreSQL stoppet.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil ved stopp av PostgreSQL: " + ex.Message);
            }
        }

        // Sjekker om databasen allerede finnes
        private static bool DatabaseExists(string databaseName)
        {
            // Kobler til "sjakkdb" for å sjekke i pg_database (alternativt koble til "postgres")
            string connectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbname", conn))
                {
                    cmd.Parameters.AddWithValue("@dbname", databaseName);
                    return cmd.ExecuteScalar() != null;
                }
            }
        }

        // Oppretter databasen hvis den ikke finnes
        private static void CreateDatabase()
        {
            // Her kobler vi oss til "postgres" for å kunne kjøre CREATE DATABASE
            string connectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=postgres";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CREATE DATABASE sjakkdb", conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Database 'sjakkdb' opprettet.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Kunne ikke opprette database: {ex.Message}");
                    }
                }
            }
        }

        // Sletter eksisterende tabeller (små bokstaver, ingen anførselstegn)
        private static void ResetTables()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string dropTables = @"
                    DROP TABLE IF EXISTS trekk;
                    DROP TABLE IF EXISTS botparti;
                    DROP TABLE IF EXISTS onlineparti;
                ";
                using (var cmd = new NpgsqlCommand(dropTables, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Eksisterende tabeller slettet.");
                }
            }
        }

        // Oppretter tabeller i "sjakkdb" med små bokstaver
        private static void CreateTables()
        {
            string newDbConnectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";

            using (var conn = new NpgsqlConnection(newDbConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS onlineparti (
                            onlinepartiid SERIAL PRIMARY KEY,
                            motstander VARCHAR(100),
                            rating INT,
                            tidsformat VARCHAR(50),
                            antalltrekk INT,
                            vinner VARCHAR(50),
                            farge VARCHAR(10),
                            dato DATE
                        );

                        CREATE TABLE IF NOT EXISTS botparti (
                            botpartiid SERIAL PRIMARY KEY,
                            vanskelighetsgrad INT,
                            antalltrekk INT,
                            vinner VARCHAR(50),
                            farge VARCHAR(10),
                            dato DATE
                        );

                        CREATE TABLE IF NOT EXISTS trekk (
                            trekknr VARCHAR(10) PRIMARY KEY,
                            tidbrukt INTERVAL,
                            resterendetid INTERVAL,
                            notasjon VARCHAR(10),
                            posisjon VARCHAR(100),
                            evalbar DOUBLE PRECISION,
                            partitype VARCHAR(10),
                            partiid INT,
                            CONSTRAINT fk_parti FOREIGN KEY (partiid) REFERENCES onlineparti(onlinepartiid) ON DELETE CASCADE
                        );
                    ";
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Tabeller opprettet.");
                }
            }
        }
    }
}