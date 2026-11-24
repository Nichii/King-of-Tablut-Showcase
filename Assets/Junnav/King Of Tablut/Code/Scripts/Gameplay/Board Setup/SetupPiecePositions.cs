using Junnav.KingOfTablut.Gameplay;
using UnityEngine;

namespace Junnav.KingOfTablut
{
    public class SetupPiecePositions : MonoBehaviour
    {
        [Header("Position Settings")]
        [SerializeField]
        private float positionSpacing = 0.175f;

        private void Start()
        {
            // Spawn empty parent objects for Tablut pieces to be parented to, starting from top left
            for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
            {
                for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
                {
                    // We'll store the name of the position on the game object itself
                    GameObject positionParentObj = new($"Position ({x}, {y})");

                    // Parent the object to current game object
                    positionParentObj.transform.SetParent(transform, false);

                    // Calculate the difference from the center square and scaled by the position spacing
                    float parentPosX = (x - TablutBoard.CenterHorizontalSquare) * positionSpacing;
                    float parentPosY = (y - TablutBoard.CenterVerticalSquare) * positionSpacing;

                    // Our Y is the representation of Y on chess board, not the height in 3D world
                    positionParentObj.transform.localPosition = new Vector3(parentPosX, 0.0f, parentPosY);
                }
            }
        }
    }
}
