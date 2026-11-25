using System;
using System.Collections.Generic;
using Junnav.KingOfTablut.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Junnav.KingOfTablut
{
    public class TablutBoardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SetupPiecePositions piecePositions;
        [SerializeField] private GameObject blackPawnPrefab;
        [SerializeField] private GameObject whitePawnPrefab;
        [SerializeField] private GameObject whiteKingPrefab;
        [SerializeField] private Material markerMaterial;

        private readonly GameObject[,] pieces = new GameObject[TablutBoard.TotalHorizontalSquares, TablutBoard.TotalVerticalSquares];
        private readonly TablutPieceType[,] displayedPieces = new TablutPieceType[TablutBoard.TotalHorizontalSquares, TablutBoard.TotalVerticalSquares];
        private readonly List<LineRenderer> markers = new();

        public event Action<Vector2Int> SquareClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Vector2Int position = piecePositions.GetBoardPosition(eventData.pointerCurrentRaycast.worldPosition);
            if (position.x >= 0 && position.x < TablutBoard.TotalHorizontalSquares
                && position.y >= 0 && position.y < TablutBoard.TotalVerticalSquares)
            {
                SquareClicked?.Invoke(position);
            }
        }

        public void ShowPieces(TablutGameMode gameMode)
        {
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    Vector2Int position = new(x, y);
                    TablutPieceType pieceType = gameMode.GetPiece(position);
                    if (displayedPieces[x, y] == pieceType)
                    {
                        continue;
                    }

                    if (pieces[x, y] != null)
                    {
                        pieces[x, y].SetActive(false);
                        Destroy(pieces[x, y]);
                        pieces[x, y] = null;
                    }

                    displayedPieces[x, y] = pieceType;
                    GameObject prefab = GetPrefab(pieceType);
                    if (prefab != null)
                    {
                        pieces[x, y] = Instantiate(prefab, piecePositions.transform);
                        pieces[x, y].transform.localPosition = piecePositions.GetLocalPosition(position);
                    }
                }
            }
        }

        public void ShowSelection(Vector2Int selectedSquare, IReadOnlyList<Vector2Int> destinations)
        {
            ClearSelection();
            DrawMarker(0, selectedSquare, 0.082f, new Color(1.0f, 0.78f, 0.32f));
            for (int i = 0; i < destinations.Count; i++)
            {
                DrawMarker(i + 1, destinations[i], 0.035f, new Color(0.38f, 0.87f, 0.77f));
            }
        }

        public void ClearSelection()
        {
            foreach (LineRenderer marker in markers)
            {
                marker.gameObject.SetActive(false);
            }
        }

        private void DrawMarker(int index, Vector2Int square, float radius, Color color)
        {
            if (index == markers.Count)
            {
                GameObject markerObject = new("Move Marker");
                markerObject.transform.SetParent(piecePositions.transform, false);
                LineRenderer newMarker = markerObject.AddComponent<LineRenderer>();
                newMarker.useWorldSpace = false;
                newMarker.loop = true;
                newMarker.positionCount = 20;
                newMarker.widthMultiplier = 0.012f;
                newMarker.sharedMaterial = markerMaterial;
                markers.Add(newMarker);
            }

            LineRenderer marker = markers[index];
            marker.startColor = color;
            marker.endColor = color;
            Vector3 center = piecePositions.GetLocalPosition(square) + Vector3.up * 0.02f;
            for (int i = 0; i < marker.positionCount; i++)
            {
                float angle = 2.0f * Mathf.PI * i / marker.positionCount;
                marker.SetPosition(i, center + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * radius);
            }

            marker.gameObject.SetActive(true);
        }

        private GameObject GetPrefab(TablutPieceType pieceType)
        {
            switch (pieceType)
            {
                case TablutPieceType.BlackPawn:
                    return blackPawnPrefab;
                case TablutPieceType.WhitePawn:
                    return whitePawnPrefab;
                case TablutPieceType.WhiteKing:
                    return whiteKingPrefab;
                default:
                    return null;
            }
        }
    }
}
