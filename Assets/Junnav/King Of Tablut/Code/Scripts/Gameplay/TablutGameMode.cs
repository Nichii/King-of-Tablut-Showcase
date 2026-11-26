using System;
using System.Collections.Generic;
using UnityEngine;

namespace Junnav.KingOfTablut.Gameplay
{
    public class TablutGameMode
    {
        private const int RegularCaptorCount = 2;
        private const int KingNearCastleCaptorCount = 3;
        private const int KingInsideCastleCaptorCount = 4;

        private static readonly Vector2Int[] s_surroundingPositions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        private readonly TablutBoard gameBoard = new();
        public GameState GameState { get; private set; } = GameState.Playing;
        public InvalidMoveMessage LastInvalidMoveMessage { get; private set; } = InvalidMoveMessage.None;
        public MoveMessage LastMoveMessage { get; private set; } = MoveMessage.None;

        private readonly PlayerTurnState currentPlayerTurn = new();

        /// <summary>
        /// Starts a match with the standard layout or a copy of the supplied positions.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The custom layout has the wrong dimensions, an unknown piece type, or a king count other than one.
        /// </exception>
        public TablutGameMode(TablutPieceType[,] customBoardPositions = null)
        {
            if (customBoardPositions == null)
            {
                StandardTablutSetup.SetupStandardPositions(gameBoard);
                return;
            }

            if (!gameBoard.SetBoardPositions(customBoardPositions))
            {
                throw new ArgumentException("Custom positions must match the board dimensions.", nameof(customBoardPositions));
            }

            // Validate the match's copy so the checked layout is the one used for play.
            int kingCount = 0;
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    TablutPieceType pieceType = gameBoard.GetPiece(new Vector2Int(x, y));
                    switch (pieceType)
                    {
                        case TablutPieceType.WhiteKing:
                            kingCount++;
                            break;
                        case TablutPieceType.Empty:
                        case TablutPieceType.WhitePawn:
                        case TablutPieceType.BlackPawn:
                            break;
                        default:
                            throw new ArgumentException("Custom positions contain an unknown piece type.", nameof(customBoardPositions));
                    }
                }
            }

            if (kingCount != 1)
            {
                throw new ArgumentException("Custom positions must contain exactly one king.", nameof(customBoardPositions));
            }
        }

        public TablutPieceType GetPiece(Vector2Int position)
        {
            return gameBoard.GetPiece(position);
        }

        public override string ToString()
        {
            return gameBoard.ToString();
        }

        public TablutPlayer CurrentPlayer => (TablutPlayer)currentPlayerTurn.GetCurrentPlayerTurn();

        public bool CanSelectPiece(Vector2Int position)
        {
            return GameState == GameState.Playing && gameBoard.IsValidPosition(position) && IsTargetSquareMyPiece(position);
        }

        /// <summary>
        /// Lists the squares the current player can move to without changing the match.
        /// </summary>
        public IReadOnlyList<Vector2Int> GetLegalDestinations(Vector2Int from)
        {
            List<Vector2Int> destinations = new();
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    Vector2Int to = new(x, y);
                    if (ValidateMove(from, to) == InvalidMoveMessage.None)
                    {
                        destinations.Add(to);
                    }
                }
            }

            return destinations;
        }

        public bool MovePiece(Vector2Int from, Vector2Int to)
        {
            // Reset last move messages
            LastInvalidMoveMessage = InvalidMoveMessage.None;
            LastMoveMessage = MoveMessage.None;

            LastInvalidMoveMessage = ValidateMove(from, to);
            if (LastInvalidMoveMessage != InvalidMoveMessage.None)
            {
                return false;
            }

            TablutPieceType currentPieceType = gameBoard.GetPiece(from);

            // Now move the piece
            gameBoard.SetPiece(TablutPieceType.Empty, from);
            gameBoard.SetPiece(currentPieceType, to);

            // Check for captures for the piece last moved
            TryCaptureDirection(to, Vector2Int.up);
            TryCaptureDirection(to, Vector2Int.down);
            TryCaptureDirection(to, Vector2Int.left);
            TryCaptureDirection(to, Vector2Int.right);

            // If White King, check if it has reached the winning squares (edge of the board)
            if (currentPieceType == TablutPieceType.WhiteKing)
            {
                if (to.x == 0
                    || to.y == 0
                    || to.x == TablutBoard.TotalHorizontalSquares - 1
                    || to.y == TablutBoard.TotalVerticalSquares - 1)
                {
                    GameState = GameState.WhiteWon;
                }
            }

            // End current player's turn
            currentPlayerTurn.GoToNextPlayer();

            return true;
        }

        private InvalidMoveMessage ValidateMove(Vector2Int from, Vector2Int to)
        {
            if (GameState != GameState.Playing)
            {
                return InvalidMoveMessage.GameEnded;
            }

            // Verify that both the from and to positions are valid
            if (!gameBoard.IsValidPosition(from) || !gameBoard.IsValidPosition(to))
            {
                return InvalidMoveMessage.OutOfBound;
            }

            // Get the current piece type from the position first
            TablutPieceType currentPieceType = gameBoard.GetPiece(from);

            // Check whose player this piece belongs to
            TablutPlayer owningPlayer = TablutPlayer.Attacker;
            switch (currentPieceType)
            {
                case TablutPieceType.WhitePawn:
                case TablutPieceType.WhiteKing:
                    owningPlayer = TablutPlayer.Defender;
                    break;

                case TablutPieceType.Empty:
                    // Do not allow moving empty squares
                    return InvalidMoveMessage.CannotMoveEmptySpace;
            }

            // Check if current player can move this piece
            if (!currentPlayerTurn.CanPlayerMove((int)owningPlayer))
            {
                return InvalidMoveMessage.WrongPlayerTurn;
            }

            // Only allow moving orthogonally, therefore both x and y cannot be different at the same time
            if (from.x != to.x && from.y != to.y)
            {
                return InvalidMoveMessage.OnlyOrthogonalMovementAllowed;
            }

            // Check if the destination square is occupied
            if (gameBoard.GetPiece(to) != TablutPieceType.Empty)
            {
                return InvalidMoveMessage.OccupiedSpace;
            }

            // Check if there are any pieces in the way
            int xDir = Math.Sign(to.x - from.x);
            for (int x = from.x + xDir; x != to.x; x += xDir)
            {
                if (gameBoard.GetPiece(new Vector2Int(x, to.y)) != TablutPieceType.Empty)
                {
                    return InvalidMoveMessage.CannotJumpOverPieces;
                }
                // Also disallow jumping over castle squares
                else if (IsTargetPositionACastleSquare(new Vector2Int(x, to.y)))
                {
                    return InvalidMoveMessage.CannotMoveIntoCastleSquare;
                }
            }
            int yDir = Math.Sign(to.y - from.y);
            for (int y = from.y + yDir; y != to.y; y += yDir)
            {
                if (gameBoard.GetPiece(new Vector2Int(to.x, y)) != TablutPieceType.Empty)
                {
                    return InvalidMoveMessage.CannotJumpOverPieces;
                }
                // Also disallow jumping over castle squares
                else if (IsTargetPositionACastleSquare(new Vector2Int(to.x, y)))
                {
                    return InvalidMoveMessage.CannotMoveIntoCastleSquare;
                }
            }

            // Disallow moving into castle square
            if (IsTargetPositionACastleSquare(to))
            {
                return InvalidMoveMessage.CannotMoveIntoCastleSquare;
            }

            return InvalidMoveMessage.None;
        }

        private bool TryCaptureDirection(Vector2Int currentPosition, Vector2Int captureDirection)
        {
            // The position of the piece we're attempting to capture
            Vector2Int capturePos = currentPosition + captureDirection;
            // Check that ally (or empty castle) is on the other side to capture this piece
            Vector2Int allyPos = currentPosition + captureDirection * 2;

            if (gameBoard.IsValidPosition(capturePos) && gameBoard.IsValidPosition(allyPos))
            {
                if (gameBoard.GetPiece(capturePos) != TablutPieceType.Empty
                    && !IsTargetSquareMyPiece(capturePos)
                    && (IsTargetSquareMyPiece(allyPos) || IsTargetPositionACastleSquare(allyPos)))
                {
                    int currentPlayer = currentPlayerTurn.GetCurrentPlayerTurn();

                    // Check if White King has been captured
                    if (IsTargetSquareAWhiteKing(capturePos))
                    {
                        bool isWhiteKingCaptured = false;

                        // If White King is near the castle, then you'll need to surround it with three black pawns to capture it
                        // If White King is inside the castle, then you'll need to surround it with four black pawns to capture it
                        // Otherwise, White King can be captured like any other pawns.
                        int blackPawnsRequiredForCapture = RegularCaptorCount; // Only two captors are required

                        if (IsTargetSquareNearACastle(capturePos))
                        {
                            blackPawnsRequiredForCapture = KingNearCastleCaptorCount; // Three captors are required
                        }
                        else if (IsTargetPositionACastleSquare(capturePos))
                        {
                            blackPawnsRequiredForCapture = KingInsideCastleCaptorCount; // Four captors are required
                        }

                        // If King is near or inside a castle, count how many captors are surrounding the King
                        if (blackPawnsRequiredForCapture > RegularCaptorCount)
                        {

                            // Just count how many black pawns are surrounding that square
                            int blackPawnCounts = 0;

                            for (int i = 0; i < s_surroundingPositions.Length; i++)
                            {
                                Vector2Int offsetPos = s_surroundingPositions[i] + capturePos;
                                if (gameBoard.GetPiece(offsetPos) == TablutPieceType.BlackPawn)
                                {
                                    blackPawnCounts++;
                                }
                            }

                            // If there are exactly the right amount of captors, then the King is now captured
                            // It is impossible for there to be more than three captors if King is near a castle
                            if (blackPawnCounts == blackPawnsRequiredForCapture)
                            {
                                isWhiteKingCaptured = true;
                            }
                        }
                        // Else, White King is captured like any other pawns
                        else
                        {
                            isWhiteKingCaptured = true;
                        }

                        if (isWhiteKingCaptured)
                        {
                            LastMoveMessage = MoveMessage.CapturedDefenderKing;
                            GameState = GameState.BlackWon;

                            return true;
                        }
                    }
                    // Otherwise, its the pawn that has been captured
                    else
                    {
                        // A pawn captured on the same move must not replace the winning capture.
                        if (LastMoveMessage != MoveMessage.CapturedDefenderKing)
                        {
                            LastMoveMessage = currentPlayer == (int)TablutPlayer.Attacker
                                ? MoveMessage.CapturedDefenderPawn
                                : MoveMessage.CapturedAttackerPawn;
                        }

                        // Destroy the pawn at that position too
                        gameBoard.SetPiece(TablutPieceType.Empty, capturePos);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsTargetSquareMyPiece(Vector2Int targetPosition)
        {
            int currentPlayer = currentPlayerTurn.GetCurrentPlayerTurn();
            TablutPieceType pieceTypeAtTarget = gameBoard.GetPiece(targetPosition);

            if (currentPlayer == (int)TablutPlayer.Attacker)
            {
                return pieceTypeAtTarget == TablutPieceType.BlackPawn;
            }
            else if (currentPlayer == (int)TablutPlayer.Defender)
            {
                return pieceTypeAtTarget == TablutPieceType.WhitePawn || pieceTypeAtTarget == TablutPieceType.WhiteKing;
            }

            return false;
        }

        private bool IsTargetPositionACastleSquare(Vector2Int targetPosition)
        {
            return targetPosition == TablutBoard.CastleSquare;
        }

        private bool IsTargetSquareNearACastle(Vector2Int targetPosition)
        {
            return ((targetPosition.x == TablutBoard.CastleSquare.x && Mathf.Abs(targetPosition.y - TablutBoard.CastleSquare.y) == 1)
                || (targetPosition.y == TablutBoard.CastleSquare.y && Mathf.Abs(targetPosition.x - TablutBoard.CastleSquare.x) == 1));
        }

        private bool IsTargetSquareAWhiteKing(Vector2Int targetPosition)
        {
            TablutPieceType pieceTypeAtTarget = gameBoard.GetPiece(targetPosition);

            return pieceTypeAtTarget == TablutPieceType.WhiteKing;
        }
    }
}
