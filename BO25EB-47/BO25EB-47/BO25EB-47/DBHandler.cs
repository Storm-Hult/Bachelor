using Npgsql;
using System;

namespace BO25EB_47
{
    public class DBHandler
    {
        // Save a move made in an online game to the database
        public static void SaveOnlineMove(
            TimeSpan timeSpent, TimeSpan timeRemaining,
            string notation, string position, int gameId)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                // Count existing moves for this online game
                int moveCount;
                using (var countCmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM trekk WHERE partitype = 'online' AND partiid = @gameId", conn))
                {
                    countCmd.Parameters.AddWithValue("gameId", gameId);
                    moveCount = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                // Determine move number and color (e.g., 1w, 1b, 2w, etc.)
                int moveNumber = moveCount / 2 + 1;
                string color = (moveCount % 2 == 0) ? "w" : "b";
                string moveId = $"{moveNumber}{color}";

                // Insert the move into the 'trekk' table
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

        // Save a move made by a bot (or against a bot)
        public static void SaveBotMove(int gameId, string notation, string position)
        {
            const string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            // Count previous bot moves for this game
            const string countSql =
                "SELECT COUNT(*) FROM trekk WHERE partitype = 'bot' AND partiid = @id";
            using var countCmd = new NpgsqlCommand(countSql, conn);
            countCmd.Parameters.AddWithValue("id", gameId);
            int moveCount = Convert.ToInt32(countCmd.ExecuteScalar());

            int moveNumber = moveCount / 2 + 1;
            string color = (moveCount % 2 == 0) ? "w" : "b";
            string moveId = $"{moveNumber}{color}";

            // Insert the move into the 'trekk' table
            const string insertSql = @"
        INSERT INTO trekk (
            trekknr, notasjon, posisjon,
            partitype, partiid
        ) VALUES (
            @moveId, @notation, @position,
            'bot', @gameId);";

            using var cmd = new NpgsqlCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("moveId", moveId);
            cmd.Parameters.AddWithValue("notation", notation);
            cmd.Parameters.AddWithValue("position", position);
            cmd.Parameters.AddWithValue("gameId", gameId);

            cmd.ExecuteNonQuery();
            Console.WriteLine($"Bot move {moveId} saved for game {gameId}.");
        }

        // Create a new online game entry in the database
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

        // Create a new bot game and return the new game ID
        public static int CreateBotGame(
            int difficulty, int moveCount,
            string winner, char playerColor, DateTime date)
        {
            const string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            // RETURNING botpartiid lets us retrieve the new game ID
            var sql = @"
        INSERT INTO botparti (vanskelighetsgrad, antalltrekk, vinner, farge, dato)
        VALUES (@difficulty, @moveCount, @winner, @playerColor, @date)
        RETURNING botpartiid;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("difficulty", difficulty);
            cmd.Parameters.AddWithValue("moveCount", moveCount);
            cmd.Parameters.AddWithValue("winner", winner);
            cmd.Parameters.AddWithValue("playerColor", playerColor);
            cmd.Parameters.AddWithValue("date", date);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Update the number of moves in the most recent online game
        public static void UpdateOnlineGameMoveCount(int moveCount)
        {
            string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                // Find the latest game ID
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

                // Update move count
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

        // Update move count for a specific bot game
        public static void UpdateBotGameMoveCount(int gameId, int moveCount)
        {
            const string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            const string sql = @"
        UPDATE botparti
        SET antalltrekk = @mc
        WHERE botpartiid = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("mc", moveCount);
            cmd.Parameters.AddWithValue("id", gameId);

            cmd.ExecuteNonQuery();
        }

        // Delete a specific bot game and all its associated moves
        public static void DeleteBotGame(int gameId)
        {
            const string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            const string sql = @"
        DELETE FROM trekk    WHERE partitype = 'bot' AND partiid   = @id;
        DELETE FROM botparti WHERE botpartiid = @id;";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", gameId);
            cmd.ExecuteNonQuery();
        }

        // Update the winner field for a given bot game
        public static void UpdateBotGameWinner(int gameId, string winner)
        {
            const string connStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE botparti SET vinner = @w WHERE botpartiid = @id", conn);
            cmd.Parameters.AddWithValue("w", winner);   // e.g., "player" or "bot"
            cmd.Parameters.AddWithValue("id", gameId);
            cmd.ExecuteNonQuery();
        }
    }
}
