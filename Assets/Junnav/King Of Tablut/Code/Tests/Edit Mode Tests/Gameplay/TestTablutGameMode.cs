using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;

namespace Junnav.KingOfTablut.Tests.EditMode
{
    public class TestTablutGameMode
    {
        [Test]
        public void TablutBoardIsSetupOnStart()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Check to make sure the board has been setup in standard positions
            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new BoardPosition(4, 4)), "The White King is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(4, 2)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(4, 3)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(2, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(3, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(6, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(4, 5)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(4, 6)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 3)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 5)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(1, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(3, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(4, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(5, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(4, 1)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(7, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(8, 3)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(8, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(8, 5)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(4, 7)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(3, 8)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(4, 8)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(5, 8)), "The Black Pawn is not at the right position!");

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Added pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void ResetLastInvalidMoveMessageOnNextMove()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Defender tries to take a move (just a simple one, not testing for game rules here)
            gameMode.MovePiece(new BoardPosition(4, 2), new BoardPosition(4, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);

            // Attacker takes a move (just a simple one, not testing for game rules here)
            gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(0, 2));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
        }

        [Test]
        public void CannotMoveAPieceIntoOccupiedSpace()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move a piece onto it's own piece's location
            bool validMove = gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(0, 4));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OccupiedSpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move a piece into it's own piece's location
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 3)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 4)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotMoveAnEmptySquare()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move an empty piece to a valid location
            bool validMove = gameMode.MovePiece(new BoardPosition(0, 2), new BoardPosition(0, 1));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotMoveEmptySpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move an empty piece
            Assert.IsFalse(validMove);

            // Make sure that the squares are still empty
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(0, 2)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(0, 1)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotMovePieceToTheSameSquare()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move a piece onto it's own location
            bool validMove = gameMode.MovePiece(new BoardPosition(0, 5), new BoardPosition(0, 5));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OccupiedSpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move a piece into it's own location
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 5)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void PlayersCanOnlyMoveOnTheirTurn()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Get a reference to the board positions

            // =====================================================================================
            // TURN ONE
            // =====================================================================================
            // Defender should NOT be able to move on the first turn
            bool validMove = gameMode.MovePiece(new BoardPosition(2, 4), new BoardPosition(2, 5));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(2, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(2, 5)));

            // Attacker should be able to move on the first turn
            validMove = gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(0, 2));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 2)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(0, 3)));

            // =====================================================================================
            // TURN TWO
            // =====================================================================================
            // Attacker should NOT be able to move on the second turn
            validMove = gameMode.MovePiece(new BoardPosition(4, 7), new BoardPosition(5, 7));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(4, 7)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(5, 7)));

            // Defender should be able to move on the second turn
            validMove = gameMode.MovePiece(new BoardPosition(4, 6), new BoardPosition(3, 6));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(3, 6)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(4, 6)));

            // =====================================================================================
            // TURN THREE
            // =====================================================================================
            // Defender should NOT be able to move on the third turn
            validMove = gameMode.MovePiece(new BoardPosition(5, 4), new BoardPosition(5, 3));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(5, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(5, 3)));

            // Attacker should be able to move on the third turn
            validMove = gameMode.MovePiece(new BoardPosition(8, 5), new BoardPosition(8, 7));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(8, 7)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(8, 5)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotJumpPiecesOverOtherPieces()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker jumps a piece over a defender's piece horizontally
            bool validMove = gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(5, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotJumpOverPieces, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot jump a piece over another piece
            Assert.IsFalse(validMove);

            // Attacker jumps a piece over a defender's piece vertically
            validMove = gameMode.MovePiece(new BoardPosition(5, 8), new BoardPosition(5, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotJumpOverPieces, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot jump a piece over another piece
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(0, 3)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(5, 8)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OnlyOrthogonalMovementAllowed()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker tries to move diagonally
            bool validMove = gameMode.MovePiece(new BoardPosition(5, 8), new BoardPosition(6, 7));

            // Check that attacker cannot move diagonally
            Assert.AreEqual(InvalidMoveMessage.OnlyOrthogonalMovementAllowed, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(5, 8)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(6, 7)));

            // Attacker tries to make a random non-orthogonal movement
            validMove = gameMode.MovePiece(new BoardPosition(7, 4), new BoardPosition(2, 5));

            // Check that attacker cannot move in a random non-orthogonal movement
            Assert.AreEqual(InvalidMoveMessage.OnlyOrthogonalMovementAllowed, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(7, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(2, 5)));

            // Attacker tries to make a valid vertical movement
            validMove = gameMode.MovePiece(new BoardPosition(7, 4), new BoardPosition(7, 0));

            // Check that attacker can move
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(7, 4)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(7, 0)));

            // Defender tries to make a valid horizontal movement
            validMove = gameMode.MovePiece(new BoardPosition(4, 2), new BoardPosition(8, 2));

            // Check that defender can move
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(4, 2)));
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(8, 2)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OnlyMovementWithinBoardSquaresAllowed()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker makes invalid starting moves
            bool validMove = gameMode.MovePiece(new BoardPosition(-1, 3), new BoardPosition(1, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Attacker makes invalid destination moves
            validMove = gameMode.MovePiece(new BoardPosition(8, 3), new BoardPosition(8, -1));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Attacker makes invalid starting and destination moves
            validMove = gameMode.MovePiece(new BoardPosition(3, 9), new BoardPosition(-1, 9));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void KingCannotEnterCastleAfterLeavingIt()
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[0, 0] = TablutPieceType.BlackPawn; // Need a random piece for opponent

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Make moves until the King is out of the castle
            gameMode.MovePiece(new BoardPosition(0, 0), new BoardPosition(0, 1)); // Attacker random moves
            gameMode.MovePiece(new BoardPosition(4, 4), new BoardPosition(4, 5)); // King moves out of castle
            gameMode.MovePiece(new BoardPosition(0, 1), new BoardPosition(0, 2)); // Attacker random moves

            // Now try to move the King back into the castle
            bool isValid = gameMode.MovePiece(new BoardPosition(4, 5), new BoardPosition(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OtherPiecesCannotEnterCastle()
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[1, 1] = TablutPieceType.WhiteKing;
            customBoardPositions[4, 3] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 4] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Try to move pieces into castle
            bool isValid = gameMode.MovePiece(new BoardPosition(3, 4), new BoardPosition(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Attacker move out of the way
            gameMode.MovePiece(new BoardPosition(3, 4), new BoardPosition(2, 4));

            // Defender tries to move into castle
            isValid = gameMode.MovePiece(new BoardPosition(4, 3), new BoardPosition(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OtherPiecesCannotJumpOverCastle()
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[1, 1] = TablutPieceType.WhiteKing;
            customBoardPositions[4, 3] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 4] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Try to jump pieces over the castle
            bool isValid = gameMode.MovePiece(new BoardPosition(3, 4), new BoardPosition(5, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Attacker move out of the way
            gameMode.MovePiece(new BoardPosition(3, 4), new BoardPosition(2, 4));

            // Defender tries to move into castle
            isValid = gameMode.MovePiece(new BoardPosition(4, 3), new BoardPosition(4, 5));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, TablutPieceType,
            BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            AttackerCaptureTestCases()
        {
            // Captive's starting pos / Capturer 1's starting pos / Capturer 2's starting pos / Capturer 2's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(1, 1), new BoardPosition(2, 1), new BoardPosition(0, 0), new BoardPosition(0, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(1, 1), new BoardPosition(0, 1), new BoardPosition(2, 0), new BoardPosition(2, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(0, 1), new BoardPosition(0, 0), new BoardPosition(1, 2), new BoardPosition(0, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(8, 1), new BoardPosition(8, 2), new BoardPosition(6, 0), new BoardPosition(8, 0));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new BoardPosition(1, 1), new BoardPosition(2, 1), new BoardPosition(0, 0), new BoardPosition(0, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new BoardPosition(1, 1), new BoardPosition(0, 1), new BoardPosition(2, 0), new BoardPosition(2, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new BoardPosition(0, 1), new BoardPosition(0, 0), new BoardPosition(1, 2), new BoardPosition(0, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new BoardPosition(8, 1), new BoardPosition(8, 2), new BoardPosition(6, 0), new BoardPosition(8, 0));

            // King cannot start at or move to the edge, so let's move them inward
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new BoardPosition(2, 2), new BoardPosition(3, 2), new BoardPosition(1, 1), new BoardPosition(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new BoardPosition(2, 2), new BoardPosition(1, 2), new BoardPosition(3, 1), new BoardPosition(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new BoardPosition(1, 2), new BoardPosition(1, 1), new BoardPosition(2, 3), new BoardPosition(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new BoardPosition(7, 2), new BoardPosition(7, 3), new BoardPosition(5, 1), new BoardPosition(7, 1));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new BoardPosition(2, 2), new BoardPosition(3, 2), new BoardPosition(1, 1), new BoardPosition(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new BoardPosition(2, 2), new BoardPosition(1, 2), new BoardPosition(3, 1), new BoardPosition(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new BoardPosition(1, 2), new BoardPosition(1, 1), new BoardPosition(2, 3), new BoardPosition(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new BoardPosition(7, 2), new BoardPosition(7, 3), new BoardPosition(5, 1), new BoardPosition(7, 1));

            // King should be capturable, although, do not destroy the piece, just end the game
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(2, 2), new BoardPosition(3, 2), new BoardPosition(1, 1), new BoardPosition(1, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(2, 2), new BoardPosition(1, 2), new BoardPosition(3, 1), new BoardPosition(3, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(1, 2), new BoardPosition(1, 1), new BoardPosition(2, 3), new BoardPosition(1, 3));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new BoardPosition(7, 2), new BoardPosition(7, 3), new BoardPosition(5, 1), new BoardPosition(7, 1));
        }

        [TestCaseSource(nameof(AttackerCaptureTestCases))]
        public void CapturePawnOnOneDirection((
            TablutPieceType captivePieceType,
            TablutPieceType capturerOnePieceType,
            TablutPieceType capturerTwoPieceType,
            BoardPosition captivePos,
            BoardPosition capturerOnePos,
            BoardPosition capturerTwoPos,
            BoardPosition capturerTwoMovePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];

            // Add a King if there's none
            if (testData.captivePieceType != TablutPieceType.WhiteKing
                && testData.capturerOnePieceType != TablutPieceType.WhiteKing
                && testData.capturerTwoPieceType != TablutPieceType.WhiteKing)
            {
                customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            }
            
            // Put the pieces near the edge so we can verify possible logic error
            customBoardPositions[testData.captivePos.X, testData.captivePos.Y] = testData.captivePieceType;
            customBoardPositions[testData.capturerOnePos.X, testData.capturerOnePos.Y] = testData.capturerOnePieceType;
            customBoardPositions[testData.capturerTwoPos.X, testData.capturerTwoPos.Y] = testData.capturerTwoPieceType;
            // Lay some trap and check that these are not counted as capture
            customBoardPositions[1, 6] = TablutPieceType.BlackPawn;
            customBoardPositions[2, 6] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 6] = TablutPieceType.BlackPawn;
            // A random attacker piece for moving first move
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.capturerTwoPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new BoardPosition(4, 8), new BoardPosition(5, 8));
            }

            // Try to move in and capture opponent's pawn
            bool isValid = gameMode.MovePiece(new BoardPosition(testData.capturerTwoPos.X, testData.capturerTwoPos.Y),
                new BoardPosition(testData.capturerTwoMovePos.X, testData.capturerTwoMovePos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isValid);

            // If captive is not a King, check to make sure that the defender's pawn has been captured
            if (testData.captivePieceType != TablutPieceType.WhiteKing)
            {
                Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));

                if (testData.capturerTwoPieceType == TablutPieceType.BlackPawn)
                {
                    Assert.AreEqual(MoveMessage.CapturedDefenderPawn, gameMode.LastMoveMessage);
                }
                else
                {
                    Assert.AreEqual(MoveMessage.CapturedAttackerPawn, gameMode.LastMoveMessage);
                }
            }
            // If it is a King, do not allow it to be destroyed
            else
            {
                Assert.AreNotEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));
                Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);

                // If King has been captured, make sure that we can't continue the game
                // Assume the next player is White
                // Try to move white pawn to see if the game can be continued
                isValid = gameMode.MovePiece(new BoardPosition(2, 6), new BoardPosition(2, 5));

                // If the move is valid, then its wrong
                Assert.IsFalse(isValid);

                // Make sure that you can't move due to the game ended
                Assert.AreEqual(InvalidMoveMessage.GameEnded, gameMode.LastInvalidMoveMessage);

                // Make sure that Black has won the game
                Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            }

            // Make sure the static position is not counted as capture
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(2, 6)));
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, BoardPosition, BoardPosition, BoardPosition)>
            AttackerNoCaptureTestCases()
        {
            // Captive's colour | Captor's colour | Captive's start pos | Captor's start pos | Captor's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(1, 1), new BoardPosition(0, 0), new BoardPosition(0, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(1, 1), new BoardPosition(2, 0), new BoardPosition(2, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(0, 1), new BoardPosition(1, 2), new BoardPosition(0, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(8, 1), new BoardPosition(6, 0), new BoardPosition(8, 0));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(1, 1), new BoardPosition(0, 0), new BoardPosition(0, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(1, 1), new BoardPosition(2, 0), new BoardPosition(2, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(0, 1), new BoardPosition(1, 2), new BoardPosition(0, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(8, 1), new BoardPosition(6, 0), new BoardPosition(8, 0));

            // King cannot start at or move to the edge, so let's move them inward
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new BoardPosition(2, 2), new BoardPosition(1, 1), new BoardPosition(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new BoardPosition(2, 2), new BoardPosition(3, 1), new BoardPosition(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new BoardPosition(1, 2), new BoardPosition(2, 3), new BoardPosition(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new BoardPosition(7, 2), new BoardPosition(5, 1), new BoardPosition(7, 1));
        }

        [TestCaseSource(nameof(AttackerNoCaptureTestCases))]
        public void NoCaptureIfNotSurroundedOrthogonally((
            TablutPieceType captivePieceType,
            TablutPieceType captorPieceType,
            BoardPosition captivePos,
            BoardPosition captorOnePos,
            BoardPosition captorOneMovePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];

            // Add a King if there's none
            if (testData.captorPieceType != TablutPieceType.WhiteKing)
            {
                customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            }

            // Put the pieces near the edge so we can verify possible logic error
            customBoardPositions[testData.captivePos.X, testData.captivePos.Y] = testData.captivePieceType;
            customBoardPositions[testData.captorOnePos.X, testData.captorOnePos.Y] = testData.captorPieceType;
            // Lay some trap and check that these are not counted as capture
            customBoardPositions[1, 6] = TablutPieceType.BlackPawn;
            customBoardPositions[2, 6] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 6] = TablutPieceType.BlackPawn;
            // A random attacker piece for moving first move
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.captorPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new BoardPosition(4, 8), new BoardPosition(5, 8));
            }

            // Try to move in and capture opponent's pawn
            bool isValid = gameMode.MovePiece(new BoardPosition(testData.captorOnePos.X, testData.captorOnePos.Y),
                new BoardPosition(testData.captorOneMovePos.X, testData.captorOneMovePos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Check to make sure that the pawn has NOT been captured
            Assert.IsTrue(isValid, gameMode.LastInvalidMoveMessage.ToString());
            Assert.AreEqual(testData.captivePieceType, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);

            // Make sure the static position is not counted as capture
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new BoardPosition(2, 6)));
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, BoardPosition, BoardPosition, BoardPosition)>
            CaptureBetweenEmptyCastleTestCases()
        {
            // Captive's colour | Captor's colour | Captive's start pos | Captor's start pos | Captor's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(3, 4), new BoardPosition(2, 3), new BoardPosition(2, 4));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(4, 3), new BoardPosition(5, 2), new BoardPosition(4, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(5, 4), new BoardPosition(6, 5), new BoardPosition(6, 4));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new BoardPosition(4, 5), new BoardPosition(4, 7), new BoardPosition(4, 6));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(3, 4), new BoardPosition(2, 3), new BoardPosition(2, 4));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(4, 3), new BoardPosition(5, 2), new BoardPosition(4, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(5, 4), new BoardPosition(6, 5), new BoardPosition(6, 4));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new BoardPosition(4, 5), new BoardPosition(4, 7), new BoardPosition(4, 6));

            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new BoardPosition(3, 4), new BoardPosition(2, 3), new BoardPosition(2, 4));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new BoardPosition(4, 3), new BoardPosition(5, 2), new BoardPosition(4, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new BoardPosition(5, 4), new BoardPosition(6, 5), new BoardPosition(6, 4));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new BoardPosition(4, 5), new BoardPosition(4, 7), new BoardPosition(4, 6));
        }

        [TestCaseSource(nameof(CaptureBetweenEmptyCastleTestCases))]
        public void CapturePawnInBetweenEnemyAndEmptyCastle((
            TablutPieceType captivePieceType,
            TablutPieceType captorPieceType,
            BoardPosition captivePos,
            BoardPosition captorOnePos,
            BoardPosition captorOneMovePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            
            // If captive is not a King, then put a King somewhere off the center
            if (testData.captivePieceType != TablutPieceType.WhiteKing)
            {
                customBoardPositions[1, 1] = TablutPieceType.WhiteKing;
            }
            
            // A random attacker piece for moving first move
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;
            // Place a target pawn to be captured and an attacking pawn
            customBoardPositions[testData.captivePos.X, testData.captivePos.Y] = testData.captivePieceType;
            customBoardPositions[testData.captorOnePos.X, testData.captorOnePos.Y] = testData.captorPieceType;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.captorPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new BoardPosition(4, 8), new BoardPosition(5, 8));
            }

            // Make a move that captures enemy pawn against an empty castle
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOnePos.X, testData.captorOnePos.Y),
                new BoardPosition(testData.captorOneMovePos.X, testData.captorOneMovePos.Y));
            
            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // If captive is not a King, just check if the square is now empty
            if (testData.captivePieceType != TablutPieceType.WhiteKing)
            {
                // Check to make sure that the target pawn has been captured
                Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

                Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));
            }
            // If captive is a King, make sure the capture is invalid
            else
            {
                // Make sure that the move is still valid, even if we didn't capture the King
                Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

                Assert.AreNotEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));
                Assert.AreNotEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);

                // Make sure that the game is still in playing state
                Assert.AreEqual(GameState.Playing, gameMode.GameState);
            }
        }

        public static IEnumerable<(BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            CaptureKingNearEmptyCastleTestCases()
        {
            // King's start pos | Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos
            yield return (new BoardPosition(3, 4), new BoardPosition(2, 3), new BoardPosition(2, 4),
                new BoardPosition(3, 3), new BoardPosition(3, 5));
            yield return (new BoardPosition(4, 3), new BoardPosition(5, 2), new BoardPosition(4, 2),
                new BoardPosition(3, 3), new BoardPosition(5, 3));
            yield return (new BoardPosition(5, 4), new BoardPosition(6, 5), new BoardPosition(6, 4),
                new BoardPosition(5, 3), new BoardPosition(5, 5));
            yield return (new BoardPosition(4, 5), new BoardPosition(4, 7), new BoardPosition(4, 6),
                new BoardPosition(3, 5), new BoardPosition(5, 5));
        }

        [TestCaseSource(nameof(CaptureKingNearEmptyCastleTestCases))]
        public void CaptureKingWhenNearEmptyCastleAndSurroundedByThreeEnemies((
            BoardPosition kingStartPos,
            BoardPosition captorOneStartPos,
            BoardPosition captorOneTargetPos,
            BoardPosition captorTwoPos,
            BoardPosition captorThreePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[testData.kingStartPos.X, testData.kingStartPos.Y] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.X, testData.captorOneStartPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.X, testData.captorTwoPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.X, testData.captorThreePos.Y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOneStartPos.X, testData.captorOneStartPos.Y),
                new BoardPosition(testData.captorOneTargetPos.X, testData.captorOneTargetPos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has won
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            CaptureKingInCastleTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new BoardPosition(3, 3), new BoardPosition(4, 3),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(2, 4), new BoardPosition(3, 4),
                new BoardPosition(4, 3), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(5, 5), new BoardPosition(4, 5),
                new BoardPosition(3, 4), new BoardPosition(4, 3), new BoardPosition(5, 4));
            yield return (new BoardPosition(7, 4), new BoardPosition(5, 4),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(4, 3));
        }

        [TestCaseSource(nameof(CaptureKingInCastleTestCases))]
        public void CaptureKingInCastleWhenSurroundedByFourEnemies((
            BoardPosition captorOneStartPos,
            BoardPosition captorOneTargetPos,
            BoardPosition captorTwoPos,
            BoardPosition captorThreePos,
            BoardPosition captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.X, testData.captorOneStartPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.X, testData.captorTwoPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.X, testData.captorThreePos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.X, testData.captorFourPos.Y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOneStartPos.X, testData.captorOneStartPos.Y),
                new BoardPosition(testData.captorOneTargetPos.X, testData.captorOneTargetPos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has won
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            NoCaptureKingInCastleTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos
            yield return (new BoardPosition(3, 3), new BoardPosition(4, 3),
                new BoardPosition(3, 4), new BoardPosition(4, 5));
            yield return (new BoardPosition(2, 4), new BoardPosition(3, 4),
                new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(5, 5), new BoardPosition(4, 5),
                new BoardPosition(3, 4), new BoardPosition(5, 4));
            yield return (new BoardPosition(7, 4), new BoardPosition(5, 4),
                new BoardPosition(3, 4), new BoardPosition(4, 5));
        }

        [TestCaseSource(nameof(NoCaptureKingInCastleTestCases))]
        public void NoCaptureWhenKingIsNotSurroundedByFourEnemiesWhileInCastle((
            BoardPosition captorOneStartPos,
            BoardPosition captorOneTargetPos,
            BoardPosition captorTwoPos,
            BoardPosition captorThreePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.X, testData.captorOneStartPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.X, testData.captorTwoPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.X, testData.captorThreePos.Y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOneStartPos.X, testData.captorOneStartPos.Y),
                new BoardPosition(testData.captorOneTargetPos.X, testData.captorOneTargetPos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has not won
            Assert.AreNotEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreNotEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            CaptureDefenderWhenNearKingTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos | Captive's pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new BoardPosition(3, 2), new BoardPosition(4, 2), new BoardPosition(4, 3),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(1, 4), new BoardPosition(2, 4), new BoardPosition(3, 4),
                new BoardPosition(4, 3), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(5, 6), new BoardPosition(4, 6), new BoardPosition(4, 5),
                new BoardPosition(3, 4), new BoardPosition(4, 3), new BoardPosition(5, 4));
            yield return (new BoardPosition(7, 4), new BoardPosition(6, 4), new BoardPosition(5, 4),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(4, 3));
        }

        [TestCaseSource(nameof(CaptureDefenderWhenNearKingTestCases))]
        public void CaptureDefenderWhenNearKingInCastleSurroundedByThreeAttackers((
            BoardPosition captorOneStartPos,
            BoardPosition captorOneTargetPos,
            BoardPosition captivePos,
            BoardPosition captorTwoPos,
            BoardPosition captorThreePos,
            BoardPosition captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.X, testData.captorOneStartPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captivePos.X, testData.captivePos.Y] = TablutPieceType.WhitePawn;
            customBoardPositions[testData.captorTwoPos.X, testData.captorTwoPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.X, testData.captorThreePos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.X, testData.captorFourPos.Y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOneStartPos.X, testData.captorOneStartPos.Y),
                new BoardPosition(testData.captorOneTargetPos.X, testData.captorOneTargetPos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Check to make sure that the white pawn has been captured
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(testData.captivePos.X, testData.captivePos.Y)));
        }

        public static IEnumerable<(BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition, BoardPosition)>
            NoCaptureAttackerWhenNearKingTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos | Target captor's pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new BoardPosition(3, 2), new BoardPosition(4, 2), new BoardPosition(4, 3),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(1, 4), new BoardPosition(2, 4), new BoardPosition(3, 4),
                new BoardPosition(4, 3), new BoardPosition(4, 5), new BoardPosition(5, 4));
            yield return (new BoardPosition(5, 6), new BoardPosition(4, 6), new BoardPosition(4, 5),
                new BoardPosition(3, 4), new BoardPosition(4, 3), new BoardPosition(5, 4));
            yield return (new BoardPosition(7, 4), new BoardPosition(6, 4), new BoardPosition(5, 4),
                new BoardPosition(3, 4), new BoardPosition(4, 5), new BoardPosition(4, 3));
        }

        [TestCaseSource(nameof(NoCaptureAttackerWhenNearKingTestCases))]
        public void NoCaptureAttackerPawnWhenNearKingInCastleSurroundedByThreeAttackers((
            BoardPosition captorOneStartPos,
            BoardPosition captorOneTargetPos,
            BoardPosition targetCaptorPos,
            BoardPosition captorTwoPos,
            BoardPosition captorThreePos,
            BoardPosition captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.X, testData.captorOneStartPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.targetCaptorPos.X, testData.targetCaptorPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.X, testData.captorTwoPos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.X, testData.captorThreePos.Y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.X, testData.captorFourPos.Y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new BoardPosition(testData.captorOneStartPos.X, testData.captorOneStartPos.Y),
                new BoardPosition(testData.captorOneTargetPos.X, testData.captorOneTargetPos.Y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that nothing happens
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(testData.targetCaptorPos.X, testData.targetCaptorPos.Y)));
        }

        public static IEnumerable<(BoardPosition, BoardPosition)> KingMoveToWinTestCases()
        {
            // King's starting pos | King's target pos
            yield return (new BoardPosition(3, 4),  new BoardPosition(0, 4));
            yield return (new BoardPosition(4, 4),  new BoardPosition(4, 0));
            yield return (new BoardPosition(7, 4),  new BoardPosition(8, 4));
            yield return (new BoardPosition(7, 7),  new BoardPosition(7, 8));
        }

        [TestCaseSource(nameof(KingMoveToWinTestCases))]
        public void WhiteWinsWhenKingReachesTheEdge((
            BoardPosition kingStartingPos,
            BoardPosition kingTargetPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[testData.kingStartingPos.X, testData.kingStartingPos.Y] = TablutPieceType.WhiteKing;
            // Put random pawns
            customBoardPositions[1, 1] = TablutPieceType.BlackPawn;
            customBoardPositions[5, 5] = TablutPieceType.WhitePawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure that the game is still on-going
            Assert.AreEqual(GameState.Playing, gameMode.GameState);

            // Black moves first, just make a random move
            gameMode.MovePiece(new BoardPosition(1, 1), new BoardPosition(2, 1));

            // Now White King makes the move
            bool isMoveValid = gameMode.MovePiece(testData.kingStartingPos, testData.kingTargetPos);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            TestContext.WriteLine($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that White has won
            Assert.AreEqual(GameState.WhiteWon, gameMode.GameState);

            // Make sure that the game can't be continued by trying to move the Black Pawn
            isMoveValid = gameMode.MovePiece(new BoardPosition(2, 1), new BoardPosition(3, 1));

            // Make sure the move is invalid
            Assert.IsFalse(isMoveValid, gameMode.LastInvalidMoveMessage.ToString());
        }

        [Test]
        public void ResetMoveMessageOnNextMove()
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[5, 5] = TablutPieceType.BlackPawn;
            customBoardPositions[6, 5] = TablutPieceType.WhitePawn;
            customBoardPositions[7, 6] = TablutPieceType.BlackPawn;
            // Add a random defender piece for the next move
            customBoardPositions[1, 1] = TablutPieceType.WhitePawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Attacker capture defender's pawn
            gameMode.MovePiece(new BoardPosition(7, 6), new BoardPosition(7, 5));
            // Defender makes the next move
            gameMode.MovePiece(new BoardPosition(1, 1), new BoardPosition(2, 1));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Check that the move message has been reset
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);
        }
        [TestCase(2, 1, 6, 1)]
        [TestCase(1, 2, 1, 6)]
        public void CanCrossCastleAxisAwayFromCastle(int fromX, int fromY, int toX, int toY)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            positions[fromX, fromY] = TablutPieceType.BlackPawn;
            TablutGameMode gameMode = new(positions);

            Assert.IsTrue(gameMode.MovePiece(new BoardPosition(fromX, fromY), new BoardPosition(toX, toY)));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
        }

        [Test]
        public void EmptySquareBetweenAlliesIsNotCaptured()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            positions[1, 0] = TablutPieceType.BlackPawn;
            positions[3, 1] = TablutPieceType.BlackPawn;
            TablutGameMode gameMode = new(positions);

            Assert.IsTrue(gameMode.MovePiece(new BoardPosition(1, 0), new BoardPosition(1, 1)));
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new BoardPosition(1, 1)));
        }

        [Test]
        public void MoveResultIncludesEveryPawnCapturedByTheMove()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[7, 7] = TablutPieceType.WhiteKing;
            positions[3, 1] = TablutPieceType.BlackPawn;
            positions[1, 3] = TablutPieceType.BlackPawn;
            positions[5, 3] = TablutPieceType.BlackPawn;
            positions[2, 3] = TablutPieceType.WhitePawn;
            positions[4, 3] = TablutPieceType.WhitePawn;
            TablutGameMode gameMode = new(positions);

            MoveResult result = gameMode.MovePieceWithResult(new BoardPosition(3, 1), new BoardPosition(3, 3));

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(InvalidMoveMessage.None, result.RejectionReason);
            Assert.AreEqual(MoveMessage.CapturedDefenderPawn, result.Message);
            Assert.AreEqual(GameState.Playing, result.GameState);
            CollectionAssert.AreEquivalent(new[]
            {
                (TablutPieceType.WhitePawn, new BoardPosition(2, 3)),
                (TablutPieceType.WhitePawn, new BoardPosition(4, 3))
            }, result.Captures.Select(capture => (capture.PieceType, capture.Position)));

            foreach (CapturedPiece capture in result.Captures)
            {
                Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(capture.Position));
            }
        }

        [Test]
        public void RejectedMoveResultDoesNotReuseCapturesFromPreviousMove()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[7, 7] = TablutPieceType.WhiteKing;
            positions[1, 3] = TablutPieceType.BlackPawn;
            positions[2, 3] = TablutPieceType.WhitePawn;
            positions[3, 1] = TablutPieceType.BlackPawn;
            TablutGameMode gameMode = new(positions);

            BoardPosition king = new(7, 7);
            MoveResult capture = gameMode.MovePieceWithResult(new BoardPosition(3, 1), new BoardPosition(3, 3));
            TablutPieceType[,] boardAfterCapture = ReadBoard(gameMode);
            MoveResult rejected = gameMode.MovePieceWithResult(king, new BoardPosition(9, 7));

            Assert.IsTrue(capture.Succeeded);
            Assert.IsFalse(rejected.Succeeded);
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, rejected.RejectionReason);
            Assert.AreEqual(MoveMessage.None, rejected.Message);
            Assert.AreEqual(GameState.Playing, rejected.GameState);
            CollectionAssert.IsEmpty(rejected.Captures);
            CollectionAssert.AreEqual(boardAfterCapture, ReadBoard(gameMode));
            Assert.AreEqual(TablutPlayer.Defender, gameMode.CurrentPlayer);

            if (capture.Captures is IList<CapturedPiece> captures)
            {
                Assert.Throws<NotSupportedException>(() => captures[0] = default);
            }

            MoveResult escape = gameMode.MovePieceWithResult(king, new BoardPosition(7, 8));
            Assert.IsTrue(escape.Succeeded);
            Assert.AreEqual(GameState.WhiteWon, escape.GameState);
            CollectionAssert.IsEmpty(escape.Captures);

            Assert.AreEqual(GameState.Playing, capture.GameState);
            CollectionAssert.AreEqual(new[] { (TablutPieceType.WhitePawn, new BoardPosition(2, 3)) },
                capture.Captures.Select(piece => (piece.PieceType, piece.Position)));
        }

        [Test]
        public void KingCaptureTakesPrecedenceOverPawnCapture()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            positions[4, 1] = TablutPieceType.BlackPawn;
            positions[3, 4] = TablutPieceType.BlackPawn;
            positions[5, 4] = TablutPieceType.BlackPawn;
            positions[4, 5] = TablutPieceType.BlackPawn;
            positions[5, 3] = TablutPieceType.WhitePawn;
            positions[6, 3] = TablutPieceType.BlackPawn;
            TablutGameMode gameMode = new(positions);

            MoveResult result = gameMode.MovePieceWithResult(new BoardPosition(4, 1), new BoardPosition(4, 3));

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(GameState.BlackWon, result.GameState);
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, result.Message);
            CollectionAssert.AreEquivalent(new[]
            {
                (TablutPieceType.WhiteKing, new BoardPosition(4, 4)),
                (TablutPieceType.WhitePawn, new BoardPosition(5, 3))
            }, result.Captures.Select(capture => (capture.PieceType, capture.Position)));
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new BoardPosition(4, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(5, 3)));
        }

        [TestCase(TablutPlayer.Attacker, 1, 1, 3, 3, TablutPlayer.Defender)]
        [TestCase(TablutPlayer.Defender, 3, 3, 1, 1, TablutPlayer.Attacker)]
        public void CustomMatchUsesConfiguredStartingPlayer(TablutPlayer startingPlayer, int firstX, int firstY,
            int otherX, int otherY, TablutPlayer nextPlayer)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[1, 1] = TablutPieceType.BlackPawn;
            positions[3, 3] = TablutPieceType.WhiteKing;
            TablutGameMode gameMode = new(positions, startingPlayer);
            BoardPosition firstPiece = new(firstX, firstY);
            BoardPosition otherPiece = new(otherX, otherY);
            BoardPosition destination = new(firstX + 1, firstY);

            Assert.AreEqual(startingPlayer, gameMode.CurrentPlayer);
            Assert.IsTrue(gameMode.CanSelectPiece(firstPiece));
            Assert.IsFalse(gameMode.CanSelectPiece(otherPiece));
            CollectionAssert.Contains(gameMode.GetLegalDestinations(firstPiece), destination);
            CollectionAssert.IsEmpty(gameMode.GetLegalDestinations(otherPiece));

            MoveResult rejected = gameMode.MovePieceWithResult(otherPiece, new BoardPosition(otherX + 1, otherY));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, rejected.RejectionReason);
            Assert.AreEqual(startingPlayer, gameMode.CurrentPlayer);

            Assert.IsTrue(gameMode.MovePiece(firstPiece, destination));
            Assert.AreEqual(nextPlayer, gameMode.CurrentPlayer);
        }

        [TestCase((TablutPlayer)(-1))]
        [TestCase((TablutPlayer)2)]
        public void RejectsUnknownStartingPlayer(TablutPlayer startingPlayer)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new TablutGameMode(positions, startingPlayer));
            Assert.AreEqual("startingPlayer", exception.ParamName);
        }

        [Test]
        public void StandardMatchRequiresAttackersToStart()
        {
            Assert.AreEqual(TablutPlayer.Attacker, new TablutGameMode().CurrentPlayer);
            Assert.AreEqual(TablutPlayer.Attacker, new TablutGameMode(null, TablutPlayer.Attacker).CurrentPlayer);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new TablutGameMode(null, TablutPlayer.Defender));
            Assert.AreEqual("startingPlayer", exception.ParamName);
        }

        [Test]
        public void RejectsCustomBoardWithInvalidDimensions()
        {
            TablutPieceType[,] positions = new TablutPieceType[8, 8];

            Assert.Throws<ArgumentException>(() => new TablutGameMode(positions));
            Assert.AreEqual(TablutPieceType.WhiteKing, new TablutGameMode().GetPiece(new BoardPosition(4, 4)));
        }

        [TestCase(0)]
        [TestCase(2)]
        public void RejectsCustomBoardWithWrongNumberOfKings(int kingCount)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            for (int i = 0; i < kingCount; i++)
            {
                positions[4 + i, 4] = TablutPieceType.WhiteKing;
            }

            Assert.Throws<ArgumentException>(() => new TablutGameMode(positions));
        }

        [Test]
        public void RejectsCustomBoardWithUnknownPieceType()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            positions[1, 1] = (TablutPieceType)99;

            Assert.Throws<ArgumentException>(() => new TablutGameMode(positions));
        }

        [Test]
        public void ChangesToSetupDoNotAffectMatch()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            TablutGameMode gameMode = new(positions);

            positions[4, 4] = TablutPieceType.Empty;

            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new BoardPosition(4, 4)));
        }

        [Test]
        public void LegalDestinationsDoNotChangeMatchState()
        {
            TablutGameMode gameMode = new();
            BoardPosition from = new(0, 3);

            Assert.IsFalse(gameMode.MovePiece(from, new BoardPosition(-1, 3)));
            CollectionAssert.AreEquivalent(new[]
            {
                new BoardPosition(0, 0), new BoardPosition(0, 1), new BoardPosition(0, 2),
                new BoardPosition(1, 3), new BoardPosition(2, 3), new BoardPosition(3, 3)
            }, gameMode.GetLegalDestinations(from));
            Assert.IsTrue(gameMode.CanSelectPiece(from));
            Assert.IsFalse(gameMode.CanSelectPiece(new BoardPosition(4, 4)));
            CollectionAssert.IsEmpty(gameMode.GetLegalDestinations(new BoardPosition(4, 4)));
            CollectionAssert.IsEmpty(gameMode.GetLegalDestinations(new BoardPosition(-1, 3)));

            Assert.AreEqual(TablutPlayer.Attacker, gameMode.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(from));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new BoardPosition(1, 3)));
            Assert.AreEqual(GameState.Playing, gameMode.GameState);
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);
        }

        [TestCase(2, 1)]
        [TestCase(6, -1)]
        public void LegalDestinationsStopAtPiecesAndCastleOnEitherSideOfBoard(int column, int towardsCastle)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[column, 4] = TablutPieceType.BlackPawn;
            positions[column, 6] = TablutPieceType.WhitePawn;
            positions[4, 2] = TablutPieceType.WhiteKing;

            AssertLegalDestinationsMatchMoves(positions, new BoardPosition(column, 4),
                new BoardPosition(column + towardsCastle, 4),
                new BoardPosition(column - towardsCastle, 4),
                new BoardPosition(column - 2 * towardsCastle, 4),
                new BoardPosition(column, 5),
                new BoardPosition(column, 3),
                new BoardPosition(column, 2),
                new BoardPosition(column, 1),
                new BoardPosition(column, 0));
        }

        [TestCase(TablutPlayer.Attacker)]
        [TestCase(TablutPlayer.Defender)]
        public void LegalDestinationsMatchMoveExecutionForEverySquare(TablutPlayer startingPlayer)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 6] = TablutPieceType.WhiteKing;
            positions[2, 2] = TablutPieceType.WhitePawn;
            positions[6, 4] = TablutPieceType.WhitePawn;
            positions[4, 2] = TablutPieceType.BlackPawn;
            positions[0, 0] = TablutPieceType.BlackPawn;
            positions[4, 8] = TablutPieceType.BlackPawn;
            TablutGameMode gameMode = new(positions, startingPlayer);

            for (int fromX = 0; fromX < positions.GetLength(0); fromX++)
            {
                for (int fromY = 0; fromY < positions.GetLength(1); fromY++)
                {
                    BoardPosition from = new(fromX, fromY);
                    List<BoardPosition> accepted = new();
                    for (int toX = 0; toX < positions.GetLength(0); toX++)
                    {
                        for (int toY = 0; toY < positions.GetLength(1); toY++)
                        {
                            BoardPosition to = new(toX, toY);
                            // Judge each move from the same untouched starting position.
                            TablutGameMode attempt = new(positions, startingPlayer);
                            if (attempt.MovePiece(from, to))
                            {
                                accepted.Add(to);
                            }
                        }
                    }

                    CollectionAssert.AreEqual(accepted, gameMode.GetLegalDestinations(from),
                        $"{startingPlayer} destinations from {from} differ from move execution.");
                }
            }
        }

        [Test]
        public void LegalDestinationsAtCornerStayInsideTheBoard()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[0, 0] = TablutPieceType.BlackPawn;
            positions[2, 0] = TablutPieceType.WhitePawn;
            positions[0, 2] = TablutPieceType.WhitePawn;
            positions[4, 4] = TablutPieceType.WhiteKing;

            AssertLegalDestinationsMatchMoves(positions, new BoardPosition(0, 0),
                new BoardPosition(1, 0), new BoardPosition(0, 1));
        }

        [TestCase(4, 2, -1, 2, InvalidMoveMessage.OutOfBound)]
        [TestCase(0, 2, 0, 1, InvalidMoveMessage.WrongPlayerTurn)]
        [TestCase(4, 2, 5, 3, InvalidMoveMessage.OnlyOrthogonalMovementAllowed)]
        [TestCase(4, 2, 2, 2, InvalidMoveMessage.OccupiedSpace)]
        [TestCase(4, 2, 1, 2, InvalidMoveMessage.CannotJumpOverPieces)]
        [TestCase(4, 2, 4, 4, InvalidMoveMessage.CannotMoveIntoCastleSquare)]
        [TestCase(8, 8, 8, 7, InvalidMoveMessage.CannotMoveEmptySpace)]
        public void RejectedMovesDoNotChangeMatchState(int fromX, int fromY, int toX, int toY, InvalidMoveMessage reason)
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[0, 3] = TablutPieceType.BlackPawn;
            positions[2, 2] = TablutPieceType.WhitePawn;
            positions[4, 2] = TablutPieceType.WhiteKing;
            TablutGameMode gameMode = new(positions);

            Assert.IsTrue(gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(0, 2)));

            AssertRejectedMoveLeavesMatchUnchanged(gameMode, new BoardPosition(fromX, fromY), new BoardPosition(toX, toY), reason);
        }

        [Test]
        public void RejectedMovesDoNotChangeFinishedMatch()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[0, 3] = TablutPieceType.BlackPawn;
            positions[1, 1] = TablutPieceType.WhiteKing;
            TablutGameMode gameMode = new(positions);

            Assert.IsTrue(gameMode.MovePiece(new BoardPosition(0, 3), new BoardPosition(0, 2)));
            Assert.IsTrue(gameMode.MovePiece(new BoardPosition(1, 1), new BoardPosition(1, 0)));
            Assert.AreEqual(GameState.WhiteWon, gameMode.GameState);

            AssertRejectedMoveLeavesMatchUnchanged(gameMode, new BoardPosition(0, 2), new BoardPosition(0, 1), InvalidMoveMessage.GameEnded);
            CollectionAssert.IsEmpty(gameMode.GetLegalDestinations(new BoardPosition(0, 2)));
        }

        [Test]
        public void OpeningSequenceEndsWithTheKingEscaping()
        {
            TablutGameMode gameMode = new();
            (BoardPosition from, BoardPosition to)[] moves =
            {
                (new BoardPosition(0, 3), new BoardPosition(0, 2)),
                (new BoardPosition(4, 3), new BoardPosition(6, 3)),
                (new BoardPosition(3, 0), new BoardPosition(3, 3)),
                (new BoardPosition(4, 2), new BoardPosition(3, 2)),
                (new BoardPosition(8, 3), new BoardPosition(8, 2)),
                (new BoardPosition(4, 4), new BoardPosition(4, 2)),
                (new BoardPosition(8, 2), new BoardPosition(8, 3)),
                (new BoardPosition(4, 2), new BoardPosition(8, 2))
            };

            for (int i = 0; i < moves.Length; i++)
            {
                (BoardPosition from, BoardPosition to) = moves[i];
                Assert.IsTrue(gameMode.MovePiece(from, to),
                    $"Move {i + 1} from {from} to {to} failed with {gameMode.LastInvalidMoveMessage}.");
            }

            Assert.AreEqual(GameState.WhiteWon, gameMode.GameState);
            Assert.AreEqual(TablutPlayer.Attacker, gameMode.CurrentPlayer);

            // Rows start at y = 0.
            string[] expectedRows =
            {
                "□□□□♟♟□□□",
                "□□□□♟□□□□",
                "♟□□♙□□□□♔",
                "□□□□□□♙□♟",
                "♟♟♙♙□♙♙♟♟",
                "♟□□□♙□□□♟",
                "□□□□♙□□□□",
                "□□□□♟□□□□",
                "□□□♟♟♟□□□"
            };

            for (int y = 0; y < expectedRows.Length; y++)
            {
                for (int x = 0; x < expectedRows[y].Length; x++)
                {
                    BoardPosition square = new(x, y);
                    TablutPieceType expected = expectedRows[y][x] switch
                    {
                        '♟' => TablutPieceType.BlackPawn,
                        '♙' => TablutPieceType.WhitePawn,
                        '♔' => TablutPieceType.WhiteKing,
                        '□' => TablutPieceType.Empty,
                        _ => throw new ArgumentException($"Unknown piece at {square}.")
                    };

                    Assert.AreEqual(expected, gameMode.GetPiece(square), $"Unexpected piece at {square}.");
                }
            }
        }

        private static void AssertLegalDestinationsMatchMoves(TablutPieceType[,] positions, BoardPosition from,
            params BoardPosition[] expectedDestinations)
        {
            TablutGameMode gameMode = new(positions);
            IReadOnlyList<BoardPosition> destinations = gameMode.GetLegalDestinations(from);
            CollectionAssert.AreEquivalent(expectedDestinations, destinations);

            // Each attempt starts from the same board and turn.
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    BoardPosition to = new(x, y);
                    TablutGameMode attempt = new(positions);

                    Assert.AreEqual(Array.IndexOf(expectedDestinations, to) >= 0, attempt.MovePiece(from, to),
                        $"Move from {from} to {to} disagrees with the legal destinations.");
                }
            }
        }

        private static void AssertRejectedMoveLeavesMatchUnchanged(TablutGameMode gameMode, BoardPosition from, BoardPosition to,
            InvalidMoveMessage reason)
        {
            TablutPieceType[,] boardBeforeMove = ReadBoard(gameMode);
            TablutPlayer playerBeforeMove = gameMode.CurrentPlayer;
            GameState stateBeforeMove = gameMode.GameState;

            Assert.IsFalse(gameMode.MovePiece(from, to));
            Assert.AreEqual(reason, gameMode.LastInvalidMoveMessage);
            CollectionAssert.AreEqual(boardBeforeMove, ReadBoard(gameMode));
            Assert.AreEqual(playerBeforeMove, gameMode.CurrentPlayer);
            Assert.AreEqual(stateBeforeMove, gameMode.GameState);
        }

        private static TablutPieceType[,] ReadBoard(TablutGameMode gameMode)
        {
            TablutPieceType[,] positions = new TablutPieceType[TablutBoard.TotalHorizontalSquares, TablutBoard.TotalVerticalSquares];
            for (int x = 0; x < positions.GetLength(0); x++)
            {
                for (int y = 0; y < positions.GetLength(1); y++)
                {
                    positions[x, y] = gameMode.GetPiece(new BoardPosition(x, y));
                }
            }

            return positions;
        }

    }
}
