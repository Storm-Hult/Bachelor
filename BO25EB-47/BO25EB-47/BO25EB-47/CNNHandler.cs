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
using Emgu.CV.Reg;
using Emgu.CV.Util;

namespace BO25EB_47
{
    public class CNNHandler
    {
        // Main function to analyze the chessboard image and return the position as FEN
        public static string AnalyzeImage()
        {
            string position = "";
            CaptureImage(); // Take a new image from webcam
            char[,] boardMatrix = RunCNNAndGetBoardMatrix(); // Run CNN and get board as 2D matrix
            if (boardMatrix != null)
            {
                // Convert 2D board matrix to FEN string
                position = FenToArray.MatrixToFEN(boardMatrix);
            }
            return position;
        }

        // Captures an image from the webcam and saves it
        private static void CaptureImage()
        {
            string savePath = @"C:\Users\Elias\Camera_folder\image_test.png";

            // Delete previous image if it exists
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            try
            {
                // Open webcam (index 1, using DirectShow backend)
                VideoCapture capture = new VideoCapture(1, VideoCapture.API.DShow);
                if (!capture.IsOpened)
                {
                    Console.WriteLine("Could not open camera.");
                    return;
                }

                // Set camera properties (resolution, exposure, etc.)
                capture.Set(CapProp.FrameWidth, 3840);
                capture.Set(CapProp.FrameHeight, 2160);
                capture.Set(CapProp.Gain, 14);
                capture.Set(CapProp.Brightness, 50);
                capture.Set(CapProp.Focus, 28);
                capture.Set(CapProp.Contrast, 65);
                capture.Set(CapProp.Saturation, 60);
                capture.Set(CapProp.Gamma, 1);
                capture.Set(CapProp.AutoExposure, 0.75);
                capture.Set(CapProp.AutoWb, 1); // enable auto white balance

                // Warm up camera (grab frames for ~0.5s)
                for (int i = 0; i < 30; ++i) capture.Grab();

                // Lock exposure before capture
                capture.Set(CapProp.AutoExposure, 0.25);
                capture.Set(CapProp.Exposure, -6);

                // Capture a single frame
                Mat frame = new Mat();
                capture.Read(frame);
                if (frame.IsEmpty)
                {
                    Console.WriteLine("No frame captured.");
                    return;
                }

                // Adjust white balance and brightness
                FixColor(frame);
                CvInvoke.ConvertScaleAbs(frame, frame, 1.1, 10); // Slight brightness/contrast boost

                // Save frame to file
                CvInvoke.Imwrite(savePath, frame);
                Console.WriteLine($"Image saved to: {savePath}");

                // Release camera resources
                capture.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Calls a Python script that runs the CNN and returns a board matrix
        private static char[,] RunCNNAndGetBoardMatrix()
        {
            char[,] boardMatrix = null;
            string scriptPath = @"C:\Users\Elias\Documents\skole\CNN.py"; // Path to Python script

            // Set up the Python process
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",                         // Use Python interpreter
                Arguments = $"\"{scriptPath}\"",             // Pass script path as argument
                RedirectStandardOutput = true,               // Capture script output
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    // Read output (board string)
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    // Ensure output is valid length (64 = 8x8 board)
                    if (output.Length != 64)
                    {
                        Console.WriteLine($"Error: Invalid output length ({output.Length}). Expected 64 characters.");
                        return null;
                    }

                    // Convert flat string to 2D char array
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
                Console.WriteLine($"Error running Python script: {ex.Message}");
            }

            return boardMatrix;
        }

        // Adjusts blue/red channels to balance white color in the image
        private static void FixColor(Mat img)
        {
            // Compute average color
            MCvScalar mean = CvInvoke.Mean(img);
            double gray = (mean.V0 + mean.V1 + mean.V2) / 3.0;
            double sB = gray / mean.V0;
            double sR = gray / mean.V2;

            using (VectorOfMat bgr = new VectorOfMat())
            {
                // Split image into BGR channels
                CvInvoke.Split(img, bgr);

                // Scale blue and red channels
                CvInvoke.ConvertScaleAbs(bgr[0], bgr[0], sB, 0);
                CvInvoke.ConvertScaleAbs(bgr[2], bgr[2], sR, 0);

                // Merge channels back into image
                CvInvoke.Merge(bgr, img);
            }
        }
    }
}
