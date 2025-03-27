namespace BO25EB_47

{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            DBInitialize.InitializeDB();
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            homePage.Show();
            Application.Run();
            Application.Run(new FormHomePage());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            DBInitialize.StopPostgreSQL();
        }
    }
}