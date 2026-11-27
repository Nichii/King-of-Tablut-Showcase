using System;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;

namespace Junnav.KingOfTablut.Tests.EditMode
{
    public class TestTablutBoard
    {
        [Test]
        public void RulesCompileIntoTheirOwnAssembly()
        {
            var rulesAssembly = typeof(TablutGameMode).Assembly;

            Assert.AreEqual("Junnav.KingOfTablut.Rules", rulesAssembly.GetName().Name);
            Assert.AreSame(rulesAssembly, typeof(TablutBoard).Assembly);
            Assert.IsFalse(Array.Exists(rulesAssembly.GetReferencedAssemblies(),
                reference => reference.Name.StartsWith("Unity", StringComparison.Ordinal)));
        }

        [Test]
        public void BoardPositionsAreEqualWhenBothCoordinatesMatch()
        {
            BoardPosition position = new(2, 4);
            BoardPosition samePosition = new(2, 4);

            Assert.IsTrue(position.Equals(samePosition));
            Assert.IsTrue(position == samePosition);
            Assert.AreEqual(position.GetHashCode(), samePosition.GetHashCode());
            Assert.IsTrue(position != new BoardPosition(4, 4));
            Assert.IsTrue(position != new BoardPosition(2, 5));
        }

        [Test]
        public void SetupTablutBoard()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Get the board positions in an array
            TablutPieceType[,] boardPositions = tablutBoard.GetBoardPositions();

            // Check to make sure there's 9x9 squares
            Assert.AreEqual(9, boardPositions.GetLength(0), "Tablut board positions must have 9 rows!");
            Assert.AreEqual(9, boardPositions.GetLength(1), "Tablut board positions must have 9 columns!");
        }

        [Test]
        public void CheckIsTablutBoardInitialized()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Get the board positions in an array
            TablutPieceType[,] boardPositions = tablutBoard.GetBoardPositions();

            // Verify that all positions is not null and is set to empty piece type
            for (int x = 0; x < boardPositions.GetLength(0); x++)
            {
                for (int y = 0; y < boardPositions.GetLength(1); y++)
                {
                    Assert.IsNotNull(boardPositions[x, y], $"Tablut board position at {x}, {y} is null!");
                    Assert.AreEqual(TablutPieceType.Empty, boardPositions[x, y], $"Tablut board position at {x}, {y} is not set to empty piece!");
                }
            }
        }

        [Test]
        public void OnlyAddPiecesToValidPositions()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Add pieces to invalid positions
            Assert.IsFalse(tablutBoard.SetPiece(TablutPieceType.WhitePawn, new BoardPosition(-1, 0)),
                "Cannot set piece to invalid positions!");
            Assert.IsFalse(tablutBoard.SetPiece(TablutPieceType.BlackPawn, new BoardPosition(9, 0)),
                "Cannot set piece to invalid positions!");
            Assert.IsFalse(tablutBoard.SetPiece(TablutPieceType.WhitePawn, new BoardPosition(0, -1)),
                "Cannot set piece to invalid positions!");
            Assert.IsFalse(tablutBoard.SetPiece(TablutPieceType.BlackPawn, new BoardPosition(0, 9)),
                "Cannot set piece to invalid positions!");

            // Add pieces to valid positions
            Assert.IsTrue(tablutBoard.SetPiece(TablutPieceType.BlackPawn, new BoardPosition(0, 0)),
                "Cannot set piece to invalid positions!");
            Assert.IsTrue(tablutBoard.SetPiece(TablutPieceType.WhitePawn, new BoardPosition(8, 0)),
                "Cannot set piece to invalid positions!");
            Assert.IsTrue(tablutBoard.SetPiece(TablutPieceType.BlackPawn, new BoardPosition(0, 8)),
                "Cannot set piece to invalid positions!");
            Assert.IsTrue(tablutBoard.SetPiece(TablutPieceType.WhitePawn, new BoardPosition(8, 8)),
                "Cannot set piece to invalid positions!");
        }

        [Test]
        public void CheckIfPositionIsValidOnTheBoard()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Check for invalid positions
            Assert.IsFalse(tablutBoard.IsValidPosition(new BoardPosition(-1, 0)));
            Assert.IsFalse(tablutBoard.IsValidPosition(new BoardPosition(9, 0)));
            Assert.IsFalse(tablutBoard.IsValidPosition(new BoardPosition(0, -1)));
            Assert.IsFalse(tablutBoard.IsValidPosition(new BoardPosition(0, 9)));

            // Check for valid positions
            Assert.IsTrue(tablutBoard.IsValidPosition(new BoardPosition(0, 0)));
            Assert.IsTrue(tablutBoard.IsValidPosition(new BoardPosition(8, 0)));
            Assert.IsTrue(tablutBoard.IsValidPosition(new BoardPosition(0, 8)));
            Assert.IsTrue(tablutBoard.IsValidPosition(new BoardPosition(8, 8)));
        }

        [Test]
        public void SetCustomBoardPositions()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Setup positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            customBoardPositions[3, 3] = TablutPieceType.BlackPawn;
            customBoardPositions[5, 6] = TablutPieceType.WhitePawn;
            customBoardPositions[2, 6] = TablutPieceType.WhiteKing;

            // Setup tablut board with the above board positions
            tablutBoard.SetBoardPositions(customBoardPositions);

            // Get the board positions
            TablutPieceType[,] boardPositions = tablutBoard.GetBoardPositions();

            // Check that the custom positions has been properly adhered to
            Assert.AreEqual(TablutPieceType.BlackPawn, boardPositions[3, 3], "The Black Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhitePawn, boardPositions[5, 6], "The White Pawn is not at the right position!");
            Assert.AreEqual(TablutPieceType.WhiteKing, boardPositions[2, 6], "The White King is not at the right position!");

            // Output human readable board positions to log so that we can verify the positions with our own eyes
            TestContext.WriteLine($"Added pieces locations:{Environment.NewLine}{tablutBoard}");
        }

        [Test]
        public void DisallowCustomPositionsWithWrongNumberOfSquares()
        {
            // Setup tablut board
            TablutBoard tablutBoard = new();

            // Setup positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[8, 9];

            // Try to set position to the tablut board
            bool isValid = tablutBoard.SetBoardPositions(customBoardPositions);

            // Make sure that its invalid
            Assert.IsFalse(isValid);

            // Also check to make sure that the tablut board squares are still 9x9
            TablutPieceType[,] boardPositions = tablutBoard.GetBoardPositions();
            Assert.AreEqual(9, boardPositions.GetLength(0));
            Assert.AreEqual(9, boardPositions.GetLength(1));
        }
    }
}
