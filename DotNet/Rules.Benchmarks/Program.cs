using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Junnav.KingOfTablut.Gameplay;

namespace Junnav.KingOfTablut.Benchmarks
{
    internal static class Program
    {
        private const int WarmupQueries = 50_000;
        private const int QueriesPerSample = 50_000;
        private const int SampleCount = 7;

        private static int Main()
        {
            Console.WriteLine($"{RuntimeInformation.FrameworkDescription}, {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"{WarmupQueries} warmup queries and {SampleCount} samples of {QueriesPerSample} queries per position. Results are medians.");

            TablutPieceType[,] open = new TablutPieceType[9, 9];
            open[3, 3] = TablutPieceType.BlackPawn;
            open[7, 7] = TablutPieceType.WhiteKing;

            TablutPieceType[,] blocked = new TablutPieceType[9, 9];
            blocked[4, 2] = TablutPieceType.BlackPawn;
            blocked[4, 0] = TablutPieceType.BlackPawn;
            blocked[2, 2] = TablutPieceType.WhitePawn;
            blocked[6, 2] = TablutPieceType.WhitePawn;
            blocked[7, 7] = TablutPieceType.WhiteKing;

            bool passed = Measure("Opening", new TablutGameMode(), new BoardPosition(0, 3), new[]
            {
                new BoardPosition(0, 0), new BoardPosition(0, 1), new BoardPosition(0, 2),
                new BoardPosition(1, 3), new BoardPosition(2, 3), new BoardPosition(3, 3)
            })
            && Measure("Open lines", new TablutGameMode(open), new BoardPosition(3, 3), new[]
            {
                new BoardPosition(0, 3), new BoardPosition(1, 3), new BoardPosition(2, 3),
                new BoardPosition(4, 3), new BoardPosition(5, 3), new BoardPosition(6, 3),
                new BoardPosition(7, 3), new BoardPosition(8, 3),
                new BoardPosition(3, 0), new BoardPosition(3, 1), new BoardPosition(3, 2),
                new BoardPosition(3, 4), new BoardPosition(3, 5), new BoardPosition(3, 6),
                new BoardPosition(3, 7), new BoardPosition(3, 8)
            })
            && Measure("Blocked lines", new TablutGameMode(blocked), new BoardPosition(4, 2), new[]
            {
                new BoardPosition(3, 2), new BoardPosition(5, 2),
                new BoardPosition(4, 1), new BoardPosition(4, 3)
            });

            return passed ? 0 : 1;
        }

        private static bool Measure(string name, TablutGameMode gameMode, BoardPosition from, BoardPosition[] expected)
        {
            IReadOnlyList<BoardPosition> destinations = gameMode.GetLegalDestinations(from);
            if (destinations.Count != expected.Length || !new HashSet<BoardPosition>(expected).SetEquals(destinations))
            {
                Console.Error.WriteLine($"{name}: unexpected legal destinations from {from}.");
                Console.Error.WriteLine($"Expected: {string.Join(", ", expected)}");
                Console.Error.WriteLine($"Actual: {string.Join(", ", destinations)}");
                return false;
            }

            RunQueries(gameMode, from, WarmupQueries);
            double[] times = new double[SampleCount];
            double[] allocations = new double[SampleCount];
            for (int sample = 0; sample < SampleCount; sample++)
            {
                // Measure the queries with the board already built and warmed up.
                long startBytes = GC.GetAllocatedBytesForCurrentThread();
                long startTime = Stopwatch.GetTimestamp();
                int totalDestinations = RunQueries(gameMode, from, QueriesPerSample);
                times[sample] = Stopwatch.GetElapsedTime(startTime).TotalNanoseconds / QueriesPerSample;
                allocations[sample] = (GC.GetAllocatedBytesForCurrentThread() - startBytes) / (double)QueriesPerSample;

                if (totalDestinations != QueriesPerSample * expected.Length)
                {
                    Console.Error.WriteLine($"{name}: the destination count changed during measurement.");
                    return false;
                }
            }

            Array.Sort(times);
            Array.Sort(allocations);
            int middle = SampleCount / 2;
            Console.WriteLine(FormattableString.Invariant(
                $"{name}: {times[middle]:F0} ns/query, {allocations[middle]:F0} B/query, {expected.Length} destinations"));
            return true;
        }

        private static int RunQueries(TablutGameMode gameMode, BoardPosition from, int count)
        {
            int totalDestinations = 0;
            for (int i = 0; i < count; i++)
            {
                totalDestinations += gameMode.GetLegalDestinations(from).Count;
            }

            return totalDestinations;
        }
    }
}
