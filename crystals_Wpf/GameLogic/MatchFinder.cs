using crystals_Wpf.Models;
using System.Collections.Generic;

namespace crystals_Wpf.GameLogic
{
    public class MatchFinder
    {
        public List<Crystal> FindMatches(Crystal[,] board)
        {
            List<Crystal> matches =
                new List<Crystal>();

            FindHorizontal(board, matches);

            FindVertical(board, matches);

            return matches;
        }

        private void FindHorizontal(
            Crystal[,] board,
            List<Crystal> matches)
        {
            for (int row = 0; row < 8; row++)
            {
                int count = 1;

                for (int column = 1; column < 8; column++)
                {
                    if (board[row, column].Type ==
                        board[row, column - 1].Type)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= 3)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                Crystal crystal =
                                    board[row,
                                    column - 1 - i];

                                if (!matches.Contains(crystal))
                                {
                                    matches.Add(crystal);
                                }
                            }
                        }

                        count = 1;
                    }
                }

                if (count >= 3)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Crystal crystal =
                            board[row, 7 - i];

                        if (!matches.Contains(crystal))
                        {
                            matches.Add(crystal);
                        }
                    }
                }
            }
        }

        private void FindVertical(
            Crystal[,] board,
            List<Crystal> matches)
        {
            for (int column = 0; column < 8; column++)
            {
                int count = 1;

                for (int row = 1; row < 8; row++)
                {
                    if (board[row, column].Type ==
                        board[row - 1, column].Type)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= 3)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                Crystal crystal =
                                    board[row - 1 - i,
                                    column];

                                if (!matches.Contains(crystal))
                                {
                                    matches.Add(crystal);
                                }
                            }
                        }

                        count = 1;
                    }
                }

                if (count >= 3)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Crystal crystal =
                            board[7 - i, column];

                        if (!matches.Contains(crystal))
                        {
                            matches.Add(crystal);
                        }
                    }
                }
            }
        }
    }
}