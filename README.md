# King of Tablut

King of Tablut is a local two-player strategy game built in Unity. The attackers try to capture the king. The defenders try to get the king to any edge of the board.

[Play WebGL Demo](https://nikorasuchan.itch.io/king-of-tablut-demo) · [Download for Windows](https://github.com/Nichii/King-of-Tablut-Showcase/releases/latest/download/King-of-Tablut-Windows.zip) · [Watch gameplay](https://github.com/Nichii/King-of-Tablut-Showcase/releases/latest/download/King-of-Tablut-Gameplay.mp4) · [Read the rules code](Assets/Junnav/King%20Of%20Tablut/Code/Scripts/Gameplay/TablutGameMode.cs)

![A selected attacker and its legal destinations on the Tablut board](Docs/Gameplay.jpg)

## Play

Extract the Windows ZIP and run `King of Tablut.exe`. Click one of your pieces to see where it can move, then click a marked square. Use **Restart** to return to the starting position.

To run from source, open the project in Unity **6000.3.24f1**, open `TablutGameplayScene`, and enter Play Mode.

## Gameplay code and tests

`TablutGameMode` owns the board, turns, and win conditions. The Unity scene reads its board state after a move. Move highlights use the same validation as move execution, so the view only offers squares the rules accept.

One regression came from the throne at `(4,4)`. Moving from `(2,1)` to `(6,1)` was rejected because the path crossed column 4. The [fix checks the full square](https://github.com/Nichii/King-of-Tablut-Showcase/commit/592ccfe) and tests valid moves across both axes, alongside the test that blocks a move through the throne itself.

In Unity, open **Window → General → Test Runner**, select **PlayMode**, and choose **Run All**. The suite covers moves, captures, both victory outcomes, and an opening move and restart through the gameplay scene.

## Project scope

This repository is a playable slice of an independent project. The full commercial project remains under private development. Source and assets here are provided for portfolio review and technical evaluation. All rights reserved, subject to applicable third-party licences.
