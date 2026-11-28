# King of Tablut

King of Tablut is a local two-player strategy game built in Unity. The attackers try to capture the king. The defenders try to get the king to any edge of the board.

[Play WebGL Demo](https://nikorasuchan.itch.io/king-of-tablut-demo) · [Download for Windows](https://github.com/Nichii/King-of-Tablut-Showcase/releases/latest/download/King-of-Tablut-Windows.zip) · [Watch gameplay](https://github.com/Nichii/King-of-Tablut-Showcase/releases/latest/download/King-of-Tablut-Gameplay.mp4) · [Read the rules code](Assets/Junnav/King%20Of%20Tablut/Code/Scripts/Rules/TablutGameMode.cs)

![A selected attacker and its legal destinations on the Tablut board](Docs/Gameplay.jpg)

## Play

Extract the Windows ZIP and run `King of Tablut.exe`. Click one of your pieces to see where it can move, then click a marked square. Use **Restart** to return to the starting position.

To run from source, open the project in Unity **6000.3.24f1**, open `TablutGameplayScene`, and enter Play Mode.

See the [Tablut rules](Docs/Rules.md) for movement, captures, and how a match ends.

## Build from source

Open the project in Unity **6000.3.24f1**. Install **Web Build Support** for that editor version through Unity Hub if you want a browser build.

Open **File -> Build Profiles**, make the target platform active, and confirm that `TablutGameplayScene` is enabled in the Scene List.

- **Windows.** Select **Windows** and **Intel 64-bit**, then choose **Build** and save to `Builds/Windows`. Run the generated executable with the rest of the build files in the same folder.
- **Web.** Select **Web**, then choose **Build and Run** and save to `Builds/Web`. Unity serves the build locally and opens it in your browser. The first Web build can take several minutes.

To check either build, make an opening move and click **Restart**. The opening position should return with the attackers to move.

## Gameplay code and tests

`TablutGameMode` owns the board, turns, and win conditions. It lives in the `Junnav.KingOfTablut.Rules` assembly, which compiles without UnityEngine references. The Unity scene reads its board state after a move. Move highlights use the same validation as move execution, so the view only offers squares the rules accept.

One regression came from the throne at `(4,4)`. Moving from `(2,1)` to `(6,1)` was rejected because the path crossed column 4. The [fix checks the full square](https://github.com/Nichii/King-of-Tablut-Showcase/commit/592ccfe) and tests valid moves across both axes, alongside the test that blocks a move through the throne itself.

In Unity, open **Window → General → Test Runner**. In **EditMode**, choose **Run All** for the board, turn, and match rules. In **PlayMode**, choose **Run All** for the gameplay scene tests, which check board clicks, move feedback, and restart.

### Run the rules tests without Unity

From the repository root, use the .NET 10 SDK to run the board, turn and match tests. The .NET projects compile the same C# rules and NUnit fixtures as Unity.

```sh
dotnet test DotNet/Rules.Tests/Junnav.KingOfTablut.Rules.Tests.csproj
```

## Project scope

This repository is a playable slice of an independent project. The full commercial project remains under private development. Source and assets here are provided for portfolio review and technical evaluation. All rights reserved, subject to applicable third-party licences.
