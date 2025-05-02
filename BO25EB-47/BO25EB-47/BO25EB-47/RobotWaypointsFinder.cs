using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BO25EB_47
{
    class RobotWaypointsFinder
    {
        public static bool IsEmpty(char c)
        {
            return c == '.';
        }

        public static string GetMoveType(string beforeFen, string afterFen, string uciMove)
        {
            var boardBefore = FenToArray.FenToMatrix(beforeFen);
            var boardAfter = FenToArray.FenToMatrix(afterFen);

            (int fromRow, int fromCol) = SquareToCoords(uciMove.Substring(0, 2));
            (int toRow, int toCol) = SquareToCoords(uciMove.Substring(2, 2));

            char movedPiece = boardBefore[fromRow, fromCol];
            char capturedPiece = boardBefore[toRow, toCol];

            bool isCapture = !IsEmpty(capturedPiece) && boardAfter[toRow, toCol] != capturedPiece;
            bool isPromotion = uciMove.Length == 5;
            bool isPawn = char.ToLower(movedPiece) == 'p';

            // En passant
            if (isPawn && IsEmpty(boardBefore[toRow, toCol]) && !IsEmpty(boardAfter[toRow, toCol]) &&
                fromCol != toCol && IsEmpty(boardAfter[fromRow, toCol]))
            {
                return "en passant";
            }

            // Castling
            if (char.ToLower(movedPiece) == 'k' && Math.Abs(fromCol - toCol) == 2)
            {
                return toCol > fromCol ? "castling kingside" : "castling queenside";
            }

            if (isPromotion && isCapture) return "promotion capture";
            if (isPromotion) return "promotion";
            if (isCapture) return "capture";
            return "normal";
        }


        static (int, int) SquareToCoords(string square)
        {
            int col = square[0] - 'a';
            int row = 8 - (square[1] - '0');
            return (row, col);
        }

        public static List<double[]> GetRobotWaypoints(string moveType, string uciMove, string beforeFen, string afterFen)
        {
            var waypoints = new List<double[]>();

            string from = uciMove.Substring(0, 2);
            string to = uciMove.Substring(2, 2);
            var fromPose = GetPoseFromSquare(from);
            var toPose = GetPoseFromSquare(to);

            var offBoardPose = new double[] { 0, -0.7, 0.19, 0, 3.14, 0 };
            var centerPose = new double[] { 0, -0.534, 0.5, 0, 3.14, 0 };

            switch (moveType)
            {
                case "en passant":
                    {
                        int direction = beforeFen.Contains(" w ") ? -1 : 1;
                        string capturedSquare = $"{to[0]}{(char)(to[1] + direction)}";
                        var capPose = GetPoseFromSquare(capturedSquare);

                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.055));
                        waypoints.Add(SetZ(toPose, 0.155));
                        break;
                    }
                case "castling kingside":
                case "castling queenside":
                    {
                        bool isWhite = beforeFen.Contains(" w ");
                        bool kingside = moveType.Contains("kingside");

                        string kingFrom = isWhite ? "e1" : "e8";
                        string kingTo = kingside ? (isWhite ? "g1" : "g8") : (isWhite ? "c1" : "c8");
                        string rookFrom = kingside ? (isWhite ? "h1" : "h8") : (isWhite ? "a1" : "a8");
                        string rookTo = kingside ? (isWhite ? "f1" : "f8") : (isWhite ? "d1" : "d8");

                        var kingFromPose = GetPoseFromSquare(kingFrom);
                        var kingToPose = GetPoseFromSquare(kingTo);
                        var rookFromPose = GetPoseFromSquare(rookFrom);
                        var rookToPose = GetPoseFromSquare(rookTo);

                        // Flytt konge
                        waypoints.Add(SetZ(kingFromPose, 0.155));
                        waypoints.Add(SetZ(kingFromPose, 0.055));
                        waypoints.Add(SetZ(kingFromPose, 0.155));
                        waypoints.Add(SetZ(kingToPose, 0.155));
                        waypoints.Add(SetZ(kingToPose, 0.055));
                        waypoints.Add(SetZ(kingToPose, 0.155));

                        // Flytt tårn
                        waypoints.Add(SetZ(rookFromPose, 0.155));
                        waypoints.Add(SetZ(rookFromPose, 0.055));
                        waypoints.Add(SetZ(rookFromPose, 0.155));
                        waypoints.Add(SetZ(rookToPose, 0.155));
                        waypoints.Add(SetZ(rookToPose, 0.055));
                        waypoints.Add(SetZ(rookToPose, 0.155));
                        break;
                    }
                case "promotion capture":
                    {
                        // Fjern motstanders brikke
                        var capPose = GetPoseFromSquare(to);
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        // Fjern egen brikke
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(offBoardPose);
                        break;
                    }
                case "promotion":
                    {
                        // Fjern egen brikke
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(offBoardPose);
                        break;
                    }
                case "capture":
                    {
                        var capPose = GetPoseFromSquare(to);
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.055));
                        waypoints.Add(SetZ(toPose, 0.155));
                        break;
                    }
                case "normal":
                    {
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.155));
                        waypoints.Add(SetZ(toPose, 0.055));
                        waypoints.Add(SetZ(toPose, 0.155));
                        break;
                    }
                default:
                    break;
            }

            waypoints.Add(centerPose);
            return waypoints;
        }

        static double[] SetZ(double[] pose, double z)
        {
            return new double[] { pose[0], pose[1], z, pose[3], pose[4], pose[5] };
        }

        static double[] GetPoseFromSquare(string square)
        {
            int row = square[0] - 'a';
            int col = square[1] - '1';

            double xA1 = 0.151, yA1 = -0.564;
            double xH1 = 0.148, yH1 = -0.277;
            double xA8 = -0.134, yA8 = -0.564;

            double x = xA1 + (xA8 - xA1) * (col / 7.0);
            double y = yA1 + (yH1 - yA1) * (row / 7.0);

            return new double[] { x, y, 0.16, 0, 3.14, 0 };
        }
        public static void SendWaypointsToPython(List<double[]> waypoints)
        {
            // Serialiser waypoints til JSON
            string json = JsonSerializer.Serialize(waypoints);

            // Angi den fulle banen til Python-skriptet
            string scriptPath = @"C:\Users\Elias\Documents\skole\RobotMovements.py";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python", // Python er lagt til i PATH
                Arguments = $"\"{scriptPath}\" \"{json}\"",
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

                Console.WriteLine("Output fra Python-skriptet:");
                Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("Feilmeldinger:");
                    Console.WriteLine(error);
                }
            }
        }
    }

}