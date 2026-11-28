using System;
using Junnav.KingOfTablut.Gameplay;
using UnityEngine;

namespace Junnav.KingOfTablut
{
    [CreateAssetMenu(fileName = "Tablut Scenario", menuName = "Tablut/Scenario")]
    public class TablutScenario : ScriptableObject
    {
        [Serializable]
        private struct PiecePlacement
        {
            public TablutPieceType pieceType;
            public int x;
            public int y;
        }

        [SerializeField] private string scenarioName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private TablutPlayer startingPlayer = TablutPlayer.Attacker;
        [SerializeField] private PiecePlacement[] pieces = Array.Empty<PiecePlacement>();

        public string ScenarioName => string.IsNullOrWhiteSpace(scenarioName) ? name : scenarioName;
        public string Description => description;

        /// <summary>
        /// Creates a fresh match from this asset or returns a reason the layout cannot be played.
        /// </summary>
        public bool TryCreateMatch(out TablutGameMode gameMode, out string error)
        {
            gameMode = null;
            TablutPieceType[,] layout = new TablutPieceType[TablutBoard.TotalHorizontalSquares, TablutBoard.TotalVerticalSquares];
            PiecePlacement[] placements = pieces ?? Array.Empty<PiecePlacement>();
            foreach (PiecePlacement piece in placements)
            {
                BoardPosition square = new(piece.x, piece.y);
                if (piece.x < 0 || piece.x >= layout.GetLength(0) || piece.y < 0 || piece.y >= layout.GetLength(1))
                {
                    error = $"Square {square} is outside the board.";
                    return false;
                }

                if (piece.pieceType != TablutPieceType.BlackPawn && piece.pieceType != TablutPieceType.WhitePawn
                    && piece.pieceType != TablutPieceType.WhiteKing)
                {
                    error = $"Square {square} has an invalid piece type.";
                    return false;
                }

                if (layout[piece.x, piece.y] != TablutPieceType.Empty)
                {
                    error = $"Square {square} has more than one piece.";
                    return false;
                }

                if (square == TablutBoard.CastleSquare && piece.pieceType != TablutPieceType.WhiteKing)
                {
                    error = "In a scenario, only the king can start on the throne.";
                    return false;
                }

                if (piece.pieceType == TablutPieceType.WhiteKing && (piece.x == 0 || piece.x == layout.GetLength(0) - 1
                    || piece.y == 0 || piece.y == layout.GetLength(1) - 1))
                {
                    error = "The king must start away from the edge.";
                    return false;
                }

                layout[piece.x, piece.y] = piece.pieceType;
            }

            TablutGameMode match;
            try
            {
                match = new(layout, startingPlayer);
            }
            catch (ArgumentException exception)
            {
                error = exception.Message;
                return false;
            }

            // Use the match rules to check that the starting side can make a move.
            foreach (PiecePlacement piece in placements)
            {
                BoardPosition square = new(piece.x, piece.y);
                if (match.CanSelectPiece(square) && match.GetLegalDestinations(square).Count > 0)
                {
                    gameMode = match;
                    error = null;
                    return true;
                }
            }

            error = $"{startingPlayer} has no legal opening move.";
            return false;
        }

        [ContextMenu("Validate Scenario")]
        private void ValidateScenario()
        {
            if (TryCreateMatch(out _, out string error))
            {
                Debug.Log($"Scenario '{ScenarioName}' is ready to play.", this);
            }
            else
            {
                Debug.LogWarning(error, this);
            }
        }
    }
}
