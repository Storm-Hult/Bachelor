using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BO25EB_47
{
    internal class StockFishHandler2
    {
        // Full path to the Stockfish executable
        private readonly string stockfishPath = @"C:\Users\Elias\Documents\skole\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe";

        // Tries to find which UCI move leads from one FEN to another
        public string FindUci(string fenBefore, string fenAfter)
        {
            try
            {
                using (var stockfish = new Process())
                {
                    stockfish.StartInfo = new ProcessStartInfo
                    {
                        FileName = stockfishPath,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    stockfish.Start();

                    // Initialize UCI
                    SendCmd(stockfish, "uci");
                    WaitForUciOk(stockfish);
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    // Load the original position
                    SendCmd(stockfish, $"position fen {fenBefore}");
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    // Get all legal moves from the position
                    SendCmd(stockfish, "go perft 1");
                    var moves = new List<string>();
                    string line;
                    DateTime timeout = DateTime.Now.AddSeconds(10);
                    while (DateTime.Now < timeout && (line = stockfish.StandardOutput.ReadLine()) != null)
                    {
                        if (line.Contains(":"))
                        {
                            string[] parts = line.Split(':');
                            string move = parts[0].Trim();
                            if (move.Length == 4 || move.Length == 5)
                                moves.Add(move);
                        }
                        if (line.StartsWith("Nodes searched:") || line.StartsWith("Total nodes:"))
                            break;
                    }

                    // Try each move and compare resulting FEN to fenAfter
                    string boardAfter = fenAfter.Split(' ')[0];
                    foreach (var move in moves)
                    {
                        SendCmd(stockfish, $"position fen {fenBefore} moves {move}");
                        SendCmd(stockfish, "isready");
                        WaitForReadyOk(stockfish);
                        SendCmd(stockfish, "d");

                        string newFen = ReadFenLine(stockfish);
                        if (!string.IsNullOrEmpty(newFen))
                        {
                            string boardNew = newFen.Split(' ')[0];
                            if (boardNew.Equals(boardAfter, StringComparison.Ordinal))
                            {
                                SendCmd(stockfish, "quit");
                                stockfish.WaitForExit();
                                return move;
                            }
                        }
                    }

                    SendCmd(stockfish, "quit");
                    stockfish.WaitForExit();
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Applies a UCI move to a FEN and returns the new FEN
        public string FenMove(string fen, string uciMove)
        {
            try
            {
                using (var stockfish = new Process())
                {
                    stockfish.StartInfo = new ProcessStartInfo
                    {
                        FileName = stockfishPath,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    stockfish.Start();

                    SendCmd(stockfish, "uci");
                    WaitForUciOk(stockfish);
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    SendCmd(stockfish, $"position fen {fen} moves {uciMove}");
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);
                    SendCmd(stockfish, "d");

                    string newFen = ReadFenLine(stockfish);
                    SendCmd(stockfish, "quit");
                    stockfish.WaitForExit();
                    return newFen;
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Sends a command to the Stockfish process
        private void SendCmd(Process process, string cmd)
        {
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.Flush();
        }

        // Waits for "uciok" from Stockfish
        private void WaitForUciOk(Process process)
        {
            DateTime timeout = DateTime.Now.AddSeconds(10);
            string line;
            while (DateTime.Now < timeout && (line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim().Equals("uciok", StringComparison.OrdinalIgnoreCase))
                    return;
            }
        }

        // Waits for "readyok" from Stockfish
        private void WaitForReadyOk(Process process)
        {
            DateTime timeout = DateTime.Now.AddSeconds(10);
            string line;
            while (DateTime.Now < timeout && (line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim().Equals("readyok", StringComparison.OrdinalIgnoreCase))
                    return;
            }
        }

        // Reads the current FEN from the output
        private string ReadFenLine(Process process)
        {
            DateTime timeout = DateTime.Now.AddSeconds(5);
            string line;
            while (DateTime.Now < timeout && (line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Fen:"))
                    return line.Substring(4).Trim();
            }
            return "";
        }

        // Evaluates a position and returns either a centipawn or mate evaluation
        public string EvaluatePosition(string fen, int depth = 20)
        {
            try
            {
                using (var stockfish = new Process())
                {
                    stockfish.StartInfo = new ProcessStartInfo
                    {
                        FileName = stockfishPath,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    stockfish.Start();

                    SendCmd(stockfish, "uci");
                    WaitForUciOk(stockfish);
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    SendCmd(stockfish, $"position fen {fen}");
                    SendCmd(stockfish, $"go depth {depth}");

                    string evaluation = "";
                    string line;
                    DateTime timeout = DateTime.Now.AddSeconds(10);
                    while (DateTime.Now < timeout && (line = stockfish.StandardOutput.ReadLine()) != null)
                    {
                        if (line.Contains("score cp") || line.Contains("score mate"))
                        {
                            evaluation = ExtractEvaluation(line);
                        }
                        if (line.StartsWith("bestmove"))
                            break;
                    }

                    SendCmd(stockfish, "quit");
                    stockfish.WaitForExit();
                    return string.IsNullOrEmpty(evaluation) ? "No evaluation found." : evaluation;
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Parses evaluation from score cp or score mate
        private string ExtractEvaluation(string infoLine)
        {
            if (infoLine.Contains("score mate"))
            {
                int idx = infoLine.IndexOf("score mate");
                string val = infoLine.Substring(idx).Split(' ')[2];
                int mate = int.Parse(val);
                return mate > 0 ? $"Mate in {mate} for white." : $"Mate in {-mate} for black.";
            }
            else if (infoLine.Contains("score cp"))
            {
                int idx = infoLine.IndexOf("score cp");
                string val = infoLine.Substring(idx).Split(' ')[2];
                int cp = int.Parse(val);
                double score = cp / 100.0;
                if (score > 0)
                    return $"White has the advantage of {score}";
                else if (score < 0)
                    return $"Black has the advantage of {score}";
                else
                    return "The position is equal";
            }

            return "Could not extract evaluation";
        }

        // Utility: Replace only the board part of a FEN string
        public static string ReplaceFenPosition(string fen, string position)
        {
            string[] parts = fen.Split(' ');
            if (parts.Length < 6)
                throw new ArgumentException("FEN must contain at least 6 parts.");

            // Rebuild FEN with new board but same state
            string newFen = position + " " + string.Join(" ", parts, 1, parts.Length - 1);
            return newFen;
        }
    }
}
