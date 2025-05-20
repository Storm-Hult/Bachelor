using System;

namespace BO25EB_47
{
    class FenToArray
    {
        // Converts a FEN string to a 2D board matrix (char[8,8])
        public static char[,] FenToMatrix(string fen)
        {
            int spaceIndex = fen.IndexOf(' ');
            if (spaceIndex == -1) throw new ArgumentException("Invalid FEN string.");

            string boardPart = fen.Substring(0, spaceIndex);
            char[,] board = new char[8, 8];

            // Initialize all squares to '.' (empty)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = '.';
                }
            }

            // Split FEN into 8 rows
            string[] rows = boardPart.Split('/');
            if (rows.Length != 8) throw new ArgumentException("Invalid FEN format.");

            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char c in rows[row])
                {
                    if (char.IsDigit(c))
                    {
                        // Empty squares — advance column
                        col += c - '0';
                    }
                    else
                    {
                        // Place piece
                        board[row, col] = c;
                        col++;
                    }
                }
            }

            return board;
        }

        // Converts a 2D board matrix (char[8,8]) to a FEN string (position only)
        public static string MatrixToFEN(char[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string[] fenRows = new string[rows];

            for (int i = 0; i < rows; i++)
            {
                string fenRow = "";
                int emptyCount = 0;

                for (int j = 0; j < cols; j++)
                {
                    char cell = board[i, j];
                    if (cell == '.')
                    {
                        // Count consecutive empty squares
                        emptyCount++;
                    }
                    else
                    {
                        // Append empty count (if any) before a piece
                        if (emptyCount > 0)
                        {
                            fenRow += emptyCount.ToString();
                            emptyCount = 0;
                        }
                        fenRow += cell; // Add piece
                    }
                }

                // Append trailing empty squares (if any)
                if (emptyCount > 0)
                {
                    fenRow += emptyCount.ToString();
                }

                fenRows[i] = fenRow;
            }

            return string.Join("/", fenRows); // Combine rows into full FEN position
        }
    }
}