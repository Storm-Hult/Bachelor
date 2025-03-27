using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BO25EB_47
{
    internal class StockFishHandler
    {
        // Global prosess for Stockfish
        private Process _stockfishProcess;

        public StockFishHandler()
        {
            StartStockfish();
        }

        private void StartStockfish()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "C:\\Users\\Elias\\Documents\\skole\\stockfish-windows-x86-64-avx2\\stockfish\\stockfish-windows-x86-64-avx2.exe",  // Juster banen etter behov
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _stockfishProcess = new Process();
            _stockfishProcess.StartInfo = psi;
            _stockfishProcess.Start();

            // Initialiser UCI-modus
            _stockfishProcess.StandardInput.WriteLine("uci");
            _stockfishProcess.StandardInput.Flush();
            string line;
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim() == "uciok")
                    break;
            }

            // Synkroniser med isready
            _stockfishProcess.StandardInput.WriteLine("isready");
            _stockfishProcess.StandardInput.Flush();
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.Trim() == "readyok")
                    break;
            }
        }

        public string GetUciMove(string fenBefore, string fenAfter)
{
    // Sett posisjon til fenBefore og be om visning av posisjon (som skal inneholde "Legal moves:")
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
    if (string.IsNullOrEmpty(legalMovesLine))
    {
        return "";
    }

    // Del opp linjen; forventet format: "Legal moves: e2e4 e2e3 g1f3 ..."
    string[] tokens = legalMovesLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length < 3)
    {
        return "";
    }

    // Sammenlign kun brettdelen (det første feltet) av fenAfter
    string boardAfter = fenAfter.Split(' ')[0];

    // Iterer over alle lovlige trekk (starter fra index 2)
    for (int i = 2; i < tokens.Length; i++)
    {
        string move = tokens[i];

        // Simuler trekket
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
            {
                return move;
            }
        }
    }

    return "";
}


        public void Close()
        {
            if (_stockfishProcess != null && !_stockfishProcess.HasExited)
            {
                _stockfishProcess.StandardInput.Close();
                _stockfishProcess.WaitForExit();
                _stockfishProcess.Close();
            }
        }

        public string ApplyUciMove(string fen, string uciMove)
        {
            // Sett opp posisjonen og utfør trekket
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen} moves {uciMove}");
            _stockfishProcess.StandardInput.Flush();

            // Be Stockfish skrive ut den nye posisjonen
            _stockfishProcess.StandardInput.WriteLine("d");
            _stockfishProcess.StandardInput.Flush();

            string line;
            string newFen = "";
            // Les ut Stockfish sin output til vi finner linjen med "Fen:"
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

        public string FindMove(string fen, int difficulty)
        {
            // Sett opp posisjonen
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen}");
            _stockfishProcess.StandardInput.Flush();

            // Be Stockfish om å finne et trekk med søkedybde lik difficulty
            _stockfishProcess.StandardInput.WriteLine($"go depth {difficulty}");
            _stockfishProcess.StandardInput.Flush();

            string bestMove = "";
            string line;
            // Les output til vi finner linjen med "bestmove"
            while ((line = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove"))
                {
                    // Linjen ser typisk slik ut: "bestmove e2e4 ponder e7e5"
                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 2)
                    {
                        bestMove = tokens[1];
                    }
                    break;
                }
            }
            return bestMove;
        }

        // Implementasjon av ReplaceFenPosition under GetUciMove
        public static string ReplaceFenPosition(string fen, string Position)
        {
            // Split FEN-strengen på mellomrom
            string[] parts = fen.Split(' ');
            if (parts.Length < 6)
            {
                throw new ArgumentException("FEN-strengen må inneholde minst 6 deler.");
            }

            // Bygg ny FEN: erstatt den første delen (posisjonsdelen) med variabelen Position,
            // og behold de resterende delene (aktiv farge, rokade, en passant, etc.)
            string newFen = Position + " " + string.Join(" ", parts, 1, parts.Length - 1);
            return newFen;
        }
    }
}
