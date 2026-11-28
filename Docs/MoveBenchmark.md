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
