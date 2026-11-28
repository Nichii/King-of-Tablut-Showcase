# Legal move benchmark

Run from the repository root with the .NET 10 SDK.

```sh
dotnet run -c Release --project DotNet/Rules.Benchmarks
```

The benchmark calls `GetLegalDestinations` for one selected attacker in each position. It verifies the exact destinations before timing and checks the total returned count for each sample. Board setup, destination verification, and console output are outside the timed loop. Each position gets 50,000 warmup queries and seven samples of 50,000 queries. The output reports the median time and managed bytes allocated per query on the calling thread.

## Baseline

Measured against rules commit `dedbf7d` on an Intel Core i7-8750H (6 cores, 12 logical processors), Windows x64 build 26200, .NET SDK 10.0.401 and runtime 10.0.12. The three baseline runs were consecutive, with a fresh Release process for each.

| Position | Legal destinations | Run 1 (ns/query) | Run 2 (ns/query) | Run 3 (ns/query) | Median of runs (ns/query) | Bytes/query |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Standard opening | 6 | 729 | 660 | 617 | 660 | 176 |
| Open row and column | 16 | 737 | 699 | 681 | 699 | 328 |
| Lines blocked by pieces and the throne | 4 | 603 | 621 | 594 | 603 | 88 |

Allocation counts matched across all three runs. These measurements cover the standalone rules query. Unity and browser frame timings require player profiling.

## Ray-based move generation

The original generator tried all 81 destination squares. The ray-based generator walks from the selected piece in four directions and stops at the first piece, the throne, or the board edge. Move validation uses the same blocker check. Tests compare every destination and its order with move execution for both sides.

The comparison used the same machine and runtime, alternating the order of the original scan (`b857bd3`) and the ray-based generator across three pairs. Each value below is the median of three run medians. Parentheses show the range of those run medians.

| Position | Original scan (ns/query) | Ray-based (ns/query) | Speedup | Bytes/query on both |
| --- | ---: | ---: | ---: | ---: |
| Standard opening | 656 (646–658) | 133 (132–144) | 4.9× | 176 |
| Open row and column | 691 (680–750) | 304 (296–314) | 2.3× | 328 |
| Lines blocked by pieces and the throne | 587 (569–599) | 110 (52–115) | 5.3× | 88 |

See [player profiling](PlayerProfiling.md) for Windows and Web frame measurements.
