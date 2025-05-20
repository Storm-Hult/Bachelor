namespace BO25EB_47
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.Json;

    class RobotWaypointsFinder
    {
        // Checks if a board square is empty
        public static bool IsEmpty(char c)
        {
            return c == '.';
        }

        // Determines the move type based on FENs and UCI move string
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

            // Detect en passant
            if (isPawn && IsEmpty(boardBefore[toRow, toCol]) && !IsEmpty(boardAfter[toRow, toCol]) &&
                fromCol != toCol && IsEmpty(boardAfter[fromRow, toCol]))
            {
                return "en passant";
            }

            // Detect castling
            if (char.ToLower(movedPiece) == 'k' && Math.Abs(fromCol - toCol) == 2)
            {
                return toCol > fromCol ? "castling kingside" : "castling queenside";
            }

            if (isPromotion && isCapture) return "promotion capture";
            if (isPromotion) return "promotion";
            if (isCapture) return "capture";
            return "normal";
        }

        // Converts chess square notation to matrix coordinates
        static (int, int) SquareToCoords(string square)
        {
            int col = square[0] - 'a';
            int row = 8 - (square[1] - '0');
            return (row, col);
        }

        // Returns list of robot waypoints for a given move
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

                        // Capture opponent pawn
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        // Move own pawn
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

                        // Define king and rook positions
                        string kingFrom = isWhite ? "e1" : "e8";
                        string kingTo = kingside ? (isWhite ? "g1" : "g8") : (isWhite ? "c1" : "c8");
                        string rookFrom = kingside ? (isWhite ? "h1" : "h8") : (isWhite ? "a1" : "a8");
                        string rookTo = kingside ? (isWhite ? "f1" : "f8") : (isWhite ? "d1" : "d8");

                        var kingFromPose = GetPoseFromSquare(kingFrom);
                        var kingToPose = GetPoseFromSquare(kingTo);
                        var rookFromPose = GetPoseFromSquare(rookFrom);
                        var rookToPose = GetPoseFromSquare(rookTo);

                        // Move king
                        waypoints.Add(SetZ(kingFromPose, 0.155));
                        waypoints.Add(SetZ(kingFromPose, 0.055));
                        waypoints.Add(SetZ(kingFromPose, 0.155));
                        waypoints.Add(SetZ(kingToPose, 0.155));
                        waypoints.Add(SetZ(kingToPose, 0.055));
                        waypoints.Add(SetZ(kingToPose, 0.155));

                        // Move rook
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
                        var capPose = GetPoseFromSquare(to);

                        // Remove opponent piece
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        // Remove promoting pawn
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(offBoardPose);
                        break;
                    }
                case "promotion":
                    {
                        // Remove promoting pawn
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(SetZ(fromPose, 0.055));
                        waypoints.Add(SetZ(fromPose, 0.155));
                        waypoints.Add(offBoardPose);
                        break;
                    }
                case "capture":
                    {
                        var capPose = GetPoseFromSquare(to);

                        // Remove captured piece
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(SetZ(capPose, 0.055));
                        waypoints.Add(SetZ(capPose, 0.155));
                        waypoints.Add(offBoardPose);

                        // Move attacker
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
                        // Standard move: pick up and place
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

            // Return to center pose
            waypoints.Add(centerPose);
            return waypoints;
        }

        // Set new Z value in a pose (preserve XY + orientation)
        static double[] SetZ(double[] pose, double z)
        {
            return new double[] { pose[0], pose[1], z, pose[3], pose[4], pose[5] };
        }

        // Converts chess square (e.g., "e4") to a robot (x, y) pose
        static double[] GetPoseFromSquare(string square)
        {
            int row = square[0] - 'a';
            int col = square[1] - '1';

            double xA1 = 0.151, yA1 = -0.564;
            double xH1 = 0.148, yH1 = -0.277;
            double xA8 = -0.134, yA8 = -0.564;

            double x = xA1 + (xA8 - xA1) * (col / 7.0);
            double y = yA1 + (yH1 - yA1) * (row / 7.0);

            return new double[] { x, y, 0.16, 0, 3.14, 0 };  // Constant height + orientation
        }

        // Sends the list of waypoints to Python as JSON and runs the script
        public static void SendWaypointsToPython(List<double[]> waypoints)
        {
            string json = JsonSerializer.Serialize(waypoints);
            string scriptPath = @"C:\Users\Elias\Documents\skole\RobotMovements.py";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
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

                Console.WriteLine("Output from Python script:");
                Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("Errors:");
                    Console.WriteLine(error);
                }
            }
        }
    }
}