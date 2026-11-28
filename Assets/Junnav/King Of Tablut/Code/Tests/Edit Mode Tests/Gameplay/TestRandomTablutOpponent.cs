using System.Collections.Generic;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;

namespace Junnav.KingOfTablut.Tests.EditMode
{
    public class TestRandomTablutOpponent
    {
        [Test]
        public void EqualSeedsChooseTheSameLegalMoves()
        {
            RandomTablutOpponent firstOpponent = new(24);
            RandomTablutOpponent secondOpponent = new(24);
            TablutGameMode firstMatch = new();
            TablutGameMode secondMatch = new();

            for (int turn = 0; turn < 12; turn++)
            {
                TablutPlayer currentPlayer = firstMatch.CurrentPlayer;

                Assert.IsTrue(firstOpponent.TryChooseMove(firstMatch, out TablutMove first));
                Assert.IsTrue(secondOpponent.TryChooseMove(secondMatch, out TablutMove second));
                Assert.AreEqual(first.From, second.From);
                Assert.AreEqual(first.To, second.To);
                Assert.AreEqual(currentPlayer, firstMatch.CurrentPlayer);
                Assert.IsTrue(firstMatch.CanSelectPiece(first.From));
                CollectionAssert.Contains(firstMatch.GetLegalDestinations(first.From), first.To);
                Assert.IsTrue(firstMatch.MovePiece(first.From, first.To));
                Assert.IsTrue(secondMatch.MovePiece(second.From, second.To));
            }
        }

        [Test]
        public void DifferentSeedsCanChooseDifferentMoves()
        {
            TablutGameMode match = new();
            HashSet<(BoardPosition From, BoardPosition To)> choices = new();
            for (int seed = 0; seed < 8; seed++)
            {
                RandomTablutOpponent opponent = new(seed);
                Assert.IsTrue(opponent.TryChooseMove(match, out TablutMove move));
                choices.Add((move.From, move.To));
            }

            Assert.Greater(choices.Count, 1);
        }

        [Test]
        public void ReturnsWithoutAMoveWhenTheStartingSideIsBlocked()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[4, 4] = TablutPieceType.WhiteKing;
            positions[3, 4] = TablutPieceType.BlackPawn;
            positions[5, 4] = TablutPieceType.BlackPawn;
            positions[4, 3] = TablutPieceType.BlackPawn;
            positions[4, 5] = TablutPieceType.BlackPawn;
            TablutGameMode match = new(positions, TablutPlayer.Defender);
            RandomTablutOpponent opponent = new(24);

            Assert.IsFalse(opponent.TryChooseMove(match, out _));
            Assert.AreEqual(TablutPlayer.Defender, match.CurrentPlayer);
            Assert.AreEqual(GameState.Playing, match.GameState);
        }

        [Test]
        public void ReturnsWithoutAMoveWhenTheMatchHasEnded()
        {
            TablutPieceType[,] positions = new TablutPieceType[9, 9];
            positions[3, 1] = TablutPieceType.WhiteKing;
            positions[6, 6] = TablutPieceType.BlackPawn;
            TablutGameMode match = new(positions, TablutPlayer.Defender);
            RandomTablutOpponent opponent = new(24);

            Assert.IsTrue(match.MovePiece(new BoardPosition(3, 1), new BoardPosition(3, 0)));

            Assert.IsFalse(opponent.TryChooseMove(match, out _));
            Assert.AreEqual(GameState.WhiteWon, match.GameState);
        }
    }
}
