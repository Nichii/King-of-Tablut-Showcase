using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;

namespace Junnav.KingOfTablut.Tests.PlayMode
{
    public class TestTablutPlayersTurnState
    {
        [Test]
        public void AttackingSidesTurnFirst()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Check to make sure attacker starts their turn first
            Assert.AreEqual(TablutPlayer.Attacker, playerTurnState.GetCurrentPlayerTurn());
        }

        [Test]
        public void DefendingSidesTurnNext()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure it's currently defender's turn
            Assert.AreEqual(TablutPlayer.Defender, playerTurnState.GetCurrentPlayerTurn());
        }

        [Test]
        public void CycleBackToAttackerAfterDefendersTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Advance again
            playerTurnState.GoToNextPlayer();

            // Check to make sure it's attacker's turn now
            Assert.AreEqual(TablutPlayer.Attacker, playerTurnState.GetCurrentPlayerTurn());
        }

        [Test]
        public void AttackerCanMoveOnFirstTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Check to make sure attacker can move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove(TablutPlayer.Attacker));
        }

        [Test]
        public void DefenderCantMoveIfAttackersTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Check to make sure defender can't move right now
            Assert.IsFalse(playerTurnState.CanPlayerMove(TablutPlayer.Defender));
        }

        [Test]
        public void AttackerCannotMoveOnSecondTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure attacker can't move right now
            Assert.IsFalse(playerTurnState.CanPlayerMove(TablutPlayer.Attacker));
        }

        [Test]
        public void DefenderCanMoveOnSecondTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure defender can move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove(TablutPlayer.Defender));
        }

        [Test]
        public void AttackerCanMoveOnThirdTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Advance again
            playerTurnState.GoToNextPlayer();

            // Check to make sure attacker can move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove(TablutPlayer.Attacker));
        }

        [Test]
        public void DefenderCantMoveOnThirdTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Advance again
            playerTurnState.GoToNextPlayer();

            // Check to make sure defender can't move right now
            Assert.IsFalse(playerTurnState.CanPlayerMove(TablutPlayer.Defender));
        }
    }
}
