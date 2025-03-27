using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;

namespace BO25EB_47
{
    public class CNNHandler
    {
        public static string AnalyzeImage()
        {
            string position = "";
            CaptureImage();
            char[,] boardMatrix = RunCNNAndGetBoardMatrix();
            if(boardMatrix != null)
            {
                position = FenToArray.MatrixToFEN(boardMatrix);
            }
            return position;
        }
        private static void CaptureImage()
        {
            string savePath = @"C:\Users\Elias\Camera_folder\image_test.png";
            // Slett eventuelt eksisterende bilde
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            try
            {
                // Åpne webkamera
                VideoCapture capture = new VideoCapture(1, VideoCapture.API.DShow);
                if (!capture.IsOpened)
                {
                    Console.WriteLine("Kunne ikke åpne kameraet.");
                    return;
                }

                capture.Set(CapProp.FrameWidth, 3840);
                capture.Set(CapProp.FrameHeight, 2160);
                capture.Set(CapProp.Gain, 30);
                capture.Set(CapProp.Exposure, -6);
                capture.Set(CapProp.Brightness, 25);
                capture.Set(CapProp.Focus, 28);
                capture.Set(CapProp.Contrast, 65);
                capture.Set(CapProp.Saturation, 60);
                capture.Set(CapProp.Gamma, 1);

                double exp = capture.Get(CapProp.Exposure);
                Console.WriteLine("Exposure: " + exp);
                double focus = capture.Get(CapProp.Focus);
                Console.WriteLine("Focus: " + focus);
                double gain = capture.Get(CapProp.Gain);
                Console.WriteLine("Gain: " + gain);

                // Ta et bilde
                Mat frame = new Mat();
                capture.Read(frame);
                if (frame.IsEmpty)
                {
                    Console.WriteLine("Ingen bilder ble tatt.");
                    return;
                }

                // Lagre bildet
                CvInvoke.Imwrite(savePath, frame);
                Console.WriteLine($"Bilde lagret til: {savePath}");

                // Frigjør ressurser
                capture.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil: {ex.Message}");
            }
        }

        private static char[,] RunCNNAndGetBoardMatrix()
        {
            char[,] boardMatrix = null;
            string scriptPath = @"C:\Users\Elias\Documents\skole\CNN.py";  // Oppdater med riktig sti

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    // Les output-strengen fra Python-scriptet
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    // Sjekk at output har riktig lengde
                    if (output.Length != 64)
                    {
                        Console.WriteLine($"Feil: Ugyldig output-lengde ({output.Length}). Forventet 64 tegn.");
                        return null;
                    }

                    // Konverter output til en 8x8 matrise
                    boardMatrix = new char[8, 8];
                    int index = 0;
                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 8; col++)
                        {
                            boardMatrix[row, col] = output[index];
                            index++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil ved kjøring av Python-script: {ex.Message}");
            }

            return boardMatrix;
        }
    }
}