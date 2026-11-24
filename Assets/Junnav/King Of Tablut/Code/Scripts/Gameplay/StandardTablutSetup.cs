namespace Junnav.KingOfTablut.Gameplay
{
    public static class StandardTablutSetup
    {
        public static void SetupStandardPositions(TablutBoard tablutBoard)
        {
            // Setup board positions
            TablutPieceType[,] customBoardPositions = new TablutPieceType[9, 9];
            
            // Defender King
            customBoardPositions[4, 4] = TablutPieceType.WhiteKing;

            // Defender Pawns
            customBoardPositions[4, 2] = TablutPieceType.WhitePawn;
            customBoardPositions[4, 3] = TablutPieceType.WhitePawn;
            customBoardPositions[2, 4] = TablutPieceType.WhitePawn;
            customBoardPositions[3, 4] = TablutPieceType.WhitePawn;
            customBoardPositions[5, 4] = TablutPieceType.WhitePawn;
            customBoardPositions[6, 4] = TablutPieceType.WhitePawn;
            customBoardPositions[4, 5] = TablutPieceType.WhitePawn;
            customBoardPositions[4, 6] = TablutPieceType.WhitePawn;

            // Attacker Pawns
            customBoardPositions[0, 3] = TablutPieceType.BlackPawn;
            customBoardPositions[0, 4] = TablutPieceType.BlackPawn;
            customBoardPositions[0, 5] = TablutPieceType.BlackPawn;
            customBoardPositions[1, 4] = TablutPieceType.BlackPawn;
            customBoardPositions[3, 0] = TablutPieceType.BlackPawn;
            customBoardPositions[4, 0] = TablutPieceType.BlackPawn;
            customBoardPositions[5, 0] = TablutPieceType.BlackPawn;
            customBoardPositions[4, 1] = TablutPieceType.BlackPawn;
            customBoardPositions[7, 4] = TablutPieceType.BlackPawn;
            customBoardPositions[8, 3] = TablutPieceType.BlackPawn;
            customBoardPositions[8, 4] = TablutPieceType.BlackPawn;
            customBoardPositions[8, 5] = TablutPieceType.BlackPawn;
            customBoardPositions[4, 7] = TablutPieceType.BlackPawn;
            customBoardPositions[3, 8] = TablutPieceType.BlackPawn;
            customBoardPositions[4, 8] = TablutPieceType.BlackPawn;
            customBoardPositions[5, 8] = TablutPieceType.BlackPawn;

            // Set the custom positions to the board
            tablutBoard.SetBoardPositions(customBoardPositions);
        }
    }
}
