using System;
using System.Text;
using UnityEngine;

namespace Junnav.KingOfTablut.Gameplay
{
    public class TablutBoard
    {
        public const int TotalHorizontalSquares = 9;
        public const int TotalVerticalSquares = 9;
        public const int CenterHorizontalSquare = TotalHorizontalSquares / 2;
        public const int CenterVerticalSquare = TotalVerticalSquares / 2;

        public static readonly Vector2Int CastleSquare = new(CenterHorizontalSquare, CenterVerticalSquare);

        private TablutPieceType[,] boardPositions = new TablutPieceType[TotalHorizontalSquares, TotalVerticalSquares];

        public bool SetPiece(TablutPieceType pieceType, Vector2Int position)
        {
            if (!IsValidPosition(position))
            {
                return false;
            }

            boardPositions[position.x, position.y] = pieceType;

            return true;
        }

        public bool SetBoardPositions(TablutPieceType[,] customBoardPositions)
        {
            if (customBoardPositions == null || customBoardPositions.GetLength(0) != TotalHorizontalSquares
                || customBoardPositions.GetLength(1) != TotalVerticalSquares)
            {
                return false;
            }

            boardPositions = customBoardPositions;

            return true;
        }

        public TablutPieceType[,] GetBoardPositions()
        {
            return boardPositions;
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.Append("    A    B    C    D    E    F    G    H    I");
            builder.Append(Environment.NewLine);

            int boardWidth = boardPositions.GetLength(0);
            int boardHeight = boardPositions.GetLength(1);

            for (int y = 0; y < boardHeight; y++)
            {
                builder.Append(boardHeight - y);
                builder.Append("  ");

                for (int x = 0; x < boardWidth; x++)
                {
                    switch (boardPositions[x, y])
                    {
                        case TablutPieceType.Empty:
                            if (x == 4 && y == 4)
                            {
                                builder.Append("\u2612");
                            }
                            else
                            {
                                builder.Append(" \u25A1 ");
                            }
                            break;

                        case TablutPieceType.WhiteKing:
                            builder.Append("\u2654");
                            break;

                        case TablutPieceType.WhitePawn:
                            builder.Append("\u2659");
                            break;

                        case TablutPieceType.BlackPawn:
                            builder.Append("\u265F");
                            break;
                    }

                    if (x < boardPositions.GetLength(0) - 1)
                        builder.Append(" ");
                }

                if (y < boardPositions.GetLength(1) - 1)
                    builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < boardPositions.GetLength(0)
                && position.y >= 0 && position.y < boardPositions.GetLength(1);
        }
    }
}
