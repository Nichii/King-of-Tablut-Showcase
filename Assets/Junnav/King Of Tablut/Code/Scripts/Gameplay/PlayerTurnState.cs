namespace Junnav.KingOfTablut.Gameplay
{
    public class PlayerTurnState
    {
        private const int MaxPlayerCounts = 2;

        private int _currentPlayerTurn = 0;

        public bool CanPlayerMove(int player)
        {
            return _currentPlayerTurn == player;
        }

        public int GetCurrentPlayerTurn()
        {
            return _currentPlayerTurn;
        }

        public void GoToNextPlayer()
        {
            _currentPlayerTurn++;

            if (_currentPlayerTurn > MaxPlayerCounts - 1)
            {
                _currentPlayerTurn = 0;
            }
        }
    }
}
