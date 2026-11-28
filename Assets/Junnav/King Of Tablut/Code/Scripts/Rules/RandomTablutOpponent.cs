using System;
using System.Collections.Generic;

namespace Junnav.KingOfTablut.Gameplay
{
    public class RandomTablutOpponent
    {
        private readonly Random random;

        public RandomTablutOpponent(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>
        /// Chooses one of the current side's legal moves without changing the match.
        /// Returns false when the side has no legal move or the match has ended.
        /// </summary>
        public bool TryChooseMove(TablutGameMode gameMode, out TablutMove move)
        {
            move = default;
            if (gameMode.GameState != GameState.Playing)
            {
                return false;
            }

            List<TablutMove> moves = new();
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    BoardPosition from = new(x, y);
                    if (!gameMode.CanSelectPiece(from))
                    {
                        continue;
                    }

                    foreach (BoardPosition to in gameMode.GetLegalDestinations(from))
                    {
                        moves.Add(new TablutMove(from, to));
                    }
                }
            }

            if (moves.Count == 0)
            {
                return false;
            }

            // Choose from all moves so a piece with fewer destinations does not get extra weight.
            move = moves[random.Next(moves.Count)];
            return true;
        }
    }
}
