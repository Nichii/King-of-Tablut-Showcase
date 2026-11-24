using System;
using System.Collections;
using System.Collections.Generic;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            Assert.IsTrue(playerTurnState.GetCurrentPlayerTurn() == (int)TablutPlayer.Attacker);
        }

        [Test]
        public void DefendingSidesTurnNext()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure it's currently defender's turn
            Assert.IsTrue(playerTurnState.GetCurrentPlayerTurn() == (int)TablutPlayer.Defender);
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
            Assert.IsTrue(playerTurnState.GetCurrentPlayerTurn() == (int)TablutPlayer.Attacker);
        }

        [Test]
        public void AttackerCanMoveOnFirstTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Check to make sure attacker can move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Attacker) == true);
        }

        [Test]
        public void DefenderCantMoveIfAttackersTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Check to make sure defender can't move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Defender) == false);
        }

        [Test]
        public void AttackerCannotMoveOnSecondTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure attacker can't move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Attacker) == false);
        }

        [Test]
        public void DefenderCanMoveOnSecondTurn()
        {
            // Setup
            PlayerTurnState playerTurnState = new();

            // Advance to the next player
            playerTurnState.GoToNextPlayer();

            // Check to make sure defender can move right now
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Defender) == true);
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
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Attacker) == true);
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
            Assert.IsTrue(playerTurnState.CanPlayerMove((int)TablutPlayer.Defender) == false);
        }
    }
}
