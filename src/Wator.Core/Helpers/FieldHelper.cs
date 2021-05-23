using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;

namespace Wator.Core.Helpers
{
    public class FieldHelper
    {
        /// <summary>
        /// Gets the surrounding fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static IEnumerable<Position> GetSurroundingFields<T>(T[,] field, Position position)
        {
            var top = new Position((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0),
                position.ColumnIndex);
            var right = new Position(position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            var bottom = new Position((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            var left = new Position(position.RowIndex,
                (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));

            return new List<Position> { top, right, bottom, left };
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// from - Out of source index.
        /// or
        /// to - Out of source index.
        /// or
        /// from - Must be lower or equal to to.
        /// </exception>
        public static T[,] GetRows<T>(T[,] source, int from, int to)
        {
            if (from >= source.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(from), "Out of source index.");
            if (to >= source.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(to), "Out of source index.");
            if (from > to) throw new ArgumentOutOfRangeException(nameof(from), "Must be lower or equal to to.");

            var rowCount = to - from + 1;
            var columnCount = source.GetLength(1);
            var rows = new T[rowCount, columnCount];

            var offset = from * source.GetLength(1);
            var itemCountToCopy = (to - from + 1) * source.GetLength(1);

            Array.Copy(source, offset, rows, 0, itemCountToCopy);

            return rows;
        }

        /// <summary>
        /// Gets the split boundaries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="partCount">The part count.</param>
        /// <returns></returns>
        public static (int FromRow, int ToRow)[] GetSplitBoundaries<T>(T[,] source, int partCount)
        {
            var rowCount = source.GetLength(0);
            var segmentHeight = rowCount / partCount;
            var result = new (int, int)[partCount];

            for (var i = 0; i < partCount; i++)
            {
                var from = i * segmentHeight;
                var to = Math.Min(segmentHeight, rowCount - from) - 1 + from;

                if (i == partCount - 1) to = Math.Max(to, rowCount - 1);

                result[i] = (from, to);
            }

            return result;
        }

        /// <summary>
        /// Splits the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="partCount">The part count.</param>
        /// <returns></returns>
        public static T[][,] Split<T>(T[,] source, int partCount)
        {
            return GetSplitBoundaries(source, partCount)
                .Select(indices => GetRows(source, indices.FromRow, indices.ToRow))
                .ToArray();
        }

        public static T[,] MergeTwo<T>(T[,] partOne, T[,] partTwo)
        {
            if (partOne.GetLength(1) != partTwo.GetLength(1))
                throw new InvalidOperationException("Column width must match.");

            var result = new T[partOne.GetLength(0) + partTwo.GetLength(0), partOne.GetLength(1)];

            Array.Copy(partOne, 0, result, 0, partOne.Length);
            Array.Copy(partTwo, 0, result, partOne.Length, partTwo.Length);

            return result;
        }

        /// <summary>
        /// Merges the specified parts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        public static T[,] Merge<T>(T[][,] parts)
        {
            return parts.Aggregate(MergeTwo);
        }

        /// <summary>
        /// Cuts the before.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static T[][,] CutBefore<T>(T[,] source, int index)
        {
            var lastIndex = source.GetLength(0) - 1;
            var partOne = GetRows(source, 0, index - 1);
            var partTwo = GetRows(source, index, lastIndex);
            return new T[2][,] { partOne, partTwo };
        }
    }
}