namespace BO25EB_47
{
    public partial class FormHomePage : Form
    {
        public FormHomePage()
        {
            InitializeComponent();
        }

        private void btnHomeBot_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormBotPage
            FormBotPage botPage = new FormBotPage();
            botPage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            botPage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            botPage.Show();
            this.Close();
        }

        private void btnHomeOnline_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormOnlinePage
            FormOnlinePage onlinePage = new FormOnlinePage();
            onlinePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            onlinePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            onlinePage.Show();
            this.Close();
        }

        private void btnHomeArchives_Click(object sender, EventArgs e)
        {
            // Opprett en ny instans av FormOnlinePage
            FormArchivesPage archivesPage = new FormArchivesPage();
            archivesPage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            archivesPage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            archivesPage.Show();
            this.Close();
        }

        private void btnHomeExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Application.Exit(); // required to close a fullscreen application
        }
    }
}
