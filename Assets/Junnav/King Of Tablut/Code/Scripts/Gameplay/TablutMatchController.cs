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
        [SerializeField] private TablutScenario scenario;
        [SerializeField] private bool playAgainstComputer;
        [SerializeField] private int opponentSeed;

        private TablutGameMode gameMode;
        private RandomTablutOpponent opponent;
        private BoardPosition? selectedSquare;

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
            gameMode = CreateMatch();
            opponent = playAgainstComputer ? new RandomTablutOpponent(opponentSeed) : null;
            selectedSquare = null;
            boardView.ClearSelection();
            boardView.ShowPieces(gameMode);
            UpdateTurnText(gameMode.GameState);
            moveText.text = "Select a piece to see its moves.";
            PlayComputerTurn();
        }

        private TablutGameMode CreateMatch()
        {
            if (scenario != null)
            {
                if (scenario.TryCreateMatch(out TablutGameMode match, out string error))
                {
                    return match;
                }

                Debug.LogWarning($"Scenario '{scenario.ScenarioName}' is invalid. {error} Starting a standard match.", scenario);
            }

            return new TablutGameMode();
        }

        private void SelectSquare(BoardPosition position)
        {
            if (gameMode.GameState != GameState.Playing || IsComputerTurn())
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
                System.Collections.Generic.IReadOnlyList<BoardPosition> destinations = gameMode.GetLegalDestinations(position);
                boardView.ShowSelection(position, destinations);
                moveText.text = destinations.Count == 0 ? "This piece has no legal moves." : "Select a marked square.";
                return;
            }

            if (!selectedSquare.HasValue)
            {
                moveText.text = "Select one of your pieces first.";
                return;
            }

            if (TryMovePiece(selectedSquare.Value, position))
            {
                PlayComputerTurn();
            }
        }

        private bool IsComputerTurn()
        {
            return opponent != null && gameMode.CurrentPlayer == TablutPlayer.Defender;
        }

        private void PlayComputerTurn()
        {
            if (!IsComputerTurn() || gameMode.GameState != GameState.Playing)
            {
                return;
            }

            if (opponent.TryChooseMove(gameMode, out TablutMove move))
            {
                TryMovePiece(move.From, move.To);
            }
            else
            {
                moveText.text = "The computer has no legal moves.";
            }
        }

        private bool TryMovePiece(BoardPosition from, BoardPosition to)
        {
            MoveResult result = gameMode.MovePieceWithResult(from, to);
            if (!result.Succeeded)
            {
                moveText.text = GetInvalidMoveText(result.RejectionReason);
                return false;
            }

            selectedSquare = null;
            boardView.ClearSelection();
            boardView.ShowPieces(gameMode);
            UpdateTurnText(result.GameState);

            int captureCount = result.Captures.Count;
            moveText.text = result.Message switch
            {
                MoveMessage.CapturedDefenderKing => "The attackers captured the king.",
                MoveMessage.CapturedDefenderPawn when captureCount > 1 => $"{captureCount} defenders were captured.",
                MoveMessage.CapturedDefenderPawn => "A defender was captured.",
                MoveMessage.CapturedAttackerPawn when captureCount > 1 => $"{captureCount} attackers were captured.",
                MoveMessage.CapturedAttackerPawn => "An attacker was captured.",
                _ => result.GameState == GameState.WhiteWon ? "The king reached the edge." : "Select a piece to see its moves."
            };

            return true;
        }

        private void UpdateTurnText(GameState gameState)
        {
            turnText.text = gameState switch
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
