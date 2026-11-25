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
            Vector2Int from = new(0, 3);
            Vector2Int to = new(0, 2);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, from);
            ClickSquare(boardView, piecePositions, new Vector2Int(1, 2));
            Assert.AreEqual(1, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(0, GetPiecesAt(piecePositions, to).Length);

            ClickSquare(boardView, piecePositions, to);
            yield return null;

            Assert.AreEqual(0, GetPiecesAt(piecePositions, from).Length);
            Assert.AreEqual(1, GetPiecesAt(piecePositions, to).Length);

            Button restartButton = Object.FindFirstObjectByType<Button>();
            Assert.IsNotNull(restartButton);
            Canvas.ForceUpdateCanvases();
            Vector2 restartPoint = RectTransformUtility.WorldToScreenPoint(null, restartButton.transform.position);
            PointerEventData pointer = new(EventSystem.current) { position = restartPoint };
            System.Collections.Generic.List<RaycastResult> hits = new();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.IsNotEmpty(hits);
            Assert.AreSame(restartButton.gameObject, hits[0].gameObject);

            pointer.pointerCurrentRaycast = hits[0];
            ExecuteEvents.Execute(restartButton.gameObject, pointer, ExecuteEvents.pointerClickHandler);
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

        private static void ClickSquare(TablutBoardView boardView, SetupPiecePositions piecePositions, Vector2Int square)
        {
            Vector3 worldPosition = piecePositions.transform.TransformPoint(piecePositions.GetLocalPosition(square));
            PointerEventData click = new(EventSystem.current)
            {
                pointerCurrentRaycast = new RaycastResult { worldPosition = worldPosition }
            };
            ExecuteEvents.Execute(boardView.gameObject, click, ExecuteEvents.pointerClickHandler);
        }
    }
}
