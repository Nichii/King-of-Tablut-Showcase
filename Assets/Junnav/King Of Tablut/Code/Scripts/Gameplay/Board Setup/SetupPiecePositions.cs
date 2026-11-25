using Junnav.KingOfTablut.Gameplay;
using UnityEngine;

namespace Junnav.KingOfTablut
{
    public class SetupPiecePositions : MonoBehaviour
    {
        [Header("Position Settings")]
        [SerializeField]
        private float positionSpacing = 0.175f;

        public Vector3 GetLocalPosition(Vector2Int position)
        {
            return new Vector3((position.x - TablutBoard.CenterHorizontalSquare) * positionSpacing, 0.0f,
                (position.y - TablutBoard.CenterVerticalSquare) * positionSpacing);
        }

        public Vector2Int GetBoardPosition(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

            // Board Y follows world Z. Half-square boundaries belong to the next square.
            return new Vector2Int(Mathf.FloorToInt(localPosition.x / positionSpacing + 0.5f) + TablutBoard.CenterHorizontalSquare,
                Mathf.FloorToInt(localPosition.z / positionSpacing + 0.5f) + TablutBoard.CenterVerticalSquare);
        }
    }
}
