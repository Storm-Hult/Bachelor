using Npgsql;
using System;

namespace BO25EB_47
{
    public class DBHandler
    {
        public static void SaveOnlineMove(
            TimeSpan timeSpent, TimeSpan timeRemaining,
            string notation, string position, int gameId)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                int moveCount;
                using (var countCmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM trekk WHERE partitype = 'online' AND partiid = @gameId", conn))
                {
                    countCmd.Parameters.AddWithValue("gameId", gameId);
                    moveCount = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                int moveNumber = moveCount / 2 + 1;
                string color = (moveCount % 2 == 0) ? "w" : "b";
                string moveId = $"{moveNumber}{color}";

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        INSERT INTO trekk (
                            trekknr, tidbrukt, resterendetid, notasjon, posisjon,
                            partitype, partiid
                        ) VALUES (
                            @moveId, @timeSpent, @timeRemaining, @notation, @position,
                            'online', @gameId
                        );";

                    cmd.Parameters.AddWithValue("moveId", moveId);
                    cmd.Parameters.AddWithValue("timeSpent", timeSpent);
                    cmd.Parameters.AddWithValue("timeRemaining", timeRemaining);
                    cmd.Parameters.AddWithValue("notation", notation);
                    cmd.Parameters.AddWithValue("position", position);
                    cmd.Parameters.AddWithValue("gameId", gameId);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Online move {moveId} saved.");
                }
            }
        }

        public static void SaveBotMove(string notation, string position)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                int moveCount;
                using (var countCmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM trekk WHERE partitype = 'bot'", conn))
                {
                    moveCount = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                int moveNumber = moveCount / 2 + 1;
                string color = (moveCount % 2 == 0) ? "w" : "b";
                string moveId = $"{moveNumber}{color}";

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        INSERT INTO trekk (
                            trekknr, tidbrukt, resterendetid, notasjon, posisjon,
                            partitype, partiid
                        ) VALUES (
                            @moveId, NULL, NULL, @notation, @position,
                            'bot', NULL
                        );";

                    cmd.Parameters.AddWithValue("moveId", moveId);
                    cmd.Parameters.AddWithValue("notation", notation);
                    cmd.Parameters.AddWithValue("position", position);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Bot move {moveId} saved.");
                }
            }
        }

        public static void CreateOnlineGame(
            string opponent, int rating, string timeControl,
            int moveCount, string winner, char playerColor, DateTime date)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        INSERT INTO onlineparti (
                            motstander, rating, tidsformat, antalltrekk,
                            vinner, farge, dato
                        ) VALUES (
                            @opponent, @rating, @timeControl, @moveCount,
                            @winner, @playerColor, @date
                        );";

                    cmd.Parameters.AddWithValue("opponent", opponent);
                    cmd.Parameters.AddWithValue("rating", rating);
                    cmd.Parameters.AddWithValue("timeControl", timeControl);
                    cmd.Parameters.AddWithValue("moveCount", moveCount);
                    cmd.Parameters.AddWithValue("winner", winner);
                    cmd.Parameters.AddWithValue("playerColor", playerColor);
                    cmd.Parameters.AddWithValue("date", date);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Online game created.");
                }
            }
        }

        public static void CreateBotGame(
            int difficulty, int moveCount,
            string winner, char playerColor, DateTime date)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        INSERT INTO botparti (
                            vanskelighetsgrad, antalltrekk, vinner, farge, dato
                        ) VALUES (
                            @difficulty, @moveCount, @winner, @playerColor, @date
                        );";

                    cmd.Parameters.AddWithValue("difficulty", difficulty);
                    cmd.Parameters.AddWithValue("moveCount", moveCount);
                    cmd.Parameters.AddWithValue("winner", winner);
                    cmd.Parameters.AddWithValue("playerColor", playerColor);
                    cmd.Parameters.AddWithValue("date", date);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Bot game created.");
                }
            }
        }

        public static void UpdateOnlineGameMoveCount(int moveCount)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                int lastGameId;
                using (var cmd = new NpgsqlCommand("SELECT MAX(onlinepartiid) FROM onlineparti", conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value)
                    {
                        Console.WriteLine("No online games found.");
                        return;
                    }
                    lastGameId = Convert.ToInt32(result);
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                UPDATE onlineparti
                SET antalltrekk = @moveCount
                WHERE onlinepartiid = @gameId;";

                    cmd.Parameters.AddWithValue("moveCount", moveCount);
                    cmd.Parameters.AddWithValue("gameId", lastGameId);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Updated last online game (ID {lastGameId}) with {moveCount} moves.");
                }
            }
        }

        public static void UpdateBotGameMoveCount(int moveCount)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                int lastGameId;
                using (var cmd = new NpgsqlCommand("SELECT MAX(botpartiid) FROM botparti", conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value)
                    {
                        Console.WriteLine("No bot games found.");
                        return;
                    }
                    lastGameId = Convert.ToInt32(result);
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                UPDATE botparti
                SET antalltrekk = @moveCount
                WHERE botpartiid = @gameId;";

                    cmd.Parameters.AddWithValue("moveCount", moveCount);
                    cmd.Parameters.AddWithValue("gameId", lastGameId);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Updated last bot game (ID {lastGameId}) with {moveCount} moves.");
                }
            }
        }

    }
}
