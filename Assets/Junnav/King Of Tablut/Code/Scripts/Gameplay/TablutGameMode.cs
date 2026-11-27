using System;
using System.Collections.Generic;

namespace Junnav.KingOfTablut.Gameplay
{
    public class TablutGameMode
    {
        private const int RegularCaptorCount = 2;
        private const int KingNearCastleCaptorCount = 3;
        private const int KingInsideCastleCaptorCount = 4;

        private static readonly (int x, int y)[] s_surroundingDirections =
        {
            (0, 1), (0, -1), (-1, 0), (1, 0)
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
                    TablutPieceType pieceType = gameBoard.GetPiece(new BoardPosition(x, y));
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

        public TablutPieceType GetPiece(BoardPosition position)
        {
            return gameBoard.GetPiece(position);
        }

        public override string ToString()
        {
            return gameBoard.ToString();
        }

        public TablutPlayer CurrentPlayer => currentPlayerTurn.GetCurrentPlayerTurn();

        public bool CanSelectPiece(BoardPosition position)
        {
            return GameState == GameState.Playing && gameBoard.IsValidPosition(position) && IsTargetSquareMyPiece(position);
        }

        /// <summary>
        /// Lists the squares the current player can move to without changing the match.
        /// </summary>
        public IReadOnlyList<BoardPosition> GetLegalDestinations(BoardPosition from)
        {
            List<BoardPosition> destinations = new();
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    BoardPosition to = new(x, y);
                    if (ValidateMove(from, to) == InvalidMoveMessage.None)
                    {
                        destinations.Add(to);
                    }
                }
            }

            return destinations;
        }

        public bool MovePiece(BoardPosition from, BoardPosition to)
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
            foreach ((int x, int y) direction in s_surroundingDirections)
            {
                TryCaptureDirection(to, direction);
            }

            // If White King, check if it has reached the winning squares (edge of the board)
            if (currentPieceType == TablutPieceType.WhiteKing)
            {
                if (to.X == 0
                    || to.Y == 0
                    || to.X == TablutBoard.TotalHorizontalSquares - 1
                    || to.Y == TablutBoard.TotalVerticalSquares - 1)
                {
                    GameState = GameState.WhiteWon;
                }
            }

            // End current player's turn
            currentPlayerTurn.GoToNextPlayer();

            return true;
        }

        private InvalidMoveMessage ValidateMove(BoardPosition from, BoardPosition to)
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
            if (!currentPlayerTurn.CanPlayerMove(owningPlayer))
            {
                return InvalidMoveMessage.WrongPlayerTurn;
            }

            // Only allow moving orthogonally, therefore both x and y cannot be different at the same time
            if (from.X != to.X && from.Y != to.Y)
            {
                return InvalidMoveMessage.OnlyOrthogonalMovementAllowed;
            }

            // Check if the destination square is occupied
            if (gameBoard.GetPiece(to) != TablutPieceType.Empty)
            {
                return InvalidMoveMessage.OccupiedSpace;
            }

            // Check if there are any pieces in the way
            int xDir = Math.Sign(to.X - from.X);
            for (int x = from.X + xDir; x != to.X; x += xDir)
            {
                if (gameBoard.GetPiece(new BoardPosition(x, to.Y)) != TablutPieceType.Empty)
                {
                    return InvalidMoveMessage.CannotJumpOverPieces;
                }
                // Also disallow jumping over castle squares
                else if (IsTargetPositionACastleSquare(new BoardPosition(x, to.Y)))
                {
                    return InvalidMoveMessage.CannotMoveIntoCastleSquare;
                }
            }
            int yDir = Math.Sign(to.Y - from.Y);
            for (int y = from.Y + yDir; y != to.Y; y += yDir)
            {
                if (gameBoard.GetPiece(new BoardPosition(to.X, y)) != TablutPieceType.Empty)
                {
                    return InvalidMoveMessage.CannotJumpOverPieces;
                }
                // Also disallow jumping over castle squares
                else if (IsTargetPositionACastleSquare(new BoardPosition(to.X, y)))
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

        private bool TryCaptureDirection(BoardPosition currentPosition, (int x, int y) captureDirection)
        {
            // The position of the piece we're attempting to capture
            BoardPosition capturePos = currentPosition.Offset(captureDirection.x, captureDirection.y);
            // Check that ally (or empty castle) is on the other side to capture this piece
            BoardPosition allyPos = currentPosition.Offset(captureDirection.x * 2, captureDirection.y * 2);

            if (gameBoard.IsValidPosition(capturePos) && gameBoard.IsValidPosition(allyPos))
            {
                if (gameBoard.GetPiece(capturePos) != TablutPieceType.Empty
                    && !IsTargetSquareMyPiece(capturePos)
                    && (IsTargetSquareMyPiece(allyPos) || IsTargetPositionACastleSquare(allyPos)))
                {
                    TablutPlayer currentPlayer = CurrentPlayer;

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

                            foreach ((int x, int y) direction in s_surroundingDirections)
                            {
                                BoardPosition offsetPos = capturePos.Offset(direction.x, direction.y);
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
                            LastMoveMessage = currentPlayer == TablutPlayer.Attacker
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

        private bool IsTargetSquareMyPiece(BoardPosition targetPosition)
        {
            TablutPlayer currentPlayer = CurrentPlayer;
            TablutPieceType pieceTypeAtTarget = gameBoard.GetPiece(targetPosition);

            if (currentPlayer == TablutPlayer.Attacker)
            {
                return pieceTypeAtTarget == TablutPieceType.BlackPawn;
            }
            else if (currentPlayer == TablutPlayer.Defender)
            {
                return pieceTypeAtTarget == TablutPieceType.WhitePawn || pieceTypeAtTarget == TablutPieceType.WhiteKing;
            }

            return false;
        }

        private bool IsTargetPositionACastleSquare(BoardPosition targetPosition)
        {
            return targetPosition == TablutBoard.CastleSquare;
        }

        private bool IsTargetSquareNearACastle(BoardPosition targetPosition)
        {
            return ((targetPosition.X == TablutBoard.CastleSquare.X && Math.Abs(targetPosition.Y - TablutBoard.CastleSquare.Y) == 1)
                || (targetPosition.Y == TablutBoard.CastleSquare.Y && Math.Abs(targetPosition.X - TablutBoard.CastleSquare.X) == 1));
        }

        private bool IsTargetSquareAWhiteKing(BoardPosition targetPosition)
        {
            TablutPieceType pieceTypeAtTarget = gameBoard.GetPiece(targetPosition);

            return pieceTypeAtTarget == TablutPieceType.WhiteKing;
        }
    }
}
