using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO25EB_47
{
    internal class StockFishHandler2
    {
        private readonly string stockfishPath = @"C:\Users\Elias\Documents\skole\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe";

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

                    // 1) UCI-init og isready
                    SendCmd(stockfish, "uci");
                    WaitForUciOk(stockfish);
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    // Sett posisjon til fenBefore og henter lovlige trekk
                    SendCmd(stockfish, $"position fen {fenBefore}");
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);
                    SendCmd(stockfish, "go perft 1");

                    var moves = new List<string>();
                    string line;
                    DateTime timeout = DateTime.Now.AddSeconds(10);
                    while (DateTime.Now < timeout && (line = stockfish.StandardOutput.ReadLine()) != null)
                    {
                        if (line.Contains(":"))
                        {
                            string[] parts = line.Split(':');
                            string potentialMove = parts[0].Trim();
                            if (potentialMove.Length == 4 || potentialMove.Length == 5)
                            {
                                moves.Add(potentialMove);
                            }
                        }
                        if (line.StartsWith("Nodes searched:") || line.StartsWith("Total nodes:"))
                        {
                            break;
                        }
                    }

                    // Sammenlign brettdelen i fenAfter med ny posisjon
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
                return "Feil: " + ex.Message;
            }
        }

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

                    // UCI-init og isready
                    SendCmd(stockfish, "uci");
                    WaitForUciOk(stockfish);
                    SendCmd(stockfish, "isready");
                    WaitForReadyOk(stockfish);

                    // Sett posisjon og utfør UCI-trekket
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
                return "Feil: " + ex.Message;
            }
        }

        private void SendCmd(Process process, string cmd)
        {
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.Flush();
        }

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
    }
}
