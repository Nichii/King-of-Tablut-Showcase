using System.Collections;
using Junnav.KingOfTablut.Gameplay;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Junnav.KingOfTablut.Tests.PlayMode
{
    public class TestTablutGameplayScene
    {
        [UnityTest]
        public IEnumerator CanPlayAnOpeningMove()
        {
            yield return SceneManager.LoadSceneAsync("TablutGameplayScene");
            yield return null;

            TablutBoardView boardView = Object.FindFirstObjectByType<TablutBoardView>();
            SetupPiecePositions piecePositions = boardView.GetComponent<SetupPiecePositions>();
            Camera camera = Camera.main;
            Assert.IsNotNull(camera);

            BoardPosition from = new(0, 3);
            BoardPosition to = new(0, 2);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, camera, from);
            ClickSquare(boardView, piecePositions, camera, new BoardPosition(1, 2));
            AssertFeedback("Attackers to move", "Move along a row or column.");
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, camera, to);
            yield return null;

            Assert.AreEqual(0, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, to).Length);
            AssertFeedback("Defenders to move", "Select a piece to see its moves.");

            ClickRestart();
            yield return null;

            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);
            AssertFeedback("Attackers to move", "Select a piece to see its moves.");
        }

        [UnityTest]
        public IEnumerator ShowsMultipleCapturesAndAnAttackerWin()
        {
            yield return SceneManager.LoadSceneAsync("TablutGameplayScene");
            yield return null;

            TablutBoardView boardView = Object.FindFirstObjectByType<TablutBoardView>();
            SetupPiecePositions piecePositions = boardView.GetComponent<SetupPiecePositions>();
            Camera camera = Camera.main;

            // Set up captures in two directions from (3, 3).
            MovePiece(boardView, piecePositions, camera, new BoardPosition(0, 5), new BoardPosition(3, 5));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 6), new BoardPosition(5, 6));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(8, 3), new BoardPosition(5, 3));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(5, 6), new BoardPosition(4, 6));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(3, 0), new BoardPosition(3, 3));

            Assert.AreEqual(0, GetPiecesAt(piecePositions, new BoardPosition(3, 4)).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, new BoardPosition(4, 3)).Length);
            AssertFeedback("Defenders to move", "2 defenders were captured.");

            // Clear the remaining squares next to the throne and surround the king.
            MovePiece(boardView, piecePositions, camera, new BoardPosition(5, 4), new BoardPosition(5, 6));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(5, 3), new BoardPosition(5, 4));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 5), new BoardPosition(6, 5));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(3, 5), new BoardPosition(4, 5));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(6, 5), new BoardPosition(6, 6));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(3, 3), new BoardPosition(3, 4));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 2), new BoardPosition(5, 2));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 1), new BoardPosition(4, 3));

            AssertFeedback("Attackers win", "The attackers captured the king.");
            Assert.AreEqual(1, GetPiecesAt(piecePositions, TablutBoard.CastleSquare).Length);
        }

        [UnityTest]
        public IEnumerator ShowsASingleCaptureAndADefenderWin()
        {
            yield return SceneManager.LoadSceneAsync("TablutGameplayScene");
            yield return null;

            TablutBoardView boardView = Object.FindFirstObjectByType<TablutBoardView>();
            SetupPiecePositions piecePositions = boardView.GetComponent<SetupPiecePositions>();
            Camera camera = Camera.main;

            MovePiece(boardView, piecePositions, camera, new BoardPosition(0, 3), new BoardPosition(0, 2));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 3), new BoardPosition(6, 3));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(3, 0), new BoardPosition(3, 3));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 2), new BoardPosition(3, 2));

            AssertFeedback("Attackers to move", "An attacker was captured.");

            MovePiece(boardView, piecePositions, camera, new BoardPosition(8, 3), new BoardPosition(8, 2));
            MovePiece(boardView, piecePositions, camera, TablutBoard.CastleSquare, new BoardPosition(4, 2));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(8, 2), new BoardPosition(8, 3));
            MovePiece(boardView, piecePositions, camera, new BoardPosition(4, 2), new BoardPosition(8, 2));

            AssertFeedback("Defenders win", "The king reached the edge.");
        }

#if UNITY_EDITOR
        [UnityTest]
        public IEnumerator RestartsTheAssignedScenarioWithoutChangingTheAsset()
        {
            yield return SceneManager.LoadSceneAsync("TablutGameplayScene");
            yield return null;

            TablutScenario scenario = AssetDatabase.LoadAssetAtPath<TablutScenario>(
                "Assets/Junnav/King Of Tablut/Level/Scenarios/King Escape.asset");
            Assert.IsNotNull(scenario);
            string assetBeforeMatch = EditorJsonUtility.ToJson(scenario);
            Assert.AreEqual("King Escape", scenario.ScenarioName);
            Assert.IsNotEmpty(scenario.Description);

            TablutBoardView boardView = Object.FindFirstObjectByType<TablutBoardView>();
            SetupPiecePositions piecePositions = boardView.GetComponent<SetupPiecePositions>();
            Camera camera = Camera.main;
            BoardPosition from = new(4, 2);
            BoardPosition edge = new(4, 0);

            AssignScenario(scenario);
            for (int i = 0; i < 2; i++)
            {
                ClickRestart();
                AssertFeedback("Defenders to move", "Select a piece to see its moves.");
                Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
                Assert.AreEqual(0, GetPiecesAt(piecePositions, edge).Length);

                MovePiece(boardView, piecePositions, camera, from, edge);
                AssertFeedback("Defenders win", "The king reached the edge.");
                Assert.AreEqual(1, GetPiecesAt(piecePositions, edge).Length);
            }

            Assert.AreEqual(assetBeforeMatch, EditorJsonUtility.ToJson(scenario));
        }

        [UnityTest]
        public IEnumerator InvalidScenarioRestartsTheStandardMatch()
        {
            yield return SceneManager.LoadSceneAsync("TablutGameplayScene");
            yield return null;

            TablutBoardView boardView = Object.FindFirstObjectByType<TablutBoardView>();
            SetupPiecePositions piecePositions = boardView.GetComponent<SetupPiecePositions>();
            BoardPosition from = new(0, 3);
            BoardPosition to = new(0, 2);
            MovePiece(boardView, piecePositions, Camera.main, from, to);
            AssertFeedback("Defenders to move", "Select a piece to see its moves.");

            TablutScenario scenario = ScriptableObject.CreateInstance<TablutScenario>();
            scenario.name = "Invalid Scenario";
            try
            {
                AssignScenario(scenario);
                LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(
                    "Invalid Scenario.*exactly one king"));
                ClickRestart();

                AssertFeedback("Attackers to move", "Select a piece to see its moves.");
                Assert.AreEqual(1, GetPiecesAt(piecePositions, TablutBoard.CastleSquare).Length);
                Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
                Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);
            }
            finally
            {
                Object.Destroy(scenario);
            }
        }

        private static void AssignScenario(TablutScenario scenario)
        {
            SerializedObject controller = new(Object.FindFirstObjectByType<TablutMatchController>());
            controller.FindProperty("scenario").objectReferenceValue = scenario;
            controller.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        private static void ClickRestart()
        {
            Button restartButton = Object.FindFirstObjectByType<Button>();
            Assert.IsNotNull(restartButton);
            Canvas.ForceUpdateCanvases();
            Vector2 restartPoint = RectTransformUtility.WorldToScreenPoint(null, restartButton.transform.position);
            ClickAt(restartPoint, restartButton.gameObject);
        }

        private static void AssertFeedback(string turn, string move)
        {
            TMP_Text turnText = GameObject.Find("Attackers to move").GetComponent<TMP_Text>();
            TMP_Text moveText = GameObject.Find("Select a piece to see its moves.").GetComponent<TMP_Text>();
            Assert.AreEqual(turn, turnText.text);
            Assert.AreEqual(move, moveText.text);
        }

        private static void MovePiece(TablutBoardView boardView, SetupPiecePositions piecePositions, Camera camera,
            BoardPosition from, BoardPosition to)
        {
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length, $"No piece to move at {from}.");
            ClickSquare(boardView, piecePositions, camera, from);
            ClickSquare(boardView, piecePositions, camera, to);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, from).Length, $"Piece did not move from {from} to {to}.");
        }

        private static GameObject[] GetPiecesAt(SetupPiecePositions piecePositions, BoardPosition square)
        {
            System.Collections.Generic.List<GameObject> pieces = new();
            Vector3 localPosition = piecePositions.GetLocalPosition(square);
            foreach (Transform child in piecePositions.transform)
            {
                if (child.gameObject.activeSelf && child.localPosition == localPosition
                    && child.GetComponent<LineRenderer>() == null)
                {
                    pieces.Add(child.gameObject);
                }
            }

            return pieces.ToArray();
        }

        private static void ClickSquare(TablutBoardView boardView, SetupPiecePositions piecePositions, Camera camera, BoardPosition square)
        {
            Vector3 worldPosition = piecePositions.transform.TransformPoint(piecePositions.GetLocalPosition(square));
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
            Assert.Greater(screenPosition.z, 0f, $"Square {square} is behind the camera.");

            ClickAt(screenPosition, boardView.gameObject);
        }

        private static void ClickAt(Vector2 screenPosition, GameObject expectedTarget)
        {
            EventSystem eventSystem = EventSystem.current;
            PointerEventData pointer = new(eventSystem) { position = screenPosition };
            System.Collections.Generic.List<RaycastResult> hits = new();

            // Use the first hit so an overlapping UI element can block the board click.
            eventSystem.RaycastAll(pointer, hits);
            Assert.IsNotEmpty(hits, $"No object was hit at {screenPosition}.");
            Assert.AreSame(expectedTarget, hits[0].gameObject);

            pointer.pointerCurrentRaycast = hits[0];
            Assert.IsTrue(ExecuteEvents.Execute(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler));
        }
    }
}
