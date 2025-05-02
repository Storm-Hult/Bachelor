namespace BO25EB_47

{
    using System;
    using System.Diagnostics;
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
            string scriptPath = @"C:\Users\Elias\Documents\skole\StartPose.py";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python", // Python er lagt til i PATH
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("Feilmeldinger:");
                    Console.WriteLine(error);
                }
            }
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            homePage.Show();
            Application.ApplicationExit += OnApplicationExit;
            Application.Run();
            Application.Run(new FormHomePage());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            DBInitialize.StopPostgreSQL();
        }
    }
}