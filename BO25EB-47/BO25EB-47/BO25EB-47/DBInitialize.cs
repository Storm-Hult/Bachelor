using System;
using System.Diagnostics;
using Npgsql;

namespace BO25EB_47
{
    public class DBInitialize
    {
        // Entry point for database setup
        public static void InitializeDB()
        {
            // Start PostgreSQL service if not already running
            StartPostgreSQL();

            if (DatabaseExists("sjakkdb"))
            {
                // Uncomment to reset and recreate tables if needed
                //ResetTables();
                //CreateTables();
            }
            else
            {
                // Create the database and tables if they don't exist
                CreateDatabase();
                CreateTables();
            }
        }

        // Starts the PostgreSQL Windows service
        private static void StartPostgreSQL()
        {
            string startCommand = "net start postgresql-x64-17";
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {startCommand}",
                Verb = "runas",            // Run as administrator
                UseShellExecute = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        throw new Exception("Could not start PostgreSQL. Exit code: " + process.ExitCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not start PostgreSQL: " + ex.Message);
            }
        }

        // Stops the PostgreSQL Windows service
        public static void StopPostgreSQL()
        {
            string stopCommand = "net stop postgresql-x64-17";
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {stopCommand}",
                Verb = "runas",            // Run as administrator
                UseShellExecute = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    Console.WriteLine("PostgreSQL stopped.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping PostgreSQL: " + ex.Message);
            }
        }

        // Checks if the database exists
        private static bool DatabaseExists(string databaseName)
        {
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

        // Creates the database (connects to 'postgres' to do this)
        private static void CreateDatabase()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=postgres";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CREATE DATABASE sjakkdb", conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Database 'sjakkdb' created.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not create database: {ex.Message}");
                    }
                }
            }
        }

        // Deletes existing tables (if needed)
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
                    Console.WriteLine("Existing tables dropped.");
                }
            }
        }

        // Creates the required tables if they don't already exist
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
                            partiid INT NOT NULL,
                            trekknr VARCHAR(10) NOT NULL,
                            tidbrukt INTERVAL,
                            resterendetid INTERVAL,
                            notasjon VARCHAR(10),
                            posisjon VARCHAR(100),
                            evalbar DOUBLE PRECISION,
                            partitype VARCHAR(10),
                            PRIMARY KEY (partiid, trekknr),
                            CONSTRAINT fk_parti
                            FOREIGN KEY (partiid)
                            REFERENCES botparti(botpartiid)
                            ON DELETE CASCADE
                        );
                    ";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Tables created.");
                }
            }
        }
    }
}