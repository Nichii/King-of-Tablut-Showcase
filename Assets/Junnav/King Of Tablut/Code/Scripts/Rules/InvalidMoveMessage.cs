namespace Junnav.KingOfTablut.Gameplay
{
    public enum InvalidMoveMessage
    {
        None,
        OccupiedSpace,
        CannotMoveEmptySpace,
        WrongPlayerTurn,
        CannotJumpOverPieces,
        OnlyOrthogonalMovementAllowed,
        OutOfBound,
        CannotMoveIntoCastleSquare,
        GameEnded,
    }
}
