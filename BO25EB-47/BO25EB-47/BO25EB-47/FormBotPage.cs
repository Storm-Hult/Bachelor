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
            // Legg til muligheter for bot-farge
            cbBotPlaysAs.Items.Clear();
            cbBotPlaysAs.Items.Add("Hvit");
            cbBotPlaysAs.Items.Add("Svart");
            cbBotPlaysAs.SelectedIndex = 0; // Hvit initialisert

            // Legg til muligheter for bot-difficulty (1 til 10)
            cbBotDifficulty.Items.Clear();
            for (int i = 1; i <= 10; i++)
            {
                cbBotDifficulty.Items.Add(i);
            }
            cbBotDifficulty.SelectedIndex = 4; // Initialiseres til 5 (indeks 4)
        }

        private void btnBotPageHome_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormHomePage
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            homePage.Show();
            this.Close();
        }

        private void btnBotNewGame_Click(object sender, EventArgs e)
        {
            string playsAs = cbBotPlaysAs.SelectedItem?.ToString() ?? "";
            int difficulty = Convert.ToInt32(cbBotDifficulty.SelectedItem);

            FormBotPlayPage botPlayPage = new FormBotPlayPage();
            botPlayPage.BotPlaysAs = playsAs;
            botPlayPage.BotDifficulty = difficulty;
            botPlayPage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            botPlayPage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            botPlayPage.Show();
            this.Close();
        }

        private void btnBotCustomPos_Click(object sender, EventArgs e)
        {

        }
    }
}
