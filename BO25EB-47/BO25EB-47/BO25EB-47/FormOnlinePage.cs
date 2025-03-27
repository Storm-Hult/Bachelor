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
    public partial class FormOnlinePage : Form
    {
        public FormOnlinePage()
        {
            InitializeComponent();
        }

        private void btnOnlineHome_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormHomePage
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            homePage.Show();
            this.Close();
        }

        private void btnOnlineStart_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormOnlinePlayPage
            FormOnlinePlayPage onlinePlayPage = new FormOnlinePlayPage();
            onlinePlayPage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            onlinePlayPage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            onlinePlayPage.Show();
            this.Close();
        }
    }
}
