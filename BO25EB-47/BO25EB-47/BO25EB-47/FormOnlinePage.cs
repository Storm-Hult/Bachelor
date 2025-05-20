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

        // Navigate back to the home screen
        private void btnOnlineHome_Click(object sender, EventArgs e)
        {
            FormHomePage homePage = new FormHomePage
            {
                FormBorderStyle = FormBorderStyle.None,    // Remove borders
                WindowState = FormWindowState.Maximized    // Launch in fullscreen
            };
            homePage.Show();
            this.Close();  // Close the current form
        }

        // Start a new online game
        private void btnOnlineStart_Click(object sender, EventArgs e)
        {
            FormOnlinePlayPage onlinePlayPage = new FormOnlinePlayPage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            onlinePlayPage.Show();
            this.Close();  // Close the setup page
        }
    }
}
