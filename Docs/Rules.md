# Tablut rules

## Setup and movement

The board is 9 × 9. Black has 16 attackers and moves first, then the sides alternate. White has eight defender pawns and a king, which starts on the throne at the centre.

On your turn, slide a piece along a row or column to an empty square. Pieces cannot jump over each other or cross the throne. Once the king leaves the throne, it cannot return. The throne is closed to all other pieces.

## Captures

Your move captures an enemy pawn by trapping it between the moved piece and another piece on your side or the throne. The throne can back a pawn capture even while the king occupies it. A single move can capture pawns in more than one direction.

- **King on the throne.** Surround the king with four attackers.
- **King on a square sharing an edge with the throne.** Place attackers on the other three sides.
- **King elsewhere.** Trap the king between two attackers as you would a pawn.

## Winning and restarting

The attackers win by capturing the king. The defenders win when the king reaches any square on the edge of the board. A captured king stays visible in the final position. Click **Restart** to start a new match.

The current game has no automatic draw for repeated positions and no automatic result when the side to move has no legal move. For example, this sequence returns to the starting board with the attackers to move. The coordinates are zero-based `(x, y)`.

1. Attacker moves from `(0, 3)` to `(0, 2)`.
2. Defender moves from `(4, 2)` to `(5, 2)`.
3. Attacker moves from `(0, 2)` to `(0, 3)`.
4. Defender moves from `(5, 2)` to `(4, 2)`.

[Gameplay rules code](../Assets/Junnav/King%20Of%20Tablut/Code/Scripts/Gameplay/TablutGameMode.cs) · [Rules tests](../Assets/Junnav/King%20Of%20Tablut/Code/Tests/Play%20Mode%20Tests/Gameplay/TestTablutGameMode.cs)
