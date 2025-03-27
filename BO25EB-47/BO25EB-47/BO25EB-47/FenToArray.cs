using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO25EB_47
{
    class FenToArray
    {
        public static char[,] FenToMatrix(string fen)
        {
            // Finn første mellomrom
            int spaceIndex = fen.IndexOf(' ');
            if (spaceIndex == -1) throw new ArgumentException("Ugyldig FEN-streng.");

            // Hent kun brettdelen (før første mellomrom)
            string boardPart = fen.Substring(0, spaceIndex);
            char[,] board = new char[8, 8];

            // Konverter FEN til 8x8 brett
            string[] rows = boardPart.Split('/');
            if (rows.Length != 8) throw new ArgumentException("Ugyldig FEN-struktur.");

            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char c in rows[row])
                {
                    if (char.IsDigit(c)) // Tomme felter
                    {
                        col += c - '0';
                    }
                    else // Brikker
                    {
                        board[row, col] = c;
                        col++;
                    }
                }
            }

            return board;
        }

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
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fenRow += emptyCount.ToString();
                            emptyCount = 0;
                        }
                        fenRow += cell;
                    }
                }
                if (emptyCount > 0)
                {
                    fenRow += emptyCount.ToString();
                }
                fenRows[i] = fenRow;
            }
            return string.Join("/", fenRows);
        }
    }
}
