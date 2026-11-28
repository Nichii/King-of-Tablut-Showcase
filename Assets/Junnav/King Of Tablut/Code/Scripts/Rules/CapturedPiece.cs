namespace Junnav.KingOfTablut.Gameplay
{
    public readonly struct CapturedPiece
    {
        public TablutPieceType PieceType { get; }
        public BoardPosition Position { get; }

        internal CapturedPiece(TablutPieceType pieceType, BoardPosition position)
        {
            PieceType = pieceType;
            Position = position;
        }
    }
}
