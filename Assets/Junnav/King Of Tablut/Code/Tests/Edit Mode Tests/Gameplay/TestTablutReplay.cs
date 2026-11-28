using System;
using System.Collections.Generic;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;

namespace Junnav.KingOfTablut.Tests.EditMode
{
    public class TestTablutReplay
    {
        private static readonly TablutMove[] s_escapeMoves =
        {
            new(new BoardPosition(3, 1), new BoardPosition(3, 2)),
            new(new BoardPosition(6, 6), new BoardPosition(6, 5)),
            new(new BoardPosition(3, 2), new BoardPosition(3, 0))
        };

        [Test]
        public void ReplaysACaptureAndWinWithTheSameFinalState()
        {
            TablutPieceType[,] positions = CreateEscapeLayout();
            TablutGameMode played = new(positions, TablutPlayer.Defender);
            foreach (TablutMove move in s_escapeMoves)
            {
                Assert.IsTrue(played.MovePiece(move.From, move.To));
            }

            Assert.IsTrue(TablutReplay.TryReplay(s_escapeMoves, out TablutGameMode first, out var firstFailure,
                positions, TablutPlayer.Defender));
            Assert.IsTrue(TablutReplay.TryReplay(s_escapeMoves, out TablutGameMode second, out var secondFailure,
                positions, TablutPlayer.Defender));

            Assert.IsNull(firstFailure);
            Assert.IsNull(secondFailure);
            Assert.AreNotSame(first, second);
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    BoardPosition square = new(x, y);
                    Assert.AreEqual(played.GetPiece(square), first.GetPiece(square), square.ToString());
                    Assert.AreEqual(first.GetPiece(square), second.GetPiece(square), square.ToString());
                }
            }

            Assert.AreEqual(played.CurrentPlayer, first.CurrentPlayer);
            Assert.AreEqual(first.CurrentPlayer, second.CurrentPlayer);
            Assert.AreEqual(GameState.WhiteWon, played.GameState);
            Assert.AreEqual(played.GameState, first.GameState);
            Assert.AreEqual(first.GameState, second.GameState);
            Assert.AreEqual(TablutPieceType.Empty, first.GetPiece(new BoardPosition(2, 2)));
            Assert.AreEqual(TablutPieceType.WhiteKing, first.GetPiece(new BoardPosition(3, 0)));
            Assert.AreEqual(TablutPieceType.BlackPawn, positions[2, 2]);
            Assert.AreEqual(TablutPieceType.WhiteKing, positions[3, 1]);
        }

        [Test]
        public void EmptyHistoryReturnsAStandardMatch()
        {
            Assert.IsTrue(TablutReplay.TryReplay(Array.Empty<TablutMove>(), out TablutGameMode match, out var failure));

            Assert.IsNull(failure);
            Assert.AreEqual(GameState.Playing, match.GameState);
            Assert.AreEqual(TablutPlayer.Attacker, match.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.WhiteKing, match.GetPiece(TablutBoard.CastleSquare));
        }

        [Test]
        public void StopsAtTheFirstIllegalMove()
        {
            Assert.IsFalse(TablutReplay.TryReplay(IllegalHistory(), out TablutGameMode match, out var failure));

            Assert.IsNull(match);
            Assert.AreEqual((1, InvalidMoveMessage.OutOfBound), failure);
        }

        [Test]
        public void RejectsMovesAfterTheMatchEnds()
        {
            List<TablutMove> moves = new(s_escapeMoves)
            {
                new(new BoardPosition(6, 5), new BoardPosition(6, 4))
            };

            Assert.IsFalse(TablutReplay.TryReplay(moves, out TablutGameMode match, out var failure,
                CreateEscapeLayout(), TablutPlayer.Defender));

            Assert.IsNull(match);
            Assert.AreEqual((3, InvalidMoveMessage.GameEnded), failure);
        }

        [Test]
        public void RejectsNullHistory()
        {
            Assert.Throws<ArgumentNullException>(() => TablutReplay.TryReplay(null, out _, out _));
        }

        [Test]
        public void ReplaySaveRoundTripsACaptureAndWin()
        {
            TablutPieceType[,] positions = CreateEscapeLayout();
            string save = TablutReplaySave.Write(s_escapeMoves, positions, TablutPlayer.Defender);
            string expectedSave = string.Join("\n", new[]
            {
                "TABLUT-REPLAY 1",
                "D",
                ".........",
                "...K.....",
                ".DA......",
                ".........",
                ".........",
                ".........",
                "......A..",
                ".........",
                ".........",
                "3,1,3,2",
                "6,6,6,5",
                "3,2,3,0",
                string.Empty
            });

            Assert.AreEqual(expectedSave, save);
            Assert.IsTrue(TablutReplay.TryReplay(s_escapeMoves, out TablutGameMode expected, out _,
                positions, TablutPlayer.Defender));

            Assert.IsTrue(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error), error);
            Assert.IsNull(error);
            Assert.AreEqual(GameState.WhiteWon, loaded.GameState);
            Assert.AreEqual(expected.CurrentPlayer, loaded.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.Empty, loaded.GetPiece(new BoardPosition(2, 2)));
            Assert.AreEqual(TablutPieceType.WhiteKing, loaded.GetPiece(new BoardPosition(3, 0)));

            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    BoardPosition square = new(x, y);
                    Assert.AreEqual(expected.GetPiece(square), loaded.GetPiece(square), square.ToString());
                }
            }
        }

        [Test]
        public void EmptyReplaySaveRestoresTheStandardOpening()
        {
            string save = TablutReplaySave.Write(Array.Empty<TablutMove>());

            Assert.IsTrue(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error), error);
            Assert.IsNull(error);
            Assert.AreEqual(TablutPlayer.Attacker, loaded.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.WhiteKing, loaded.GetPiece(TablutBoard.CastleSquare));
            Assert.AreEqual(TablutPieceType.BlackPawn, loaded.GetPiece(new BoardPosition(0, 3)));
        }

        [TestCase(null, "header")]
        [TestCase("broken\n", "header")]
        [TestCase("TABLUT-REPLAY 2\n", "version")]
        [TestCase("TABLUT-REPLAY 1\nX\n", "starting side")]
        [TestCase("TABLUT-REPLAY 1\nA\n", "board row")]
        [TestCase("TABLUT-REPLAY 1\nA\n....X....\n", "piece")]
        public void RejectsMalformedReplaySaves(string save, string reason)
        {
            Assert.IsFalse(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error));
            Assert.IsNull(loaded);
            StringAssert.Contains(reason, error);
        }

        [Test]
        public void RejectsASavedBoardWithoutAKing()
        {
            string save = TablutReplaySave.Write(Array.Empty<TablutMove>()).Replace('K', '.');

            Assert.IsFalse(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error));
            Assert.IsNull(loaded);
            StringAssert.Contains("exactly one king", error);
        }

        [TestCase("4,2,9,2\n", "OutOfBound")]
        [TestCase("4,2,broken,2\n", "invalid coordinates")]
        public void RejectsTheFirstInvalidSavedMove(string invalidLine, string reason)
        {
            string save = TablutReplaySave.Write(Array.Empty<TablutMove>()) + "0,3,0,2\n" + invalidLine;

            Assert.IsFalse(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error));
            Assert.IsNull(loaded);
            StringAssert.Contains("Move 1", error);
            StringAssert.Contains(reason, error);
        }

        [Test]
        public void RejectsASavedMoveAfterTheWin()
        {
            string save = TablutReplaySave.Write(s_escapeMoves, CreateEscapeLayout(), TablutPlayer.Defender)
                + "6,5,6,4\n";

            Assert.IsFalse(TablutReplaySave.TryLoad(save, out TablutGameMode loaded, out string error));
            Assert.IsNull(loaded);
            StringAssert.Contains("Move 3", error);
            StringAssert.Contains(nameof(InvalidMoveMessage.GameEnded), error);
        }

        [Test]
        public void RefusesToWriteAHistoryWithAnIllegalMove()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => TablutReplaySave.Write(IllegalHistory()));

            StringAssert.Contains("Move 1", exception.Message);
            StringAssert.Contains(nameof(InvalidMoveMessage.OutOfBound), exception.Message);
        }

        private static IEnumerable<TablutMove> IllegalHistory()
        {
            yield return new TablutMove(new BoardPosition(0, 3), new BoardPosition(0, 2));
            yield return new TablutMove(new BoardPosition(4, 2), new BoardPosition(9, 2));
            Assert.Fail("Replay requested a move after the rejected move.");
        }

        private static TablutPieceType[,] CreateEscapeLayout()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[3, 1] = TablutPieceType.WhiteKing;
            positions[1, 2] = TablutPieceType.WhitePawn;
            positions[2, 2] = TablutPieceType.BlackPawn;
            positions[6, 6] = TablutPieceType.BlackPawn;
            return positions;
        }
    }
}
