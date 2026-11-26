using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

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

            Vector2Int from = new(0, 3);
            Vector2Int to = new(0, 2);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, camera, from);
            ClickSquare(boardView, piecePositions, camera, new Vector2Int(1, 2));
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, camera, to);
            yield return null;

            Assert.AreEqual(0, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, to).Length);

            Button restartButton = Object.FindFirstObjectByType<Button>();
            Assert.IsNotNull(restartButton);
            Canvas.ForceUpdateCanvases();
            Vector2 restartPoint = RectTransformUtility.WorldToScreenPoint(null, restartButton.transform.position);
            ClickAt(restartPoint, restartButton.gameObject);
            yield return null;

            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);
        }

        private static GameObject[] GetPiecesAt(SetupPiecePositions piecePositions, Vector2Int square)
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

        private static void ClickSquare(TablutBoardView boardView, SetupPiecePositions piecePositions, Camera camera, Vector2Int square)
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
