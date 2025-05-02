using Emgu.CV.Mcc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BO25EB_47
{
    public partial class FormBotPlayPage : Form
    {
        private CancellationTokenSource _cts;
        public string BotPlaysAs { get; set; }
        public int BotDifficulty { get; set; }

        int Difficulty;
        char PlaysAs;
        string Turn;
        string CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string LastFEN;
        string Position;
        string TempFen;
        string UCIMove;
        string Winner;
        bool CheckMate = false;
        DateTime CreationTime;
        int MoveCount = 1;

        private DrawBoardClass _drawBoard;
        // Hei
        public FormBotPlayPage()
        {
            InitializeComponent();
            this.Load += new EventHandler(FormBotPlayPage_Load);
            this.FormClosed += FormBotPlayPage_FormClosed;
        }

        private void FormBotPlayPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }


        private async void FormBotPlayPage_Load(object sender, EventArgs e)
        {
            // Initialisering av variabler, tegning osv.
            Difficulty = BotDifficulty;
            if (!string.IsNullOrEmpty(BotPlaysAs) && BotPlaysAs.ToLower() == "svart")
            {
                PlaysAs = 'B';
                Turn = "Bot";
            }
            else
            {
                PlaysAs = 'W';
                Turn = "Player";
            }

            DBHandler.CreateBotGame(Difficulty, 0, Winner, PlaysAs, CreationTime);

            char[,] chessBoard = FenToArray.FenToMatrix(CurrentFEN);
            DrawBoardClass drawBoard = new DrawBoardClass();
            drawBoard.DrawBoard(800, 160, chessBoard, PlaysAs);
            this.Controls.Add(drawBoard);

            MessageBox.Show($"Du spiller som: {BotPlaysAs}\nVanskelighetsgrad: {Difficulty}", "Bot Settings");

            // Start MoveHandler uten å await
            _cts = new CancellationTokenSource();
            _ = MoveHandler(_cts.Token);
        }

        private void btnBotPlayExit_Click(object sender, EventArgs e)
        {
            // må legges til lagre alternativ dersom antall trekk >= x, og må avslutte alle prosesser
            // Opprett en ny instans av FormHomePage
            FormHomePage homePage = new FormHomePage();
            homePage.FormBorderStyle = FormBorderStyle.None;  // Fjern kantlinjer og tittel
            homePage.WindowState = FormWindowState.Maximized; // Fullskjerm
            // Vis det nye skjemaet
            homePage.Show();
            this.Close();
        }

        public async Task MoveHandler(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (Turn == "Player")
                {
                    try
                    {

                        // Unngå busy-waiting
                        await Task.Delay(100, token);

                        // Kjør bildeanalysen på en bakgrunnstråd slik at UI ikke blokkeres
                        Position = await Task.Run(() => CNNHandler.AnalyzeImage(), token);
                        // Erstatt posisjonsdelen i den nåværende FEN med den nye posisjonsdelen
                        TempFen = StockFishHandler.ReplaceFenPosition(CurrentFEN, Position);
                        
                        // Hvis posisjonsdelen ikke har endret seg, vent og prøv igjen
                        if (TempFen == LastFEN)
                        {
                            continue;
                        }
                        else
                        {
                            LastFEN = TempFen;
                        }

                        // Opprett en ny StockFishHandler (kan eventuelt gjenbrukes om ønskelig)
                        StockFishHandler2 checker = new StockFishHandler2();

                        // Kjør simuleringen (kan også pakkes inn i Task.Run hvis den er tung)
                        // MessageBox.Show($"Temp Fen: {TempFen}");
                        UCIMove = await Task.Run(() => checker.FindUci(CurrentFEN, TempFen), token);
                        if (!string.IsNullOrEmpty(UCIMove))
                        {
                            CurrentFEN = await Task.Run(() => checker.FenMove(CurrentFEN, UCIMove), token);
                            //_drawBoard.TegnBrettMedFen(CurrentFEN, PlaysAs);
                            Turn = "Bot";
                            DBHandler.SaveBotMove(UCIMove, CurrentFEN);
                            MoveCount++;
                            DBHandler.UpdateBotGameMoveCount(MoveCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"En feil oppstod: {ex.Message}");
                    }
                }
                else if (Turn == "Bot")
                {
                    try
                    {
                        await Task.Delay(100);
                        StockFishHandler checker = new StockFishHandler();
                        // Finn et trekk med ønsket søkedybde
                        string botMove = await Task.Run(() => checker.FindMove(CurrentFEN, Difficulty * 2), token);
                        if (!string.IsNullOrEmpty(botMove))
                        {
                            TempFen = CurrentFEN;
                            UCIMove = botMove;
                            CurrentFEN = await Task.Run(() => checker.ApplyUciMove(CurrentFEN, botMove), token);
                            ExecuteMove();
                            //_drawBoard.TegnBrettMedFen(CurrentFEN, PlaysAs);
                            Turn = "Player";
                            DBHandler.SaveBotMove(UCIMove, CurrentFEN);
                            MoveCount++;
                            DBHandler.UpdateBotGameMoveCount(MoveCount);
                        }
                        checker.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"En feil oppstod: {ex.Message}");
                    }
                }
            }
        }
        private void ExecuteMove()
        {
            string movetype = RobotWaypointsFinder.GetMoveType(TempFen, CurrentFEN, UCIMove);
            List<double[]> waypoints = RobotWaypointsFinder.GetRobotWaypoints(movetype, UCIMove, TempFen, CurrentFEN);
            RobotWaypointsFinder.SendWaypointsToPython(waypoints);
        }
    }
}