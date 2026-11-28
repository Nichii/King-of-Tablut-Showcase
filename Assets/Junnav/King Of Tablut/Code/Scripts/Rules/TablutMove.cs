namespace Junnav.KingOfTablut.Gameplay
{
    /// <summary>
    /// Identifies the source and destination of a move.
    /// </summary>
    public readonly struct TablutMove
    {
        public BoardPosition From { get; }
        public BoardPosition To { get; }

        public TablutMove(BoardPosition from, BoardPosition to)
        {
            From = from;
            To = to;
        }
    }
}
