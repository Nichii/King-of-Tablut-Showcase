using Junnav.KingOfTablut.Gameplay;
using UnityEngine;

namespace Junnav.KingOfTablut
{
    public class TablutMatchController : MonoBehaviour
    {
        [SerializeField] private TablutBoardView boardView;

        private TablutGameMode gameMode;
        private Vector2Int? selectedSquare;

        private void OnEnable()
        {
            boardView.SquareClicked += SelectSquare;
        }

        private void OnDisable()
        {
            boardView.SquareClicked -= SelectSquare;
        }

        private void Start()
        {
            RestartMatch();
        }

        public void RestartMatch()
        {
            gameMode = new();
            selectedSquare = null;
            boardView.ClearSelection();
            boardView.ShowPieces(gameMode);
        }

        private void SelectSquare(Vector2Int position)
        {
            if (gameMode.GameState != GameState.Playing)
            {
                return;
            }

            if (position == selectedSquare)
            {
                selectedSquare = null;
                boardView.ClearSelection();
                return;
            }

            if (gameMode.CanSelectPiece(position))
            {
                selectedSquare = position;
                var destinations = gameMode.GetLegalDestinations(position);
                boardView.ShowSelection(position, destinations);
                return;
            }

            if (!selectedSquare.HasValue)
            {
                return;
            }

            if (!gameMode.MovePiece(selectedSquare.Value, position))
            {
                return;
            }

            selectedSquare = null;
            boardView.ClearSelection();
            boardView.ShowPieces(gameMode);
        }

    }
}
