using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace BO25EB_47
{
    public partial class FormArchivesPage : Form
    {
        private const string ConnStr = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";
        private string _currentGameType;  // "online" or "bot"

        public FormArchivesPage()
        {
            InitializeComponent();

            LoadGames();  // Load all archived games into dropdown

            // Clear metadata text boxes
            txtArchivesOpponent.Clear();
            txtArchivesPlaysAs.Clear();
            txtArchivesMoveCnt.Clear();
            txtArchivesWinner.Clear();
            txtArchivesDate.Clear();

            // Reset dropdown selections
            cbArchivesGames.SelectedIndex = -1;
            cbArchivesMoveNr.SelectedIndex = -1;

            // Show starting position by default
            DrawBoard.Show(this,
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                'W');
        }

        /* ---------- Navigation: back to home page ---------- */
        private void btnArchivesMenu_Click(object sender, EventArgs e)
        {
            new FormHomePage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            }.Show();
            Close();
        }

        /* ---------- Load games into dropdown ---------- */
        private void LoadGames()
        {
            const string sql = @"
                SELECT onlinepartiid AS id,
                       motstander AS display,
                       'online' AS partitype
                FROM   onlineparti
                UNION ALL
                SELECT botpartiid AS id,
                       'Bot (Difficulty ' || vanskelighetsgrad || ')' AS display,
                       'bot' AS partitype
                FROM   botparti
                ORDER BY id DESC;";

            using var conn = new NpgsqlConnection(ConnStr);
            using var da = new NpgsqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);

            cbArchivesGames.DataSource = dt;
            cbArchivesGames.DisplayMember = "display";
            cbArchivesGames.ValueMember = "id";
        }

        /* ---------- When a game is selected ---------- */
        private void cbArchivesGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchivesGames.SelectedIndex == -1) return;

            var row = (DataRowView)cbArchivesGames.SelectedItem;
            int id = Convert.ToInt32(row["id"]);
            string type = row["partitype"].ToString();  // "online" or "bot"
            _currentGameType = type;

            LoadMoveNumbers(id, type);     // Populate move numbers
            LoadGameDetails(id, type);     // Show game metadata
            LoadFenAndDrawBoard(id, type); // Draw final position of the game
        }

        /* ---------- Load all move numbers for a given game ---------- */
        private void LoadMoveNumbers(int id, string type)
        {
            cbArchivesMoveNr.Items.Clear();

            const string sql = @"
                SELECT trekknr
                FROM   trekk
                WHERE  partiid = @id AND partitype = @type
                ORDER BY 
                    CAST(SUBSTRING(trekknr, 1, LENGTH(trekknr) - 1) AS INTEGER), 
                    CASE WHEN RIGHT(trekknr, 1) = 'w' THEN 0 ELSE 1 END";

            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("type", type);

            using var r = cmd.ExecuteReader();
            while (r.Read()) cbArchivesMoveNr.Items.Add(r.GetString(0));

            cbArchivesMoveNr.SelectedIndex = -1;
        }

        /* ---------- Load game metadata into UI ---------- */
        private void LoadGameDetails(int id, string type)
        {
            string sql = type == "online"
                ? @"SELECT motstander AS opp, farge, antalltrekk, vinner, dato
                   FROM onlineparti WHERE onlinepartiid = @id"
                : @"SELECT 'Bot (Difficulty ' || vanskelighetsgrad || ')' AS opp, 
                          farge, antalltrekk, vinner, dato
                   FROM botparti WHERE botpartiid = @id";

            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return;

            txtArchivesOpponent.Text = r["opp"].ToString();
            txtArchivesPlaysAs.Text = r["farge"].ToString();
            txtArchivesMoveCnt.Text = r["antalltrekk"].ToString();
            txtArchivesWinner.Text = r["vinner"].ToString();
            txtArchivesDate.Text = DateTime.TryParse(r["dato"].ToString(), out var d)
                ? d.ToShortDateString() : "";
        }

        /* ---------- Convert color to orientation ('W' or 'B') ---------- */
        private static char ToOrientation(string playsAs) =>
            !string.IsNullOrEmpty(playsAs) &&
            playsAs.Trim().ToLower().StartsWith("h") ? 'W' : 'B';

        /* ---------- Draw board from last position of the selected game ---------- */
        private void LoadFenAndDrawBoard(int id, string type)
        {
            string join = type == "online"
                ? "JOIN onlineparti op ON t.partiid = op.onlinepartiid"
                : "JOIN botparti bp ON t.partiid = bp.botpartiid";
            string color = type == "online" ? "op.farge" : "bp.farge";

            string sql = $@"
                SELECT t.posisjon AS fen, {color} AS farge
                FROM   trekk t
                {join}
                WHERE  t.partiid = @id AND t.partitype = @type
                ORDER BY t.trekknr DESC
                LIMIT 1;";

            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("type", type);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return;

            DrawBoard.Show(this, r["fen"].ToString(), ToOrientation(r["farge"].ToString()));
        }

        /* ---------- When a specific move is selected ---------- */
        private void cbArchivesMoveNr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchivesMoveNr.SelectedItem == null || cbArchivesGames.SelectedValue == null) return;

            int id = (int)cbArchivesGames.SelectedValue;
            string moveNr = cbArchivesMoveNr.SelectedItem.ToString();

            LoadFenForMove(id, _currentGameType, moveNr);
        }

        /* ---------- Draw board for selected move ---------- */
        private void LoadFenForMove(int id, string type, string moveNr)
        {
            const string sql = @"
                SELECT posisjon
                FROM   trekk
                WHERE  partiid = @id AND partitype = @type AND trekknr LIKE @nr || '%'
                ORDER BY trekknr DESC
                LIMIT 1;";

            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("type", type);
            cmd.Parameters.AddWithValue("nr", moveNr);

            string fen = cmd.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(fen)) return;

            DrawBoard.Show(this, fen, ToOrientation(txtArchivesPlaysAs.Text));
        }

        /* ---------- Button: Compare current board to CNN-detected board ---------- */
        private void btnArchivesSetPos_Click(object sender, EventArgs e)
        {
            // 1. Require selection of both game and move
            if (cbArchivesGames.SelectedIndex == -1 || cbArchivesMoveNr.SelectedItem == null)
            {
                MessageBox.Show("Choose game and move first.");
                return;
            }

            // 2. Run CNN to detect board state from camera image
            string imgPos = CNNHandler.AnalyzeImage();  // e.g., "rnbqkbnr/..."

            // 3. Retrieve FEN from database for selected move
            int gameId = (int)cbArchivesGames.SelectedValue;
            string moveId = cbArchivesMoveNr.SelectedItem.ToString();  // e.g., "5w"

            string fenDb = null;
            using (var conn = new NpgsqlConnection(ConnStr))
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(
                    "SELECT posisjon FROM trekk WHERE partiid=@p AND trekknr LIKE @t || '%' LIMIT 1", conn);
                cmd.Parameters.AddWithValue("p", gameId);
                cmd.Parameters.AddWithValue("t", moveId);
                fenDb = cmd.ExecuteScalar() as string;
            }
            if (fenDb == null)
            {
                MessageBox.Show("FEN not found in DB.");
                return;
            }

            // 4. Remove everything after the first space in FEN
            int comma = fenDb.IndexOf(' ');
            if (comma > -1) fenDb = fenDb[(comma + 1)..].Trim();

            // 5. Compare board part of FEN with CNN result
            string boardDb = fenDb.Split(' ')[0];
            if (!boardDb.Equals(imgPos, StringComparison.Ordinal))
            {
                MessageBox.Show("Image does not match selected position.");
                return;
            }

            // 6. Launch the bot game from this specific FEN position
            var botForm = new FormBotPlayPage
            {
                StartFEN = fenDb
            };
            botForm.Show();

            Close(); // Close the archive form
        }
    }
}
