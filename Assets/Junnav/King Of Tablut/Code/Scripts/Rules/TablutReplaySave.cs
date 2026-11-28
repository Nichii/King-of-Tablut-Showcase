using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Junnav.KingOfTablut.Gameplay
{
    public static class TablutReplaySave
    {
        private const string HeaderPrefix = "TABLUT-REPLAY ";
        private const string VersionOneHeader = HeaderPrefix + "1";

        /// <summary>
        /// Writes the starting board, side to move, and accepted moves as a version 1 text save.
        /// </summary>
        /// <exception cref="ArgumentNullException">The move history is null.</exception>
        /// <exception cref="ArgumentException">The starting configuration or a move is invalid.</exception>
        public static string Write(IEnumerable<TablutMove> moves, TablutPieceType[,] startingPositions = null,
            TablutPlayer startingPlayer = TablutPlayer.Attacker)
        {
            if (moves == null)
            {
                throw new ArgumentNullException(nameof(moves));
            }

            TablutGameMode match = new(startingPositions, startingPlayer);
            StringBuilder save = new();
            save.Append(VersionOneHeader).Append('\n');
            save.Append(startingPlayer == TablutPlayer.Attacker ? 'A' : 'D').Append('\n');

            // Store the actual starting board so a later change to the default layout cannot change an old replay.
            for (int y = 0; y < TablutBoard.TotalVerticalSquares; y++)
            {
                for (int x = 0; x < TablutBoard.TotalHorizontalSquares; x++)
                {
                    save.Append(GetPieceSymbol(match.GetPiece(new BoardPosition(x, y))));
                }

                save.Append('\n');
            }

            if (!TablutReplay.TryApplyMoves(match, WriteMoves(save, moves), out var failure))
            {
                throw new ArgumentException(GetMoveError(failure.Value), nameof(moves));
            }

            return save.ToString();
        }

        /// <summary>
        /// Loads a version 1 save and returns a fresh match only after the entire replay succeeds.
        /// </summary>
        public static bool TryLoad(string save, out TablutGameMode gameMode, out string error)
        {
            gameMode = null;
            using StringReader reader = new(save ?? string.Empty);
            string header = reader.ReadLine();
            if (header == null || !header.StartsWith(HeaderPrefix, StringComparison.Ordinal))
            {
                error = "Invalid replay save header.";
                return false;
            }

            if (header != VersionOneHeader)
            {
                error = "Unsupported replay save version.";
                return false;
            }

            try
            {
                TablutPlayer startingPlayer = ReadStartingPlayer(reader);
                TablutPieceType[,] positions = ReadStartingBoard(reader);
                if (TablutReplay.TryReplay(ReadMoves(reader), out gameMode, out var failure, positions, startingPlayer))
                {
                    error = null;
                    return true;
                }

                error = GetMoveError(failure.Value);
            }
            catch (FormatException exception)
            {
                error = exception.Message;
            }
            catch (ArgumentException exception)
            {
                error = exception.Message;
            }

            return false;
        }

        private static TablutPlayer ReadStartingPlayer(StringReader reader)
        {
            return reader.ReadLine() switch
            {
                "A" => TablutPlayer.Attacker,
                "D" => TablutPlayer.Defender,
                _ => throw new FormatException("Invalid starting side.")
            };
        }

        private static TablutPieceType[,] ReadStartingBoard(StringReader reader)
        {
            TablutPieceType[,] positions = new TablutPieceType[TablutBoard.TotalHorizontalSquares, TablutBoard.TotalVerticalSquares];
            for (int y = 0; y < positions.GetLength(1); y++)
            {
                string row = reader.ReadLine();
                if (row == null || row.Length != positions.GetLength(0))
                {
                    throw new FormatException($"Invalid board row {y}.");
                }

                for (int x = 0; x < row.Length; x++)
                {
                    positions[x, y] = row[x] switch
                    {
                        '.' => TablutPieceType.Empty,
                        'K' => TablutPieceType.WhiteKing,
                        'D' => TablutPieceType.WhitePawn,
                        'A' => TablutPieceType.BlackPawn,
                        _ => throw new FormatException($"Invalid piece at ({x}, {y}).")
                    };
                }
            }

            return positions;
        }

        private static char GetPieceSymbol(TablutPieceType piece)
        {
            return piece switch
            {
                TablutPieceType.Empty => '.',
                TablutPieceType.WhiteKing => 'K',
                TablutPieceType.WhitePawn => 'D',
                TablutPieceType.BlackPawn => 'A',
                _ => throw new ArgumentOutOfRangeException(nameof(piece))
            };
        }

        private static IEnumerable<TablutMove> WriteMoves(StringBuilder save, IEnumerable<TablutMove> moves)
        {
            foreach (TablutMove move in moves)
            {
                save.Append(FormattableString.Invariant($"{move.From.X},{move.From.Y},{move.To.X},{move.To.Y}")).Append('\n');
                yield return move;
            }
        }

        private static IEnumerable<TablutMove> ReadMoves(StringReader reader)
        {
            int moveIndex = 0;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                string[] coordinates = line.Split(',');
                if (coordinates.Length != 4
                    || !TryReadCoordinate(coordinates[0], out int fromX)
                    || !TryReadCoordinate(coordinates[1], out int fromY)
                    || !TryReadCoordinate(coordinates[2], out int toX)
                    || !TryReadCoordinate(coordinates[3], out int toY))
                {
                    throw new FormatException($"Move {moveIndex} has invalid coordinates.");
                }

                yield return new TablutMove(new BoardPosition(fromX, fromY), new BoardPosition(toX, toY));
                moveIndex++;
            }
        }

        private static bool TryReadCoordinate(string text, out int coordinate)
        {
            return int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out coordinate);
        }

        private static string GetMoveError((int MoveIndex, InvalidMoveMessage Reason) failure)
        {
            return $"Move {failure.MoveIndex} was rejected ({failure.Reason}).";
        }
    }
}
