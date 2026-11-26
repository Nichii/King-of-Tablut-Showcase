using System;
using System.Collections;
using System.Collections.Generic;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Junnav.KingOfTablut.Tests.PlayMode
{
    public class TestTablutGameMode
    {
        [Test]
        public void TablutBoardIsSetupOnStart()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Check to make sure the board has been setup in standard positions
            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new Vector2Int(4, 4)), "The White King is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(4, 2)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(4, 3)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(2, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(3, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(6, 4)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(4, 5)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(4, 6)), "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 3)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 5)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(1, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(3, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(4, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(5, 0)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(4, 1)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(7, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(8, 3)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(8, 4)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(8, 5)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(4, 7)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(3, 8)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(4, 8)), "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(5, 8)), "The Black Pawn is not at the right position!");

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Added pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void ResetLastInvalidMoveMessageOnNextMove()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Defender tries to take a move (just a simple one, not testing for game rules here)
            gameMode.MovePiece(new Vector2Int(4, 2), new Vector2Int(4, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);

            // Attacker takes a move (just a simple one, not testing for game rules here)
            gameMode.MovePiece(new Vector2Int(0, 3), new Vector2Int(0, 2));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
        }

        [Test]
        public void CannotMoveAPieceIntoOccupiedSpace()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move a piece onto it's own piece's location
            bool validMove = gameMode.MovePiece(new Vector2Int(0, 3), new Vector2Int(0, 4));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OccupiedSpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move a piece into it's own piece's location
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 3)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 4)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotMoveAnEmptySquare()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move an empty piece to a valid location
            bool validMove = gameMode.MovePiece(new Vector2Int(0, 2), new Vector2Int(0, 1));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotMoveEmptySpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move an empty piece
            Assert.IsFalse(validMove);

            // Make sure that the squares are still empty
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(0, 2)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(0, 1)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotMovePieceToTheSameSquare()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker move a piece onto it's own location
            bool validMove = gameMode.MovePiece(new Vector2Int(0, 5), new Vector2Int(0, 5));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OccupiedSpace, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot move a piece into it's own location
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 5)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
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
            bool validMove = gameMode.MovePiece(new Vector2Int(2, 4), new Vector2Int(2, 5));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(2, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(2, 5)));

            // Attacker should be able to move on the first turn
            validMove = gameMode.MovePiece(new Vector2Int(0, 3), new Vector2Int(0, 2));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 2)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(0, 3)));

            // =====================================================================================
            // TURN TWO
            // =====================================================================================
            // Attacker should NOT be able to move on the second turn
            validMove = gameMode.MovePiece(new Vector2Int(4, 7), new Vector2Int(5, 7));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(4, 7)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(5, 7)));

            // Defender should be able to move on the second turn
            validMove = gameMode.MovePiece(new Vector2Int(4, 6), new Vector2Int(3, 6));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(3, 6)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(4, 6)));

            // =====================================================================================
            // TURN THREE
            // =====================================================================================
            // Defender should NOT be able to move on the third turn
            validMove = gameMode.MovePiece(new Vector2Int(5, 4), new Vector2Int(5, 3));
            Assert.AreEqual(InvalidMoveMessage.WrongPlayerTurn, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            // Check that the piece has NOT been moved
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(5, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(5, 3)));

            // Attacker should be able to move on the third turn
            validMove = gameMode.MovePiece(new Vector2Int(8, 5), new Vector2Int(8, 7));
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            // Check that the piece has been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(8, 7)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(8, 5)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void CannotJumpPiecesOverOtherPieces()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker jumps a piece over a defender's piece horizontally
            bool validMove = gameMode.MovePiece(new Vector2Int(0, 3), new Vector2Int(5, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotJumpOverPieces, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot jump a piece over another piece
            Assert.IsFalse(validMove);

            // Attacker jumps a piece over a defender's piece vertically
            validMove = gameMode.MovePiece(new Vector2Int(5, 8), new Vector2Int(5, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.CannotJumpOverPieces, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot jump a piece over another piece
            Assert.IsFalse(validMove);

            // Make sure that the pieces hasn't been moved
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(0, 3)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(5, 8)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OnlyOrthogonalMovementAllowed()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker tries to move diagonally
            bool validMove = gameMode.MovePiece(new Vector2Int(5, 8), new Vector2Int(6, 7));

            // Check that attacker cannot move diagonally
            Assert.AreEqual(InvalidMoveMessage.OnlyOrthogonalMovementAllowed, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(5, 8)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(6, 7)));

            // Attacker tries to make a random non-orthogonal movement
            validMove = gameMode.MovePiece(new Vector2Int(7, 4), new Vector2Int(2, 5));

            // Check that attacker cannot move in a random non-orthogonal movement
            Assert.AreEqual(InvalidMoveMessage.OnlyOrthogonalMovementAllowed, gameMode.LastInvalidMoveMessage);
            Assert.IsFalse(validMove);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(7, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(2, 5)));

            // Attacker tries to make a valid vertical movement
            validMove = gameMode.MovePiece(new Vector2Int(7, 4), new Vector2Int(7, 0));

            // Check that attacker can move
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(7, 4)));
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(7, 0)));

            // Defender tries to make a valid horizontal movement
            validMove = gameMode.MovePiece(new Vector2Int(4, 2), new Vector2Int(8, 2));

            // Check that defender can move
            Assert.AreEqual(InvalidMoveMessage.None, gameMode.LastInvalidMoveMessage);
            Assert.IsTrue(validMove);
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(4, 2)));
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(8, 2)));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        [Test]
        public void OnlyMovementWithinBoardSquaresAllowed()
        {
            // Setup game mode
            TablutGameMode gameMode = new();

            // Attacker makes invalid starting moves
            bool validMove = gameMode.MovePiece(new Vector2Int(-1, 3), new Vector2Int(1, 3));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Attacker makes invalid destination moves
            validMove = gameMode.MovePiece(new Vector2Int(8, 3), new Vector2Int(8, -1));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Attacker makes invalid starting and destination moves
            validMove = gameMode.MovePiece(new Vector2Int(3, 9), new Vector2Int(-1, 9));

            // Check the last invalid move message
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);

            // Check that attacker cannot make invalid out of bound moves
            Assert.IsFalse(validMove);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
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
            gameMode.MovePiece(new Vector2Int(0, 0), new Vector2Int(0, 1)); // Attacker random moves
            gameMode.MovePiece(new Vector2Int(4, 4), new Vector2Int(4, 5)); // King moves out of castle
            gameMode.MovePiece(new Vector2Int(0, 1), new Vector2Int(0, 2)); // Attacker random moves

            // Now try to move the King back into the castle
            bool isValid = gameMode.MovePiece(new Vector2Int(4, 5), new Vector2Int(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
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
            bool isValid = gameMode.MovePiece(new Vector2Int(3, 4), new Vector2Int(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Attacker move out of the way
            gameMode.MovePiece(new Vector2Int(3, 4), new Vector2Int(2, 4));

            // Defender tries to move into castle
            isValid = gameMode.MovePiece(new Vector2Int(4, 3), new Vector2Int(4, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
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
            bool isValid = gameMode.MovePiece(new Vector2Int(3, 4), new Vector2Int(5, 4));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Attacker move out of the way
            gameMode.MovePiece(new Vector2Int(3, 4), new Vector2Int(2, 4));

            // Defender tries to move into castle
            isValid = gameMode.MovePiece(new Vector2Int(4, 3), new Vector2Int(4, 5));

            // Make sure that the move above is invalid
            Assert.IsFalse(isValid);
            Assert.AreEqual(InvalidMoveMessage.CannotMoveIntoCastleSquare, gameMode.LastInvalidMoveMessage);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, TablutPieceType,
            Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            AttackerCaptureTestCases()
        {
            // Captive's starting pos / Capturer 1's starting pos / Capturer 2's starting pos / Capturer 2's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(0, 0), new Vector2Int(0, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(2, 0), new Vector2Int(2, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 2), new Vector2Int(0, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(8, 1), new Vector2Int(8, 2), new Vector2Int(6, 0), new Vector2Int(8, 0));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(0, 0), new Vector2Int(0, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(2, 0), new Vector2Int(2, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 2), new Vector2Int(0, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhitePawn,
                new Vector2Int(8, 1), new Vector2Int(8, 2), new Vector2Int(6, 0), new Vector2Int(8, 0));

            // King cannot start at or move to the edge, so let's move them inward
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(1, 1), new Vector2Int(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new Vector2Int(2, 2), new Vector2Int(1, 2), new Vector2Int(3, 1), new Vector2Int(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new Vector2Int(1, 2), new Vector2Int(1, 1), new Vector2Int(2, 3), new Vector2Int(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn, TablutPieceType.WhiteKing,
                new Vector2Int(7, 2), new Vector2Int(7, 3), new Vector2Int(5, 1), new Vector2Int(7, 1));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(1, 1), new Vector2Int(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new Vector2Int(2, 2), new Vector2Int(1, 2), new Vector2Int(3, 1), new Vector2Int(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new Vector2Int(1, 2), new Vector2Int(1, 1), new Vector2Int(2, 3), new Vector2Int(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing, TablutPieceType.WhitePawn,
                new Vector2Int(7, 2), new Vector2Int(7, 3), new Vector2Int(5, 1), new Vector2Int(7, 1));

            // King should be capturable, although, do not destroy the piece, just end the game
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(1, 1), new Vector2Int(1, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(2, 2), new Vector2Int(1, 2), new Vector2Int(3, 1), new Vector2Int(3, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(1, 2), new Vector2Int(1, 1), new Vector2Int(2, 3), new Vector2Int(1, 3));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn, TablutPieceType.BlackPawn,
                new Vector2Int(7, 2), new Vector2Int(7, 3), new Vector2Int(5, 1), new Vector2Int(7, 1));
        }

        [TestCaseSource(nameof(AttackerCaptureTestCases))]
        public void CapturePawnOnOneDirection((
            TablutPieceType captivePieceType,
            TablutPieceType capturerOnePieceType,
            TablutPieceType capturerTwoPieceType,
            Vector2Int captivePos,
            Vector2Int capturerOnePos,
            Vector2Int capturerTwoPos,
            Vector2Int capturerTwoMovePos) testData)
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
            customBoardPositions[testData.captivePos.x, testData.captivePos.y] = testData.captivePieceType;
            customBoardPositions[testData.capturerOnePos.x, testData.capturerOnePos.y] = testData.capturerOnePieceType;
            customBoardPositions[testData.capturerTwoPos.x, testData.capturerTwoPos.y] = testData.capturerTwoPieceType;
            // Lay some trap and check that these are not counted as capture
            customBoardPositions[1, 6] = TablutPieceType.BlackPawn;
            customBoardPositions[2, 6] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 6] = TablutPieceType.BlackPawn;
            // A random attacker piece for moving first move
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.capturerTwoPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new Vector2Int(4, 8), new Vector2Int(5, 8));
            }

            // Try to move in and capture opponent's pawn
            bool isValid = gameMode.MovePiece(new Vector2Int(testData.capturerTwoPos.x, testData.capturerTwoPos.y),
                new Vector2Int(testData.capturerTwoMovePos.x, testData.capturerTwoMovePos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isValid);

            // If captive is not a King, check to make sure that the defender's pawn has been captured
            if (testData.captivePieceType != TablutPieceType.WhiteKing)
            {
                Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));

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
                Assert.AreNotEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));
                Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);

                // If King has been captured, make sure that we can't continue the game
                // Assume the next player is White
                // Try to move white pawn to see if the game can be continued
                isValid = gameMode.MovePiece(new Vector2Int(2, 6), new Vector2Int(2, 5));

                // If the move is valid, then its wrong
                Assert.IsFalse(isValid);

                // Make sure that you can't move due to the game ended
                Assert.AreEqual(InvalidMoveMessage.GameEnded, gameMode.LastInvalidMoveMessage);

                // Make sure that Black has won the game
                Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            }

            // Make sure the static position is not counted as capture
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(2, 6)));
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, Vector2Int, Vector2Int, Vector2Int)>
            AttackerNoCaptureTestCases()
        {
            // Captive's colour | Captor's colour | Captive's start pos | Captor's start pos | Captor's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(0, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(1, 1), new Vector2Int(2, 0), new Vector2Int(2, 1));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(0, 1), new Vector2Int(1, 2), new Vector2Int(0, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(8, 1), new Vector2Int(6, 0), new Vector2Int(8, 0));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(0, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(1, 1), new Vector2Int(2, 0), new Vector2Int(2, 1));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(0, 1), new Vector2Int(1, 2), new Vector2Int(0, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(8, 1), new Vector2Int(6, 0), new Vector2Int(8, 0));

            // King cannot start at or move to the edge, so let's move them inward
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new Vector2Int(2, 2), new Vector2Int(1, 1), new Vector2Int(1, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new Vector2Int(2, 2), new Vector2Int(3, 1), new Vector2Int(3, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new Vector2Int(1, 2), new Vector2Int(2, 3), new Vector2Int(1, 3));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhiteKing,
                new Vector2Int(7, 2), new Vector2Int(5, 1), new Vector2Int(7, 1));
        }

        [TestCaseSource(nameof(AttackerNoCaptureTestCases))]
        public void NoCaptureIfNotSurroundedOrthogonally((
            TablutPieceType captivePieceType,
            TablutPieceType captorPieceType,
            Vector2Int captivePos,
            Vector2Int captorOnePos,
            Vector2Int captorOneMovePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];

            // Add a King if there's none
            if (testData.captorPieceType != TablutPieceType.WhiteKing)
            {
                customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            }

            // Put the pieces near the edge so we can verify possible logic error
            customBoardPositions[testData.captivePos.x, testData.captivePos.y] = testData.captivePieceType;
            customBoardPositions[testData.captorOnePos.x, testData.captorOnePos.y] = testData.captorPieceType;
            // Lay some trap and check that these are not counted as capture
            customBoardPositions[1, 6] = TablutPieceType.BlackPawn;
            customBoardPositions[2, 6] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 6] = TablutPieceType.BlackPawn;
            // A random attacker piece for moving first move
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.captorPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new Vector2Int(4, 8), new Vector2Int(5, 8));
            }

            // Try to move in and capture opponent's pawn
            bool isValid = gameMode.MovePiece(new Vector2Int(testData.captorOnePos.x, testData.captorOnePos.y),
                new Vector2Int(testData.captorOneMovePos.x, testData.captorOneMovePos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Check to make sure that the pawn has NOT been captured
            Assert.IsTrue(isValid, gameMode.LastInvalidMoveMessage.ToString());
            Assert.AreEqual(testData.captivePieceType, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);

            // Make sure the static position is not counted as capture
            Assert.AreEqual(TablutPieceType.WhitePawn, gameMode.GetPiece(new Vector2Int(2, 6)));
        }

        public static IEnumerable<(TablutPieceType, TablutPieceType, Vector2Int, Vector2Int, Vector2Int)>
            CaptureBetweenEmptyCastleTestCases()
        {
            // Captive's colour | Captor's colour | Captive's start pos | Captor's start pos | Captor's target pos
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(3, 4), new Vector2Int(2, 3), new Vector2Int(2, 4));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(4, 3), new Vector2Int(5, 2), new Vector2Int(4, 2));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(5, 4), new Vector2Int(6, 5), new Vector2Int(6, 4));
            yield return (TablutPieceType.WhitePawn, TablutPieceType.BlackPawn,
                new Vector2Int(4, 5), new Vector2Int(4, 7), new Vector2Int(4, 6));

            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(3, 4), new Vector2Int(2, 3), new Vector2Int(2, 4));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(4, 3), new Vector2Int(5, 2), new Vector2Int(4, 2));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(5, 4), new Vector2Int(6, 5), new Vector2Int(6, 4));
            yield return (TablutPieceType.BlackPawn, TablutPieceType.WhitePawn,
                new Vector2Int(4, 5), new Vector2Int(4, 7), new Vector2Int(4, 6));

            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new Vector2Int(3, 4), new Vector2Int(2, 3), new Vector2Int(2, 4));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new Vector2Int(4, 3), new Vector2Int(5, 2), new Vector2Int(4, 2));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new Vector2Int(5, 4), new Vector2Int(6, 5), new Vector2Int(6, 4));
            yield return (TablutPieceType.WhiteKing, TablutPieceType.BlackPawn,
                new Vector2Int(4, 5), new Vector2Int(4, 7), new Vector2Int(4, 6));
        }

        [TestCaseSource(nameof(CaptureBetweenEmptyCastleTestCases))]
        public void CapturePawnInBetweenEnemyAndEmptyCastle((
            TablutPieceType captivePieceType,
            TablutPieceType captorPieceType,
            Vector2Int captivePos,
            Vector2Int captorOnePos,
            Vector2Int captorOneMovePos) testData)
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
            customBoardPositions[testData.captivePos.x, testData.captivePos.y] = testData.captivePieceType;
            customBoardPositions[testData.captorOnePos.x, testData.captorOnePos.y] = testData.captorPieceType;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // If capturer is not the attacker, then move attacker's piece for first move, otherwise defender can't move
            if (testData.captorPieceType != TablutPieceType.BlackPawn)
            {
                gameMode.MovePiece(new Vector2Int(4, 8), new Vector2Int(5, 8));
            }

            // Make a move that captures enemy pawn against an empty castle
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOnePos.x, testData.captorOnePos.y),
                new Vector2Int(testData.captorOneMovePos.x, testData.captorOneMovePos.y));
            
            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // If captive is not a King, just check if the square is now empty
            if (testData.captivePieceType != TablutPieceType.WhiteKing)
            {
                // Check to make sure that the target pawn has been captured
                Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

                Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));
            }
            // If captive is a King, make sure the capture is invalid
            else
            {
                // Make sure that the move is still valid, even if we didn't capture the King
                Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

                Assert.AreNotEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));
                Assert.AreNotEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);

                // Make sure that the game is still in playing state
                Assert.AreEqual(GameState.Playing, gameMode.GameState);
            }
        }

        public static IEnumerable<(Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            CaptureKingNearEmptyCastleTestCases()
        {
            // King's start pos | Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos
            yield return (new Vector2Int(3, 4), new Vector2Int(2, 3), new Vector2Int(2, 4),
                new Vector2Int(3, 3), new Vector2Int(3, 5));
            yield return (new Vector2Int(4, 3), new Vector2Int(5, 2), new Vector2Int(4, 2),
                new Vector2Int(3, 3), new Vector2Int(5, 3));
            yield return (new Vector2Int(5, 4), new Vector2Int(6, 5), new Vector2Int(6, 4),
                new Vector2Int(5, 3), new Vector2Int(5, 5));
            yield return (new Vector2Int(4, 5), new Vector2Int(4, 7), new Vector2Int(4, 6),
                new Vector2Int(3, 5), new Vector2Int(5, 5));
        }

        [TestCaseSource(nameof(CaptureKingNearEmptyCastleTestCases))]
        public void CaptureKingWhenNearEmptyCastleAndSurroundedByThreeEnemies((
            Vector2Int kingStartPos,
            Vector2Int captorOneStartPos,
            Vector2Int captorOneTargetPos,
            Vector2Int captorTwoPos,
            Vector2Int captorThreePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[testData.kingStartPos.x, testData.kingStartPos.y] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.x, testData.captorOneStartPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.x, testData.captorTwoPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.x, testData.captorThreePos.y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOneStartPos.x, testData.captorOneStartPos.y),
                new Vector2Int(testData.captorOneTargetPos.x, testData.captorOneTargetPos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has won
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            CaptureKingInCastleTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new Vector2Int(3, 3), new Vector2Int(4, 3),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(2, 4), new Vector2Int(3, 4),
                new Vector2Int(4, 3), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(5, 5), new Vector2Int(4, 5),
                new Vector2Int(3, 4), new Vector2Int(4, 3), new Vector2Int(5, 4));
            yield return (new Vector2Int(7, 4), new Vector2Int(5, 4),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(4, 3));
        }

        [TestCaseSource(nameof(CaptureKingInCastleTestCases))]
        public void CaptureKingInCastleWhenSurroundedByFourEnemies((
            Vector2Int captorOneStartPos,
            Vector2Int captorOneTargetPos,
            Vector2Int captorTwoPos,
            Vector2Int captorThreePos,
            Vector2Int captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.x, testData.captorOneStartPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.x, testData.captorTwoPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.x, testData.captorThreePos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.x, testData.captorFourPos.y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOneStartPos.x, testData.captorOneStartPos.y),
                new Vector2Int(testData.captorOneTargetPos.x, testData.captorOneTargetPos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has won
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            NoCaptureKingInCastleTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos
            // | Captor 2's pos | Captor 3's pos
            yield return (new Vector2Int(3, 3), new Vector2Int(4, 3),
                new Vector2Int(3, 4), new Vector2Int(4, 5));
            yield return (new Vector2Int(2, 4), new Vector2Int(3, 4),
                new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(5, 5), new Vector2Int(4, 5),
                new Vector2Int(3, 4), new Vector2Int(5, 4));
            yield return (new Vector2Int(7, 4), new Vector2Int(5, 4),
                new Vector2Int(3, 4), new Vector2Int(4, 5));
        }

        [TestCaseSource(nameof(NoCaptureKingInCastleTestCases))]
        public void NoCaptureWhenKingIsNotSurroundedByFourEnemiesWhileInCastle((
            Vector2Int captorOneStartPos,
            Vector2Int captorOneTargetPos,
            Vector2Int captorTwoPos,
            Vector2Int captorThreePos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.x, testData.captorOneStartPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.x, testData.captorTwoPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.x, testData.captorThreePos.y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOneStartPos.x, testData.captorOneStartPos.y),
                new Vector2Int(testData.captorOneTargetPos.x, testData.captorOneTargetPos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that black has not won
            Assert.AreNotEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreNotEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
        }

        public static IEnumerable<(Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            CaptureDefenderWhenNearKingTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos | Captive's pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(4, 3),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4),
                new Vector2Int(4, 3), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(5, 6), new Vector2Int(4, 6), new Vector2Int(4, 5),
                new Vector2Int(3, 4), new Vector2Int(4, 3), new Vector2Int(5, 4));
            yield return (new Vector2Int(7, 4), new Vector2Int(6, 4), new Vector2Int(5, 4),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(4, 3));
        }

        [TestCaseSource(nameof(CaptureDefenderWhenNearKingTestCases))]
        public void CaptureDefenderWhenNearKingInCastleSurroundedByThreeAttackers((
            Vector2Int captorOneStartPos,
            Vector2Int captorOneTargetPos,
            Vector2Int captivePos,
            Vector2Int captorTwoPos,
            Vector2Int captorThreePos,
            Vector2Int captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.x, testData.captorOneStartPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captivePos.x, testData.captivePos.y] = TablutPieceType.WhitePawn;
            customBoardPositions[testData.captorTwoPos.x, testData.captorTwoPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.x, testData.captorThreePos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.x, testData.captorFourPos.y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOneStartPos.x, testData.captorOneStartPos.y),
                new Vector2Int(testData.captorOneTargetPos.x, testData.captorOneTargetPos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Check to make sure that the white pawn has been captured
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(testData.captivePos.x, testData.captivePos.y)));
        }

        public static IEnumerable<(Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int, Vector2Int)>
            NoCaptureAttackerWhenNearKingTestCases()
        {
            // Captor 1's start pos | Captor 1's target pos | Target captor's pos
            // | Captor 2's pos | Captor 3's pos | Captor 4's pos
            yield return (new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(4, 3),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4),
                new Vector2Int(4, 3), new Vector2Int(4, 5), new Vector2Int(5, 4));
            yield return (new Vector2Int(5, 6), new Vector2Int(4, 6), new Vector2Int(4, 5),
                new Vector2Int(3, 4), new Vector2Int(4, 3), new Vector2Int(5, 4));
            yield return (new Vector2Int(7, 4), new Vector2Int(6, 4), new Vector2Int(5, 4),
                new Vector2Int(3, 4), new Vector2Int(4, 5), new Vector2Int(4, 3));
        }

        [TestCaseSource(nameof(NoCaptureAttackerWhenNearKingTestCases))]
        public void NoCaptureAttackerPawnWhenNearKingInCastleSurroundedByThreeAttackers((
            Vector2Int captorOneStartPos,
            Vector2Int captorOneTargetPos,
            Vector2Int targetCaptorPos,
            Vector2Int captorTwoPos,
            Vector2Int captorThreePos,
            Vector2Int captorFourPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;
            customBoardPositions[testData.captorOneStartPos.x, testData.captorOneStartPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.targetCaptorPos.x, testData.targetCaptorPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorTwoPos.x, testData.captorTwoPos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorThreePos.x, testData.captorThreePos.y] = TablutPieceType.BlackPawn;
            customBoardPositions[testData.captorFourPos.x, testData.captorFourPos.y] = TablutPieceType.BlackPawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Now make a move to capture the King
            bool isCapturingMoveValid = gameMode.MovePiece(new Vector2Int(testData.captorOneStartPos.x, testData.captorOneStartPos.y),
                new Vector2Int(testData.captorOneTargetPos.x, testData.captorOneTargetPos.y));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Make sure the move is valid
            Assert.IsTrue(isCapturingMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that nothing happens
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(testData.targetCaptorPos.x, testData.targetCaptorPos.y)));
        }

        public static IEnumerable<(Vector2Int, Vector2Int)> KingMoveToWinTestCases()
        {
            // King's starting pos | King's target pos
            yield return (new Vector2Int(3, 4),  new Vector2Int(0, 4));
            yield return (new Vector2Int(4, 4),  new Vector2Int(4, 0));
            yield return (new Vector2Int(7, 4),  new Vector2Int(8, 4));
            yield return (new Vector2Int(7, 7),  new Vector2Int(7, 8));
        }

        [TestCaseSource(nameof(KingMoveToWinTestCases))]
        public void WhiteWinsWhenKingReachesTheEdge((
            Vector2Int kingStartingPos,
            Vector2Int kingTargetPos) testData)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[testData.kingStartingPos.x, testData.kingStartingPos.y] = TablutPieceType.WhiteKing;
            // Put random pawns
            customBoardPositions[1, 1] = TablutPieceType.BlackPawn;
            customBoardPositions[5, 5] = TablutPieceType.WhitePawn;

            // Setup game mode
            TablutGameMode gameMode = new(customBoardPositions);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure that the game is still on-going
            Assert.AreEqual(GameState.Playing, gameMode.GameState);

            // Black moves first, just make a random move
            gameMode.MovePiece(new Vector2Int(1, 1), new Vector2Int(2, 1));

            // Now White King makes the move
            bool isMoveValid = gameMode.MovePiece(testData.kingStartingPos, testData.kingTargetPos);

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

            // Output the game state
            Debug.Log($"Game state: {gameMode.GameState} - Last move message: {gameMode.LastMoveMessage}");

            // Make sure the move is valid
            Assert.IsTrue(isMoveValid, gameMode.LastInvalidMoveMessage.ToString());

            // Make sure that White has won
            Assert.AreEqual(GameState.WhiteWon, gameMode.GameState);

            // Make sure that the game can't be continued by trying to move the Black Pawn
            isMoveValid = gameMode.MovePiece(new Vector2Int(2, 1), new Vector2Int(3, 1));

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
            Debug.Log($"Initial pieces locations:{Environment.NewLine}{gameMode}");

            // Attacker capture defender's pawn
            gameMode.MovePiece(new Vector2Int(7, 6), new Vector2Int(7, 5));
            // Defender makes the next move
            gameMode.MovePiece(new Vector2Int(1, 1), new Vector2Int(2, 1));

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            Debug.Log($"Updated pieces locations:{Environment.NewLine}{gameMode}");

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

            Assert.IsTrue(gameMode.MovePiece(new Vector2Int(fromX, fromY), new Vector2Int(toX, toY)));
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

            Assert.IsTrue(gameMode.MovePiece(new Vector2Int(1, 0), new Vector2Int(1, 1)));
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(new Vector2Int(1, 1)));
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

            Assert.IsTrue(gameMode.MovePiece(new Vector2Int(4, 1), new Vector2Int(4, 3)));
            Assert.AreEqual(MoveMessage.CapturedDefenderKing, gameMode.LastMoveMessage);
            Assert.AreEqual(GameState.BlackWon, gameMode.GameState);
            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new Vector2Int(4, 4)));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(5, 3)));
        }

        [Test]
        public void RejectsCustomBoardWithInvalidDimensions()
        {
            TablutPieceType[,] positions = new TablutPieceType[8, 8];

            Assert.Throws<ArgumentException>(() => new TablutGameMode(positions));
            Assert.AreEqual(TablutPieceType.WhiteKing, new TablutGameMode().GetPiece(new Vector2Int(4, 4)));
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

            Assert.AreEqual(TablutPieceType.WhiteKing, gameMode.GetPiece(new Vector2Int(4, 4)));
        }

        [Test]
        public void LegalDestinationsDoNotChangeMatchState()
        {
            TablutGameMode gameMode = new();
            Vector2Int from = new(0, 3);

            Assert.IsFalse(gameMode.MovePiece(from, new Vector2Int(-1, 3)));
            CollectionAssert.AreEquivalent(new[]
            {
                new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2),
                new Vector2Int(1, 3), new Vector2Int(2, 3), new Vector2Int(3, 3)
            }, gameMode.GetLegalDestinations(from));
            Assert.IsTrue(gameMode.CanSelectPiece(from));
            Assert.IsFalse(gameMode.CanSelectPiece(new Vector2Int(4, 4)));
            CollectionAssert.IsEmpty(gameMode.GetLegalDestinations(new Vector2Int(4, 4)));

            Assert.AreEqual(TablutPlayer.Attacker, gameMode.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.BlackPawn, gameMode.GetPiece(from));
            Assert.AreEqual(TablutPieceType.Empty, gameMode.GetPiece(new Vector2Int(1, 3)));
            Assert.AreEqual(GameState.Playing, gameMode.GameState);
            Assert.AreEqual(InvalidMoveMessage.OutOfBound, gameMode.LastInvalidMoveMessage);
            Assert.AreEqual(MoveMessage.None, gameMode.LastMoveMessage);
        }

    }
}
