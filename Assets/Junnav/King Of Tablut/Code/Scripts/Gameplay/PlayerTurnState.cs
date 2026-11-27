namespace Junnav.KingOfTablut.Gameplay
{
    public class PlayerTurnState
    {
        private TablutPlayer _currentPlayerTurn = TablutPlayer.Attacker;

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
