using System;

namespace Junnav.KingOfTablut.Gameplay
{
    public class PlayerTurnState
    {
        private TablutPlayer _currentPlayerTurn;

        public PlayerTurnState() : this(TablutPlayer.Attacker)
        {
        }

        internal PlayerTurnState(TablutPlayer startingPlayer)
        {
            if (startingPlayer != TablutPlayer.Attacker && startingPlayer != TablutPlayer.Defender)
            {
                throw new ArgumentOutOfRangeException(nameof(startingPlayer), startingPlayer, "Choose either attackers or defenders.");
            }

            _currentPlayerTurn = startingPlayer;
        }

        public bool CanPlayerMove(TablutPlayer player)
        {
            return _currentPlayerTurn == player;
        }

        public TablutPlayer GetCurrentPlayerTurn()
        {
            return _currentPlayerTurn;
        }

        public void GoToNextPlayer()
        {
            _currentPlayerTurn = _currentPlayerTurn == TablutPlayer.Attacker
                ? TablutPlayer.Defender
                : TablutPlayer.Attacker;
        }
    }
}
