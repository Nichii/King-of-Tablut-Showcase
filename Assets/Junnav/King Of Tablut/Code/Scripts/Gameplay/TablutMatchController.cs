using Junnav.KingOfTablut.Gameplay;
using TMPro;
using UnityEngine;

namespace Junnav.KingOfTablut
{
    public class TablutMatchController : MonoBehaviour
    {
        [SerializeField] private TablutBoardView boardView;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text moveText;

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
            UpdateTurnText();
            moveText.text = "Select a piece to see its moves.";
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
                moveText.text = "Select a piece to see its moves.";
                return;
            }

            if (gameMode.CanSelectPiece(position))
            {
                selectedSquare = position;
                System.Collections.Generic.IReadOnlyList<Vector2Int> destinations = gameMode.GetLegalDestinations(position);
                boardView.ShowSelection(position, destinations);
                moveText.text = destinations.Count == 0 ? "This piece has no legal moves." : "Select a marked square.";
                return;
            }

            if (!selectedSquare.HasValue)
            {
                moveText.text = "Select one of your pieces first.";
                return;
            }

            if (!gameMode.MovePiece(selectedSquare.Value, position))
            {
                moveText.text = GetInvalidMoveText(gameMode.LastInvalidMoveMessage);
                return;
            }

            selectedSquare = null;
            boardView.ClearSelection();
            boardView.ShowPieces(gameMode);
            UpdateTurnText();
            moveText.text = gameMode.LastMoveMessage switch
            {
                MoveMessage.CapturedDefenderKing => "The attackers captured the king.",
                MoveMessage.CapturedDefenderPawn => "A defender was captured.",
                MoveMessage.CapturedAttackerPawn => "An attacker was captured.",
                _ => gameMode.GameState == GameState.WhiteWon ? "The king reached the edge." : "Select a piece to see its moves."
            };
        }

        private void UpdateTurnText()
        {
            turnText.text = gameMode.GameState switch
            {
                GameState.BlackWon => "Attackers win",
                GameState.WhiteWon => "Defenders win",
                _ => gameMode.CurrentPlayer == TablutPlayer.Attacker ? "Attackers to move" : "Defenders to move"
            };
        }

        private static string GetInvalidMoveText(InvalidMoveMessage message)
        {
            return message switch
            {
                InvalidMoveMessage.OnlyOrthogonalMovementAllowed => "Move along a row or column.",
                InvalidMoveMessage.OccupiedSpace => "That square is occupied.",
                InvalidMoveMessage.CannotJumpOverPieces => "Another piece blocks the path.",
                InvalidMoveMessage.CannotMoveIntoCastleSquare => "The throne blocks this move.",
                _ => "Choose a marked square."
            };
        }
    }
}
