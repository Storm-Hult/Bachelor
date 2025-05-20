using Emgu.CV.Mcc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BO25EB_47
{
    public partial class FormBotPlayPage : Form
    {
        private CancellationTokenSource _cts;

        public string BotPlaysAs { get; set; }   // "White" or "Black"
        public int BotDifficulty { get; set; }   // 1–10
        public string StartFEN                   // Can override default start position
        {
            get => CurrentFEN;
            set => CurrentFEN = value;
        }

        int Difficulty;
        char PlaysAs;       // 'W' or 'B'
        string Turn;        // "Player" or "Bot"

        string CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string LastFEN;
        string Position;
        string TempFen;
        string UCIMove;
        string Winner = "none";

        int MoveCount = 1;
        bool CheckMate = false;
        int _botGameId;

        public FormBotPlayPage()
        {
            InitializeComponent();
            Load += FormBotPlayPage_Load;
            FormClosed += FormBotPlayPage_FormClosed;
        }

        // Cancel background tasks when form closes
        private void FormBotPlayPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        // Run when form is loaded
        private async void FormBotPlayPage_Load(object sender, EventArgs e)
        {
            Difficulty = BotDifficulty;

            if (!string.IsNullOrEmpty(BotPlaysAs) && BotPlaysAs.Equals("Black", StringComparison.OrdinalIgnoreCase))
            {
                PlaysAs = 'B';
                Turn = "Bot";  // bot starts
            }
            else
            {
                PlaysAs = 'W';
                Turn = "Player";
            }

            // Register game in database
            _botGameId = DBHandler.CreateBotGame(Difficulty, 0, "none", PlaysAs, DateTime.Now);

            // Use custom FEN if provided
            if (!string.IsNullOrEmpty(StartFEN))
                CurrentFEN = StartFEN;

            // Show starting board
            DrawBoard.Show(this, CurrentFEN, PlaysAs);

            // Show bot config info
            MessageBox.Show($"You're playing as: {BotPlaysAs}\nDifficulty: {Difficulty}", "Bot Settings");

            _cts = new CancellationTokenSource();
            _ = MoveHandler(_cts.Token); // begin turn loop
        }

        // Exit and optionally save game
        private void btnBotPlayExit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you want to save the game to the archives?", "Save game",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                DBHandler.UpdateBotGameMoveCount(_botGameId, MoveCount);
            else
                DBHandler.DeleteBotGame(_botGameId);

            var home = new FormHomePage
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            home.Show();
            Close();
        }

        // Main game loop — alternates between player and bot
        public async Task MoveHandler(CancellationToken token)
        {
            while (!CheckMate)
            {
                token.ThrowIfCancellationRequested();

                if (Turn == "Player")
                {
                    try
                    {
                        await Task.Delay(100, token);

                        // Capture board from camera
                        Position = await Task.Run(() => CNNHandler.AnalyzeImage(), token);
                        MessageBox.Show($"{Position}");

                        // Update temporary FEN with latest board
                        TempFen = StockFishHandler2.ReplaceFenPosition(CurrentFEN, Position);

                        // Skip if same position as last
                        if (TempFen == LastFEN) continue;
                        LastFEN = TempFen;

                        var checker = new StockFishHandler2();

                        // Compare positions and generate move made
                        UCIMove = await Task.Run(() => checker.FindUci(CurrentFEN, TempFen), token);

                        if (!string.IsNullOrEmpty(UCIMove))
                        {
                            CurrentFEN = await Task.Run(() => checker.FenMove(CurrentFEN, UCIMove), token);
                            DrawBoard.Show(this, CurrentFEN, PlaysAs);
                            Turn = "Bot";

                            DBHandler.SaveBotMove(_botGameId, UCIMove, CurrentFEN);
                            MoveCount++;
                            DBHandler.UpdateBotGameMoveCount(_botGameId, MoveCount);

                            if (IsCheckmate())
                            {
                                Winner = "Player";
                                EndGameAndAsk();
                            }
                            else if (IsStalemate())
                            {
                                Winner = "None";
                                EndGameAndAsk();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
                else // Bot's turn
                {
                    try
                    {
                        await Task.Delay(100, token);

                        var checker = new StockFishHandler();
                        string botMove = await Task.Run(() => checker.FindMove(CurrentFEN, Difficulty * 2), token);

                        if (!string.IsNullOrEmpty(botMove))
                        {
                            TempFen = CurrentFEN;
                            UCIMove = botMove;
                            CurrentFEN = await Task.Run(() => checker.ApplyUciMove(CurrentFEN, botMove), token);

                            DrawBoard.Show(this, CurrentFEN, PlaysAs);
                            Turn = "Player";

                            ExecuteMove(); // Send move to robot
                            DBHandler.SaveBotMove(_botGameId, UCIMove, CurrentFEN);
                            MoveCount++;
                            DBHandler.UpdateBotGameMoveCount(_botGameId, MoveCount);

                            if (IsCheckmate())
                            {
                                Winner = "Bot";
                                EndGameAndAsk();
                            }
                            else if (IsStalemate())
                            {
                                Winner = "None";
                                EndGameAndAsk();
                            }
                        }

                        checker.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
        }

        // Sends the robot move via Python bridge
        private void ExecuteMove()
        {
            string movetype = RobotWaypointsFinder.GetMoveType(TempFen, CurrentFEN, UCIMove);
            List<double[]> waypoints = RobotWaypointsFinder.GetRobotWaypoints(movetype, UCIMove, TempFen, CurrentFEN);
            RobotWaypointsFinder.SendWaypointsToPython(waypoints);
        }

        // Evaluate current position with Stockfish2
        private static string EvalPos(string fen)
        {
            var sf2 = new StockFishHandler2();
            return sf2.EvaluatePosition(fen);
        }

        // Check if current FEN is checkmate
        public bool IsCheckmate()
        {
            StockFishHandler sf = new StockFishHandler();
            string mv = sf.FindMove(CurrentFEN, 1);
            return string.IsNullOrEmpty(mv);
        }

        // Check if current FEN is stalemate
        private bool IsStalemate()
        {
            StockFishHandler sf = new StockFishHandler();
            return sf.IsStalemate(CurrentFEN);
        }

        // Finalize game and prompt user for save
        private void EndGameAndAsk()
        {
            CheckMate = true;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            DBHandler.UpdateBotGameMoveCount(_botGameId, MoveCount);

            var ans = MessageBox.Show(
                Winner == "Stalemate"
                ? "The game ended in stalemate!  Do you want to save the game?"
                : $"{Winner} won!  Do you want to save the game?",
                "Game over",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (ans == DialogResult.Yes)
            {
                DBHandler.UpdateBotGameWinner(
                    _botGameId,
                    Winner == "Stalemate" ? "draw" : Winner.ToLower());
            }
        }

        // Button: show best move
        private void btnBotPlayBestMove_Click(object sender, EventArgs e)
        {
            var sf = new StockFishHandler();
            string best = sf.FindMove(CurrentFEN, 20);
            MessageBox.Show($"Best move: {best}");
        }

        // Button: evaluate current position
        private void btnBotPlayEval_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Position evaluation: {EvalPos(CurrentFEN)}");
        }
    }
}
