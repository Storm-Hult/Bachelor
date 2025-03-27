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
    public partial class FormOnlinePlayPage : Form
    {
        public FormOnlinePlayPage()
        {
            InitializeComponent();
            // Anta at du allerede har en metode som konverterer FEN til en 8x8 matrise
            string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            char[,] chessBoard = FenToArray.FenToMatrix(fen); // Kaller din eksisterende FEN-parser

            // Opprett sjakkbrett-kontrollen
            DrawBoardClass drawBoard = new DrawBoardClass();
            drawBoard.DrawBoard(800, 160, chessBoard, 'B'); // Tegn brettet på (20,20)

            // Legg til sjakkbrettet i skjemaet
            this.Controls.Add(drawBoard);
        }

        private void btnOnlinePlayResign_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormHomePage
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            homePage.Show();
            this.Close();
        }
    }
}
