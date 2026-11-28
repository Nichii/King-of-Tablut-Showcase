using System;
using System.Collections.Generic;

namespace Junnav.KingOfTablut.Gameplay
{
    /// <summary>
    /// Records the result of a single move attempt.
    /// </summary>
    public readonly struct MoveResult
    {
        private readonly IReadOnlyList<CapturedPiece> captures;

        public bool Succeeded { get; }
        public InvalidMoveMessage RejectionReason { get; }
        public MoveMessage Message { get; }
        public GameState GameState { get; }

        /// <summary>
        /// Lists the pieces captured by the move. A captured king stays on the board.
        /// </summary>
        public IReadOnlyList<CapturedPiece> Captures => captures ?? Array.Empty<CapturedPiece>();

        internal MoveResult(InvalidMoveMessage rejectionReason, MoveMessage message, GameState gameState,
            List<CapturedPiece> captures = null)
        {
            Succeeded = rejectionReason == InvalidMoveMessage.None;
            RejectionReason = rejectionReason;
            Message = message;
            GameState = gameState;
            this.captures = captures?.AsReadOnly();
        }
    }
}
