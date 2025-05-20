using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BO25EB_47
{
    public partial class FormBotPage : Form
    {
        public FormBotPage()
        {
            InitializeComponent();

            // Add options for bot playing as white or black
            cbBotPlaysAs.Items.Clear();
            cbBotPlaysAs.Items.Add("White");   // White
            cbBotPlaysAs.Items.Add("Black");  // Black
            cbBotPlaysAs.SelectedIndex = 0;   // Default: White

            // Add options for difficulty level (1–10)
            cbBotDifficulty.Items.Clear();
            for (int i = 1; i <= 10; i++)
            {
                cbBotDifficulty.Items.Add(i);
            }
            cbBotDifficulty.SelectedIndex = 4; // Default: level 5
        }

        // Go back to main menu
        private void btnBotPageHome_Click(object sender, EventArgs e)
        {
            FormHomePage homePage = new FormHomePage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            homePage.Show();
            this.Close();
        }

        // Start a new bot game with chosen color and difficulty
        private void btnBotNewGame_Click(object sender, EventArgs e)
        {
            string playsAs = cbBotPlaysAs.SelectedItem?.ToString() ?? "";
            int difficulty = Convert.ToInt32(cbBotDifficulty.SelectedItem);

            FormBotPlayPage botPlayPage = new FormBotPlayPage
            {
                BotPlaysAs = playsAs,
                BotDifficulty = difficulty,
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            botPlayPage.Show();
            this.Close();
        }

        // Start a new bot game from the current board position captured from camera
        private void btnBotCustomPos_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Capture board state using CNN (as a FEN board part)
                string board = CNNHandler.AnalyzeImage();  // e.g. "rnbqkbnr/..."

                if (string.IsNullOrWhiteSpace(board))
                    throw new InvalidOperationException("Board detection failed.");

                // 2. Construct full FEN string with default metadata
                string startFen = StockFishHandler2.ReplaceFenPosition(
                    "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                    board);

                // 3. Start a new bot game from the detected position
                var botForm = new FormBotPlayPage
                {
                    StartFEN = startFen
                };
                botForm.Show();

                Close();  // Close the current page
            }
            catch
            {
                MessageBox.Show("Something went wrong, try again");
            }
        }
    }
}
