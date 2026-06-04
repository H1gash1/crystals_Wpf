using crystals_Wpf.Models;
using System;

namespace crystals_Wpf.GameLogic
{
    public class BoardManager
    {
        private const int Rows = 8;
        private const int Columns = 8;

        private Random random = new Random();

        public Crystal[,] GenerateBoard()
        {
            Crystal[,] board =
                new Crystal[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    board[row, column] =
                        new Crystal
                        {
                            Row = row,
                            Column = column,
                            Type = GenerateType(
                                board,
                                row,
                                column)
                        };
                }
            }

            return board;
        }

        private int GenerateType(
            Crystal[,] board,
            int row,
            int column)
        {
            while (true)
            {
                int type =
                    random.Next(0, 5);

                bool horizontalMatch =
                    column >= 2 &&
                    board[row, column - 1] != null &&
                    board[row, column - 2] != null &&
                    board[row, column - 1].Type == type &&
                    board[row, column - 2].Type == type;

                bool verticalMatch =
                    row >= 2 &&
                    board[row - 1, column] != null &&
                    board[row - 2, column] != null &&
                    board[row - 1, column].Type == type &&
                    board[row - 2, column].Type == type;

                if (!horizontalMatch &&
                    !verticalMatch)
                {
                    return type;
                }
            }
        }
    }
}