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
            string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            char[,] chessBoard = FenToArray.FenToMatrix(fen);

            DrawBoard.Show(this, fen, 'B');
        }

        private void btnOnlinePlayResign_Click(object sender, EventArgs e)
        {
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;
            homePage.WindowState = FormWindowState.Maximized;
            homePage.Show();
            this.Close();
        }
    }
}
