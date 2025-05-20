using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BO25EB_47
{
    /// <summary>
    /// Draws a chessboard in any parent control.
    /// Usage: DrawBoard.Show(parent, fen, orientation [, x, y])
    /// </summary>
    public static class DrawBoard
    {
        private const int TileSize = 100, GridSize = 8;
        private static readonly Font PieceFont = new Font("Arial", 32, FontStyle.Bold);
        private static BoardControl _current;   // stores the currently drawn board

        public static void Show(Control parent, string fen, char orientation, int x = 800, int y = 160)
        {
            if (parent == null || string.IsNullOrWhiteSpace(fen)) return;

            // Remove previously displayed board, if any
            if (_current != null)
            {
                parent.Controls.Remove(_current);
                _current.Dispose();
            }

            // Convert FEN to 2D board array and create new board control
            _current = new BoardControl(FenToArray.FenToMatrix(fen), orientation)
            {
                Location = new Point(x, y)
            };

            // Add to parent control and bring to front
            parent.Controls.Add(_current);
            _current.BringToFront();
            parent.Refresh();
        }

        /* ---------- Internal custom control that draws the board ---------- */
        private sealed class BoardControl : Control
        {
            private readonly char[,] _board;
            private readonly char _ori;  // 'W' for white orientation, 'B' for black

            public BoardControl(char[,] board, char ori)
            {
                if (board.GetLength(0) != GridSize || board.GetLength(1) != GridSize)
                    throw new ArgumentException("Must be an 8×8 matrix");
                _board = board;
                _ori = (ori == 'B') ? 'B' : 'W';  // default to white if invalid
                Size = new Size(TileSize * GridSize, TileSize * GridSize);
                DoubleBuffered = true;  // reduce flickering
            }

            // Custom drawing logic
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;

                for (int r = 0; r < GridSize; r++)
                {
                    // Flip rows (always) and columns (if black's perspective)
                    int drawRow = GridSize - 1 - r;
                    for (int c = 0; c < GridSize; c++)
                    {
                        int drawCol = (_ori == 'B') ? GridSize - 1 - c : c;

                        // A1 should be dark: dark if (row + col) % 2 == 1
                        Brush tile = ((r + c) % 2 == 0) ? Brushes.LightGray : Brushes.Gray;

                        int x = drawCol * TileSize;
                        int y = drawRow * TileSize;

                        g.FillRectangle(tile, x, y, TileSize, TileSize);
                        g.DrawRectangle(Pens.Black, x, y, TileSize, TileSize);

                        char p = _board[r, c];
                        if (p != '.')
                        {
                            // Get Unicode symbol and color
                            string sym = GetSymbol(p);
                            Brush brush = char.IsUpper(p) ? Brushes.White : Brushes.Black;
                            SizeF ts = g.MeasureString(sym, PieceFont);

                            // Center piece symbol inside tile
                            g.DrawString(sym, PieceFont, brush,
                                         x + (TileSize - ts.Width) / 2,
                                         y + (TileSize - ts.Height) / 2);
                        }
                    }
                }
            }

            // Map character to Unicode chess symbol
            private static string GetSymbol(char p) => p switch
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
    }
}
