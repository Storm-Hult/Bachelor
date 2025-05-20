namespace BO25EB_47
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application configuration (e.g. DPI, fonts)
            ApplicationConfiguration.Initialize();

            // Ensure PostgreSQL is running and database is ready
            DBInitialize.InitializeDB();

            // Path to the Python script that moves the robot to its start pose
            string scriptPath = @"C:\Users\Elias\Documents\skole\StartPose.py";

            // Set up process to call Python script
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",                    // Python must be in PATH
                Arguments = $"\"{scriptPath}\"",        // Quote path to allow spaces
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Run Python script and log any output or errors
            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("Errors:");
                    Console.WriteLine(error);
                }
            }

            // Start the application with the home screen in fullscreen mode
            FormHomePage homePage = new FormHomePage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            homePage.Show();

            // Register cleanup handler to stop PostgreSQL on exit
            Application.ApplicationExit += OnApplicationExit;

            Application.Run(); // Main message loop
            Application.Run(new FormHomePage());
        }

        // Stop PostgreSQL service when application exits
        private static void OnApplicationExit(object sender, EventArgs e)
        {
            DBInitialize.StopPostgreSQL();
        }
    }
}
