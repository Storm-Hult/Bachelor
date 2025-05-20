using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BO25EB_47
{
    internal class StockFishHandler
    {
        private Process _stockfishProcess;

        public StockFishHandler()
        {
            StartStockfish();  // Start Stockfish when handler is created
        }

        // Starts the Stockfish process and initializes UCI mode
        private void StartStockfish()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\Users\Elias\Documents\skole\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _stockfishProcess = new Process { StartInfo = psi };
            _stockfishProcess.Start();

            // Initialize UCI
            _stockfishProcess.StandardInput.WriteLine("uci");
            _stockfishProcess.StandardInput.Flush();
            string line;
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim() == "uciok") break;
            }

            // Wait until Stockfish is ready
            _stockfishProcess.StandardInput.WriteLine("isready");
            _stockfishProcess.StandardInput.Flush();
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim() == "readyok") break;
            }
        }

        // Detects which legal move transforms one FEN into another
        public string GetUciMove(string fenBefore, string fenAfter)
        {
            _stockfishProcess.StandardInput.WriteLine($"position fen {fenBefore}");
            _stockfishProcess.StandardInput.WriteLine("d");
            _stockfishProcess.StandardInput.Flush();

            string line;
            string legalMovesLine = null;
            DateTime timeout = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < timeout && (line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Legal moves:"))
                {
                    legalMovesLine = line;
                    break;
                }
            }

            if (string.IsNullOrEmpty(legalMovesLine)) return "";

            string[] tokens = legalMovesLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 3) return "";

            string boardAfter = fenAfter.Split(' ')[0];
            for (int i = 2; i < tokens.Length; i++)
            {
                string move = tokens[i];

                _stockfishProcess.StandardInput.WriteLine($"position fen {fenBefore} moves {move}");
                _stockfishProcess.StandardInput.WriteLine("d");
                _stockfishProcess.StandardInput.Flush();

                string newFen = null;
                DateTime moveTimeout = DateTime.Now.AddSeconds(5);
                while (DateTime.Now < moveTimeout && (line = _stockfishProcess.StandardOutput.ReadLine()) != null)
                {
                    if (line.StartsWith("Fen:"))
                    {
                        newFen = line.Substring(5).Trim();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(newFen))
                {
                    string boardNew = newFen.Split(' ')[0];
                    if (boardNew.Equals(boardAfter, StringComparison.Ordinal))
                        return move;
                }
            }

            return "";
        }

        // Cleanly shuts down the Stockfish process
        public void Close()
        {
            if (_stockfishProcess != null && !_stockfishProcess.HasExited)
            {
                _stockfishProcess.StandardInput.Close();
                _stockfishProcess.WaitForExit();
                _stockfishProcess.Close();
            }
        }

        // Applies a move to a FEN position and returns the new FEN
        public string ApplyUciMove(string fen, string uciMove)
        {
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen} moves {uciMove}");
            _stockfishProcess.StandardInput.Flush();

            _stockfishProcess.StandardInput.WriteLine("d");
            _stockfishProcess.StandardInput.Flush();

            string line;
            string newFen = "";
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Fen:"))
                {
                    newFen = line.Substring(5).Trim();
                    break;
                }
            }

            return newFen;
        }

        // Lets Stockfish find the best move from a given position and search depth
        public string FindMove(string fen, int difficulty)
        {
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen}");
            _stockfishProcess.StandardInput.Flush();

            _stockfishProcess.StandardInput.WriteLine($"go depth {difficulty}");
            _stockfishProcess.StandardInput.Flush();

            string bestMove = "";
            string line;
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove"))
                {
                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 2)
                        bestMove = tokens[1];
                    break;
                }
            }

            return bestMove;
        }

        // Checks if the current position is a stalemate
        public bool IsStalemate(string fen)
        {
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen}");
            _stockfishProcess.StandardInput.WriteLine("go depth 1");
            _stockfishProcess.StandardInput.Flush();

            string line;
            bool hasLegalMoves = false;

            // Check if any legal moves exist
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove") && !line.Contains("(none)"))
                {
                    hasLegalMoves = true;
                    break;
                }
            }

            if (hasLegalMoves) return false;

            // If no moves, check if king is in check (if not, it's stalemate)
            _stockfishProcess.StandardInput.WriteLine("d");
            _stockfishProcess.StandardInput.Flush();

            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Checkers:"))
                    return line.Trim().EndsWith("none");
            }

            return false;
        }
    }
}