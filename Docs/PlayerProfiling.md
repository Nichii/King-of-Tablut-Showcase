# Player profiling

The Windows and Web development players were built from `037474c` with Unity 6000.3.24f1 on Windows 11 (build 26200) and an Intel Core i7-8750H. Both builds had Development Build and Autoconnect Profiler enabled. Deep Profiling was off.

## Repeat the capture

1. Open `TablutGameplayScene` with the standard layout. In **File → Build Profiles**, select Windows or Web and enable **Development Build** and **Autoconnect Profiler**, then use **Build and Run**. For Web, use the local page Unity opens and keep the tab visible. [Unity Web profiling setup](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/building-distribution/web-profile)
2. In the Profiler CPU Usage module, use the player target and record the board clicks at the coordinates below. The fourth move captures the attacker at `(3, 3)`. Click **Restart** and confirm the attackers are to move again.
3. Repeat after warmup. Compare the Main Thread frame time and GC Allocated In Frame for frames containing the selection, move, capture, or restart.

| Move | Side | From | To |
| --- | --- | --- | --- |
| 1 | Attacker | `(0, 3)` | `(0, 2)` |
| 2 | Defender | `(4, 3)` | `(6, 3)` |
| 3 | Attacker | `(3, 0)` | `(3, 3)` |
| 4 | Defender | `(4, 2)` | `(3, 2)` |

Coordinates are zero-based `(x, y)`. For these captures, a temporary driver dispatched the clicks through the scene's event system and collected Unity's `Main Thread` and `GC Allocated In Frame` counters inside the players. It warmed up for 120 frames and eight complete sequences, then recorded 30 sequences with one action per sampled frame and one intervening frame. It checked the capture and restart HUD on every repeat. The player target was 60 FPS.

## Results

Both players reported 1024 × 768 at capture time and were focused. The Windows player used the GeForce GTX 1050 with Direct3D 12. The Web player ran in Microsoft Edge 158 with WebGL 2 through ANGLE on the Intel UHD Graphics 630 (Direct3D 11). The graphics paths differ, so the numbers describe these two player configurations.

The table shows median / 95th percentile for each frame category.

| Frame | Samples per player | Windows CPU (ms) | Windows GC (B) | Web CPU (ms) | Web GC (B) |
| --- | ---: | ---: | ---: | ---: | ---: |
| Idle | 30 | 16.66 / 16.73 | 76 / 76 | 16.70 / 16.80 | 0 / 0 |
| Select | 120 | 16.66 / 16.68 | 352 / 352 | 16.70 / 17.90 | 212 / 212 |
| Move | 90 | 16.66 / 16.68 | 200 / 200 | 16.60 / 16.90 | 84 / 84 |
| Capture | 30 | 16.66 / 16.75 | 352 / 352 | 16.65 / 16.80 | 188 / 188 |
| Restart | 30 | 16.66 / 16.69 | 1476 / 1476 | 16.70 / 18.50 | 1216 / 1216 |

The Main Thread counter includes frame pacing and waits. GC allocation is the total for the sampled frame, including input dispatch and UI work. Use the player trace to inspect individual markers. The [standalone benchmark](MoveBenchmark.md) measures the rules query separately.
