using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BO25EB_47
{
    public partial class FormArchivesPage : Form
    {
        public string connectionString = "Host=localhost;Username=postgres;Password=Zf2ddiae!!;Database=sjakkdb";

        public FormArchivesPage()
        {
            InitializeComponent();

            // Last inn partier i comboboxene uten automatisk valg
            LoadGames();

            // Nullstill tekstbokser
            txtArchivesOpponent.Text = "";
            txtArchivesPlaysAs.Text = "";
            txtArchivesMoveCnt.Text = "";
            txtArchivesWinner.Text = "";
            txtArchivesDate.Text = "";


            cbArchivesGames.SelectedIndex = -1;
            cbArchivesMoveNr.SelectedIndex = -1;

            // Tegn startbrettet med standard FEN for startposisjon
            string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            TegnBrettMedFen(startFen, 'W');
        }

        private void btnArchivesMenu_Click(object sender, EventArgs e)
        {
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;
            homePage.WindowState = FormWindowState.Maximized;
            homePage.Show();
            this.Close();
        }

        private void cbArchivesGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchivesGames.SelectedIndex != -1 && cbArchivesGames.SelectedValue != null)
            {
                DataRowView drv = (DataRowView)cbArchivesGames.SelectedItem;
                int onlinePartiID = Convert.ToInt32(drv["onlinepartiid"]);

                // Oppdater trekk-liste og spilldetaljer
                LoadMoveNumbers(onlinePartiID);
                LoadGameDetails(onlinePartiID);
            }
        }

        private void LoadMoveNumbers(int onlinePartiID)
        {
            int antallTrekk = 0;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                // Henter antall trekk fra onlineparti-tabellen (merk at kolonnenavn er små bokstaver)
                using (var cmd = new NpgsqlCommand("SELECT antalltrekk FROM onlineparti WHERE onlinepartiid = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", onlinePartiID);
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int trekk))
                    {
                        antallTrekk = trekk;
                    }
                }
            }
            cbArchivesMoveNr.Items.Clear();
            for (int i = 1; i <= antallTrekk; i++)
            {
                cbArchivesMoveNr.Items.Add(i);
            }
            // Fjern automatisk valg – ingen trekk er valgt ved sideinnlasting
            cbArchivesMoveNr.SelectedIndex = -1;
        }

        private void LoadGames()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                using (var cmd = new NpgsqlCommand("SELECT onlinepartiid, motstander FROM onlineparti", conn))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                cbArchivesGames.DataSource = dt;
                cbArchivesGames.DisplayMember = "motstander";   // Viser motstander
                cbArchivesGames.ValueMember = "onlinepartiid";    // Bruker onlinepartiid som verdi
                // Ikke velg noe automatisk – la SelectedIndex være -1
            }
        }

        private void LoadGameDetails(int onlinePartiID)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT motstander, farge, antalltrekk, vinner, dato FROM onlineparti WHERE onlinepartiid = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", onlinePartiID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtArchivesOpponent.Text = reader["motstander"].ToString();
                            txtArchivesPlaysAs.Text = reader["farge"].ToString();
                            txtArchivesMoveCnt.Text = reader["antalltrekk"].ToString();
                            txtArchivesWinner.Text = reader["vinner"].ToString();
                            if (DateTime.TryParse(reader["dato"].ToString(), out DateTime dato))
                            {
                                txtArchivesDate.Text = dato.ToShortDateString();
                            }
                            else
                            {
                                txtArchivesDate.Text = "";
                            }
                        }
                    }
                }
            }
        }

        // Denne metoden brukes til å tegne et brett gitt en FEN-streng og farge
        private void TegnBrettMedFen(string fen, char farge)
        {
            if (!string.IsNullOrEmpty(fen))
            {
                char[,] chessBoard = FenToArray.FenToMatrix(fen);
                DrawBoardClass drawBoard = new DrawBoardClass();
                drawBoard.DrawBoard(800, 160, chessBoard, farge);

                // Fjern tidligere tegnet brett
                var previousBoards = this.Controls.OfType<DrawBoardClass>().ToList();
                foreach (var board in previousBoards)
                {
                    this.Controls.Remove(board);
                    board.Dispose();
                }
                this.Controls.Add(drawBoard);
                drawBoard.BringToFront();
                this.Refresh();
            }
        }

        private void LoadFenAndDrawBoard(int onlinePartiID)
        {
            string fen = "";
            string playsAs = "";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT t.posisjon AS fen,
                   op.farge
            FROM trekk t
            JOIN onlineparti op ON t.partiid = op.onlinepartiid
            WHERE op.onlinepartiid = @id
            ORDER BY t.trekknr DESC
            LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("id", onlinePartiID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fen = reader["fen"].ToString();
                            playsAs = reader["farge"].ToString();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(fen))
            {
                char[,] chessBoard = FenToArray.FenToMatrix(fen);
                DrawBoardClass drawBoard = new DrawBoardClass();

                // Bestem brettfarge basert på "farge"
                char boardColor = 'B';
                if (!string.IsNullOrEmpty(playsAs))
                {
                    if (playsAs.ToLower().StartsWith("h") || playsAs.ToUpper().Equals("W"))
                    {
                        boardColor = 'W';
                    }
                }

                drawBoard.DrawBoard(800, 160, chessBoard, boardColor);

                // Fjern tidligere brett
                var previousBoards = this.Controls.OfType<DrawBoardClass>().ToList();
                foreach (var board in previousBoards)
                {
                    this.Controls.Remove(board);
                    board.Dispose();
                }

                this.Controls.Add(drawBoard);
                drawBoard.BringToFront();
                this.Refresh();
            }
        }


        private void cbArchivesMoveNr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchivesMoveNr.SelectedItem != null && cbArchivesGames.SelectedValue != null)
            {
                DataRowView drv = (DataRowView)cbArchivesGames.SelectedItem;
                int onlinePartiID = Convert.ToInt32(drv["onlinepartiid"]);
                // moveNumber er lagret som en streng (selv om vi legger til tall), så vi konverterer til string
                string moveNumber = cbArchivesMoveNr.SelectedItem.ToString();
                LoadFenForMoveAndDrawBoard(onlinePartiID, moveNumber);
            }
        }

        // Henter FEN fra trekk-tabellen basert på parti-ID og valgt trekknummer
        private void LoadFenForMoveAndDrawBoard(int onlinePartiID, string moveNumber)
        {
            string fen = "";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string trekknrDb = $"P1-M{moveNumber}"; // Blir "P1-M10"
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT posisjon FROM trekk WHERE partiid = @partiid AND trekknr = @trekknrDb", conn))
                {
                    cmd.Parameters.AddWithValue("partiid", onlinePartiID);
                    cmd.Parameters.AddWithValue("trekknrDb", trekknrDb);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        fen = result.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(fen))
            {

                // Bruk fargen fra txtArchivesPlaysAs for å bestemme brettfargen
                char boardColor = 'B';
                if (!string.IsNullOrEmpty(txtArchivesPlaysAs.Text))
                {
                    if (txtArchivesPlaysAs.Text.ToLower().StartsWith("h") || txtArchivesPlaysAs.Text.ToUpper().Equals("W"))
                    {
                        boardColor = 'W';
                    }
                }
                TegnBrettMedFen(fen, boardColor);
            }
        }
    }
}