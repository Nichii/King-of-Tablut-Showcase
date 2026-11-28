using System;
using System.Collections.Generic;

namespace Junnav.KingOfTablut.Gameplay
{
    public static class TablutReplay
    {
        /// <summary>
        /// Rebuilds a match from its starting layout and ordered moves. If a move is rejected,
        /// returns its zero-based index and rejection reason and leaves the output match null.
        /// A null layout uses the standard setup with attackers to move.
        /// </summary>
        /// <exception cref="ArgumentNullException">The move history is null.</exception>
        /// <exception cref="ArgumentException">The starting layout or starting side is invalid.</exception>
        public static bool TryReplay(IEnumerable<TablutMove> moves, out TablutGameMode gameMode,
            out (int MoveIndex, InvalidMoveMessage Reason)? failure,
            TablutPieceType[,] startingPositions = null, TablutPlayer startingPlayer = TablutPlayer.Attacker)
        {
            if (moves == null)
            {
                throw new ArgumentNullException(nameof(moves));
            }

            TablutGameMode replay = new(startingPositions, startingPlayer);
            int moveIndex = 0;
            foreach (TablutMove move in moves)
            {
                MoveResult result = replay.MovePieceWithResult(move.From, move.To);
                if (!result.Succeeded)
                {
                    gameMode = null;
                    failure = (moveIndex, result.RejectionReason);
                    return false;
                }

                moveIndex++;
            }

            gameMode = replay;
            failure = null;
            return true;
        }
    }
}
