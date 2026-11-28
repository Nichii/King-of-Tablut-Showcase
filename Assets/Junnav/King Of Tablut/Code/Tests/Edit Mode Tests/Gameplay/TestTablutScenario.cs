using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Junnav.KingOfTablut.Tests.EditMode
{
    public class TestTablutScenario
    {
        private TablutScenario scenario;

        [SetUp]
        public void CreateScenario()
        {
            scenario = ScriptableObject.CreateInstance<TablutScenario>();
        }

        [TearDown]
        public void DestroyScenario()
        {
            Object.DestroyImmediate(scenario);
        }

        [Test]
        public void ScenarioCreatesIndependentMatchesWithTheConfiguredTurn()
        {
            SetLayout(TablutPlayer.Defender,
                (TablutPieceType.WhiteKing, 4, 2), (TablutPieceType.BlackPawn, 1, 1));

            Assert.IsTrue(scenario.TryCreateMatch(out TablutGameMode first, out string error), error);
            Assert.AreEqual(TablutPlayer.Defender, first.CurrentPlayer);
            Assert.IsTrue(first.MovePiece(new BoardPosition(4, 2), new BoardPosition(4, 0)));
            Assert.AreEqual(GameState.WhiteWon, first.GameState);

            Assert.IsTrue(scenario.TryCreateMatch(out TablutGameMode second, out error), error);
            Assert.AreEqual(TablutPlayer.Defender, second.CurrentPlayer);
            Assert.AreEqual(TablutPieceType.WhiteKing, second.GetPiece(new BoardPosition(4, 2)));
            Assert.AreEqual(TablutPieceType.Empty, second.GetPiece(new BoardPosition(4, 0)));
        }

        [TestCase(-1, 2)]
        [TestCase(4, 9)]
        public void RejectsPiecesOutsideTheBoard(int x, int y)
        {
            SetLayout(TablutPlayer.Defender, (TablutPieceType.WhiteKing, x, y));

            AssertInvalidScenario("outside the board");
        }

        [Test]
        public void RejectsDuplicateSquares()
        {
            SetLayout(TablutPlayer.Defender,
                (TablutPieceType.WhiteKing, 4, 2), (TablutPieceType.BlackPawn, 4, 2));

            AssertInvalidScenario("more than one piece");
        }

        [Test]
        public void RejectsAPawnOnTheThrone()
        {
            SetLayout(TablutPlayer.Defender,
                (TablutPieceType.WhiteKing, 4, 2), (TablutPieceType.BlackPawn, 4, 4));

            AssertInvalidScenario("only the king can start on the throne");
        }

        [Test]
        public void RejectsAPieceTypeThatCannotBePlaced()
        {
            SetLayout(TablutPlayer.Defender,
                (TablutPieceType.WhiteKing, 4, 2), (TablutPieceType.Empty, 1, 1));

            AssertInvalidScenario("invalid piece type");
        }

        [Test]
        public void RejectsAKingThatStartsOnAnEdge()
        {
            SetLayout(TablutPlayer.Defender, (TablutPieceType.WhiteKing, 0, 2));

            AssertInvalidScenario("king must start away from the edge");
        }

        [Test]
        public void RejectsASideWithNoLegalOpeningMove()
        {
            SetLayout(TablutPlayer.Defender, (TablutPieceType.WhiteKing, 4, 4),
                (TablutPieceType.BlackPawn, 3, 4), (TablutPieceType.BlackPawn, 5, 4),
                (TablutPieceType.BlackPawn, 4, 3), (TablutPieceType.BlackPawn, 4, 5));

            AssertInvalidScenario("has no legal opening move");
        }

        [Test]
        public void RejectsAnUndefinedStartingSide()
        {
            SetLayout((TablutPlayer)2, (TablutPieceType.WhiteKing, 4, 2));

            AssertInvalidScenario("Choose either attackers or defenders");
        }

        private void AssertInvalidScenario(string reason)
        {
            Assert.IsFalse(scenario.TryCreateMatch(out TablutGameMode gameMode, out string error));
            Assert.IsNull(gameMode);
            StringAssert.Contains(reason, error);
        }

        private void SetLayout(TablutPlayer startingPlayer, params (TablutPieceType type, int x, int y)[] pieces)
        {
            SerializedObject serialized = new(scenario);
            serialized.FindProperty("startingPlayer").intValue = (int)startingPlayer;
            SerializedProperty placements = serialized.FindProperty("pieces");
            placements.arraySize = pieces.Length;
            for (int i = 0; i < pieces.Length; i++)
            {
                SerializedProperty placement = placements.GetArrayElementAtIndex(i);
                placement.FindPropertyRelative("pieceType").intValue = (int)pieces[i].type;
                placement.FindPropertyRelative("x").intValue = pieces[i].x;
                placement.FindPropertyRelative("y").intValue = pieces[i].y;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
