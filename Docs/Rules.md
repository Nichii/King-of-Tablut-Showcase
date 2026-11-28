# Tablut rules

These are the rules used by King of Tablut. [Cyningstan’s Tablut guide](http://tafl.cyningstan.com/page/170/tablut) was a reference for the project.

## Setup and movement

The board is 9 × 9. Black has 16 attackers and moves first, then the sides alternate. White has eight defender pawns and a king, which starts on the throne at the centre.

On your turn, slide a piece along a row or column to an empty square. Pieces cannot jump over each other or cross the throne. Once the king leaves the throne, it cannot return. The throne is closed to all other pieces.

## Captures

Your move captures an enemy pawn by trapping it between the moved piece and another piece on your side or the throne. The throne can back a pawn capture even while the king occupies it. The king can help capture attackers. A single move can capture pawns in more than one direction. Moving a piece between enemies does not cause it to be captured.

- **King on the throne.** Surround the king with four attackers.
- **King on a square sharing an edge with the throne.** Place attackers on the other three sides.
- **King elsewhere.** Trap the king between two attackers as you would a pawn.

## Winning and restarting

The attackers win by capturing the king. The defenders win when the king reaches any square on the edge of the board. A captured king stays visible in the final position. Choose **Play Again** from the result screen to start a new match in the same mode.

The current game has no automatic draw for repeated positions and no automatic result when the side to move has no legal move. For example, this sequence returns to the starting board with the attackers to move. The coordinates are zero-based `(x, y)`.

1. Attacker moves from `(0, 3)` to `(0, 2)`.
2. Defender moves from `(4, 2)` to `(5, 2)`.
3. Attacker moves from `(0, 2)` to `(0, 3)`.
4. Defender moves from `(5, 2)` to `(4, 2)`.

## Tabletop variations

Damian Walker's [2015 print-and-play rules](https://www.scribd.com/document/849717840/Tablut-Print-and-Play) allow the king to land on the throne and describe trapping a pawn against the empty throne. His [earlier leaflet](https://www.scribd.com/document/512304044/01-Tablut) allows pieces to cross the empty throne and uses *raichi* for one clear escape route and *tuichu* for two routes the attackers cannot both stop. The earlier leaflet uses an unarmed king and describes taking a defender between an attacker and a king who is surrounded on the other three sides. It also describes a draw by agreement, for repeated play, or when a player cannot move.

King of Tablut blocks movement through the throne and allows it to back captures while occupied. Escape requires moving the king onto the edge. The game has no escape-call mechanic or automatic draw.

## Replay saves

`TablutReplaySave.Write` stores the starting board and side to move with the move history. Version 1 begins with `TABLUT-REPLAY 1`, then `A` or `D` for the starting side. The next nine lines are board rows in increasing `y` order, with columns in increasing `x` order. The symbols are `.` for an empty square, `K` for the king, `D` for a defender pawn, and `A` for an attacker pawn. Each move occupies one line in `fromX,fromY,toX,toY` order.

`TryLoad` rebuilds a fresh match using the saved layout. A rejected move is reported by its zero-based index and reason.

[Gameplay rules code](../Assets/Junnav/King%20Of%20Tablut/Code/Scripts/Rules/TablutGameMode.cs) · [Rules tests](../Assets/Junnav/King%20Of%20Tablut/Code/Tests/Edit%20Mode%20Tests/Gameplay/TestTablutGameMode.cs)
