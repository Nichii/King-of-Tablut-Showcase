using System;
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

        public TablutBoard GameBoard { get; private set; } = new TablutBoard();
        public GameState GameState { get; private set; } = GameState.Playing;
        public InvalidMoveMessage LastInvalidMoveMessage { get; private set; } = InvalidMoveMessage.None;
        public MoveMessage LastMoveMessage { get; private set; } = MoveMessage.None;

        private readonly PlayerTurnState currentPlayerTurn = new();

        public TablutGameMode(TablutPieceType[,] customBoardPositions = null)
        {
            if (customBoardPositions == null)
            {
                // Call setup class to setup standard tablut board starting positions
                StandardTablutSetup.SetupStandardPositions(GameBoard);
            }
            else
            {
                // Otherwise, setup with the provided custom board positions
                GameBoard.SetBoardPositions(customBoardPositions);
            }
        }

        public bool MovePiece(Vector2Int from, Vector2Int to)
        {
            // Reset last move messages
            LastInvalidMoveMessage = InvalidMoveMessage.None;
            LastMoveMessage = MoveMessage.None;

            if (GameState != GameState.Playing)
            {
                LastInvalidMoveMessage = InvalidMoveMessage.GameEnded;
                return false;
            }

            // Verify that both the from and to positions are valid
            if (!GameBoard.IsValidPosition(from) || !GameBoard.IsValidPosition(to))
            {
                LastInvalidMoveMessage = InvalidMoveMessage.OutOfBound;
                return false;
            }

            // Get the current piece type from the position first
            TablutPieceType[,] boardPositions = GameBoard.GetBoardPositions();
            TablutPieceType currentPieceType = boardPositions[from.x, from.y];

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
                    LastInvalidMoveMessage = InvalidMoveMessage.CannotMoveEmptySpace;
                    return false;
            }

            // Check if current player can move this piece
            if (!currentPlayerTurn.CanPlayerMove((int)owningPlayer))
            {
                LastInvalidMoveMessage = InvalidMoveMessage.WrongPlayerTurn;
                return false;
            }

            // Only allow moving orthogonally, therefore both x and y cannot be different at the same time
            if (from.x != to.x && from.y != to.y)
            {
                LastInvalidMoveMessage = InvalidMoveMessage.OnlyOrthogonalMovementAllowed;
                return false;
            }

            // Check if the destination square is occupied
            if (boardPositions[to.x, to.y] != TablutPieceType.Empty)
            {
                LastInvalidMoveMessage = InvalidMoveMessage.OccupiedSpace;
                return false;
            }

            // Check if there are any pieces in the way
            int xDir = Math.Sign(to.x - from.x);
            for (int x = from.x + xDir; x != to.x; x += xDir)
            {
                if (boardPositions[x, to.y] != TablutPieceType.Empty)
                {
                    LastInvalidMoveMessage = InvalidMoveMessage.CannotJumpOverPieces;
                    return false;
                }
                // Also disallow jumping over castle squares
                else if (x == TablutBoard.CastleSquare.x)
                {
                    LastInvalidMoveMessage = InvalidMoveMessage.CannotMoveIntoCastleSquare;
                    return false;
                }
            }
            int yDir = Math.Sign(to.y - from.y);
            for (int y = from.y + yDir; y != to.y; y += yDir)
            {
                if (boardPositions[to.x, y] != TablutPieceType.Empty)
                {
                    LastInvalidMoveMessage = InvalidMoveMessage.CannotJumpOverPieces;
                    return false;
                }
                // Also disallow jumping over castle squares
                else if (y == TablutBoard.CastleSquare.y)
                {
                    LastInvalidMoveMessage = InvalidMoveMessage.CannotMoveIntoCastleSquare;
                    return false;
                }
            }

            // Disallow moving into castle square
            if (IsTargetPositionACastleSquare(to))
            {
                LastInvalidMoveMessage = InvalidMoveMessage.CannotMoveIntoCastleSquare;
                return false;
            }

            // Now move the piece
            GameBoard.SetPiece(TablutPieceType.Empty, from);
            GameBoard.SetPiece(currentPieceType, to);

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

        private bool TryCaptureDirection(Vector2Int currentPosition, Vector2Int captureDirection)
        {
            // The position of the piece we're attempting to capture
            Vector2Int capturePos = currentPosition + captureDirection;
            // Check that ally (or empty castle) is on the other side to capture this piece
            Vector2Int allyPos = currentPosition + captureDirection * 2;

            if (GameBoard.IsValidPosition(capturePos) && GameBoard.IsValidPosition(allyPos))
            {
                if (!IsTargetSquareMyPiece(capturePos) && (IsTargetSquareMyPiece(allyPos) || IsTargetPositionACastleSquare(allyPos)))
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
                            TablutPieceType[,] boardPositions = GameBoard.GetBoardPositions();

                            // Just count how many black pawns are surrounding that square
                            int blackPawnCounts = 0;

                            for (int i = 0; i < s_surroundingPositions.Length; i++)
                            {
                                Vector2Int offsetPos = s_surroundingPositions[i] + capturePos;
                                if (boardPositions[offsetPos.x, offsetPos.y] == TablutPieceType.BlackPawn)
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
                        if (currentPlayer == (int)TablutPlayer.Attacker)
                        {
                            LastMoveMessage = MoveMessage.CapturedDefenderPawn;
                        }
                        else
                        {
                            LastMoveMessage = MoveMessage.CapturedAttackerPawn;
                        }

                        // Destroy the pawn at that position too
                        GameBoard.SetPiece(TablutPieceType.Empty, capturePos);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsTargetSquareMyPiece(Vector2Int targetPosition)
        {
            int currentPlayer = currentPlayerTurn.GetCurrentPlayerTurn();
            TablutPieceType[,] boardPositions = GameBoard.GetBoardPositions();
            TablutPieceType pieceTypeAtTarget = boardPositions[targetPosition.x, targetPosition.y];

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
            TablutPieceType[,] boardPositions = GameBoard.GetBoardPositions();
            TablutPieceType pieceTypeAtTarget = boardPositions[targetPosition.x, targetPosition.y];

            return pieceTypeAtTarget == TablutPieceType.WhiteKing;
        }
    }
}
