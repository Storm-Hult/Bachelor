using System;
using System.Drawing;
using System.Windows.Forms;

namespace BO25EB_47
{
    internal class DrawBoardClass : Control
    {
        private const int tileSize = 100;      // Størrelse på rutene
        private const int gridSize = 8;        // 8x8 sjakkbrett
        private readonly Font pieceFont = new Font("Arial", 32, FontStyle.Bold);
        private char[,] board = new char[8, 8];
        // 'W' for standard (hvite nederst) og 'B' for flippet (svart nederst)
        private char boardOrientation = 'W';

        public DrawBoardClass()
        {
            this.Size = new Size(tileSize * gridSize, tileSize * gridSize);
        }

        // Utvidet metode: legg til parameteren orientation ('W' eller 'B')
        public void DrawBoard(int x, int y, char[,] chessBoard, char orientation)
        {
            if (chessBoard.GetLength(0) != gridSize || chessBoard.GetLength(1) != gridSize)
                throw new ArgumentException("Må være en 8x8 sjakkmatrise.");

            this.board = chessBoard;
            // Sett orientering – alt annet enn 'B' tolkes som 'W'
            this.boardOrientation = (orientation == 'B') ? 'B' : 'W';
            this.Location = new Point(x, y);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (boardOrientation == 'W')
            {
                // White-orientering: rader inverteres for at rank 1 (board[0,*]) skal ligge nederst.
                for (int r = 0; r < gridSize; r++)
                {
                    int drawRow = gridSize - 1 - r;
                    for (int c = 0; c < gridSize; c++)
                    {
                        // Bestem rutefarge basert på board-indeks slik at A1 (board[0,0]) blir mørk
                        Brush tileBrush = ((r + c) % 2 == 0) ? Brushes.Gray : Brushes.LightGray;
                        int xPos = c * tileSize;
                        int yPos = drawRow * tileSize;
                        g.FillRectangle(tileBrush, xPos, yPos, tileSize, tileSize);
                        g.DrawRectangle(Pens.Black, xPos, yPos, tileSize, tileSize);

                        char piece = board[r, c];
                        if (piece != '.')
                        {
                            string pieceSymbol = GetPieceSymbol(piece);
                            Brush pieceBrush = char.IsUpper(piece) ? Brushes.Black : Brushes.White;
                            SizeF textSize = g.MeasureString(pieceSymbol, pieceFont);
                            float pieceX = xPos + (tileSize - textSize.Width) / 2;
                            float pieceY = yPos + (tileSize - textSize.Height) / 2;
                            g.DrawString(pieceSymbol, pieceFont, pieceBrush, pieceX, pieceY);
                        }
                    }
                }
            }
            else // boardOrientation == 'B'
            {
                // Black-orientering: brettet roteres 180°.
                // Tegningskoordinatene for hver rute beregnes slik at:
                // •   x = (gridSize - 1 - c) * tileSize
                // •   y = r * tileSize
                // For å sikre korrekt fargebruk (at nederst til venstre i black-view er mørk),
                // regnes en "normalisert" indeks (som flipper board-indeksene) for fargevalget.
                for (int r = 0; r < gridSize; r++)
                {
                    for (int c = 0; c < gridSize; c++)
                    {
                        int drawCol = gridSize - 1 - c;
                        int drawRow = r;
                        // Normaliserte indekser (flippes) for å beregne rutefarge slik at
                        // den ruten som skal være A1 i black-view blir mørk.
                        int normRow = gridSize - 1 - r;
                        int normCol = gridSize - 1 - c;
                        Brush tileBrush = ((normRow + normCol) % 2 == 0) ? Brushes.Gray : Brushes.LightGray;

                        int xPos = drawCol * tileSize;
                        int yPos = drawRow * tileSize;
                        g.FillRectangle(tileBrush, xPos, yPos, tileSize, tileSize);
                        g.DrawRectangle(Pens.Black, xPos, yPos, tileSize, tileSize);

                        char piece = board[r, c];
                        if (piece != '.')
                        {
                            string pieceSymbol = GetPieceSymbol(piece);
                            Brush pieceBrush = char.IsUpper(piece) ? Brushes.Black : Brushes.White;
                            SizeF textSize = g.MeasureString(pieceSymbol, pieceFont);
                            float pieceX = xPos + (tileSize - textSize.Width) / 2;
                            float pieceY = yPos + (tileSize - textSize.Height) / 2;
                            g.DrawString(pieceSymbol, pieceFont, pieceBrush, pieceX, pieceY);
                        }
                    }
                }
            }
        }

        // Konverterer FEN-tegn til Unicode-sjakkbrikker
        private string GetPieceSymbol(char piece)
        {
            return piece switch
            {
                'r' => "♜",
                'n' => "♞",
                'b' => "♝",
                'q' => "♛",
                'k' => "♚",
                'p' => "♟",
                'R' => "♖",
                'N' => "♘",
                'B' => "♗",
                'Q' => "♕",
                'K' => "♔",
                'P' => "♙",
                _ => ""
            };
        }

        public void TegnBrettMedFen(string fen, char farge)
        {
            if (!string.IsNullOrEmpty(fen))
            {
                char[,] chessBoard = FenToArray.FenToMatrix(fen);
                DrawBoardClass drawBoard = new DrawBoardClass();
                drawBoard.DrawBoard(800, 160, chessBoard, farge);

                // Fjern tidligere tegnet brett
                var previousBoards = this.Controls.OfType<DrawBoardClass>().ToList();
                foreach (var board in previousBoards)
                {
                    this.Controls.Remove(board);
                    board.Dispose();
                }
                this.Controls.Add(drawBoard);
                drawBoard.BringToFront();
                this.Refresh();
            }
        }
    }
}
