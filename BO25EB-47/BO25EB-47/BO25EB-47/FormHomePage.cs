namespace BO25EB_47
{
    public partial class FormHomePage : Form
    {
        public FormHomePage()
        {
            InitializeComponent();
        }

        // Navigate to bot game setup
        private void btnHomeBot_Click(object sender, EventArgs e)
        {
            FormBotPage botPage = new FormBotPage
            {
                FormBorderStyle = FormBorderStyle.None,    // Remove window borders
                WindowState = FormWindowState.Maximized    // Fullscreen
            };
            botPage.Show();
            this.Close();  // Close current (home) form
        }

        // Navigate to online game setup
        private void btnHomeOnline_Click(object sender, EventArgs e)
        {
            FormOnlinePage onlinePage = new FormOnlinePage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            onlinePage.Show();
            this.Close();
        }

        // Navigate to archived games view
        private void btnHomeArchives_Click(object sender, EventArgs e)
        {
            FormArchivesPage archivesPage = new FormArchivesPage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            archivesPage.Show();
            this.Close();
        }

        // Exit the entire application
        private void btnHomeExit_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Close the application completely
        }
    }
}
