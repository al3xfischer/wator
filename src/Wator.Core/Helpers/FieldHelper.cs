using System;
using System.Linq;

namespace Wator.Core.Helpers
{
    public class FieldHelper
    {
        public T[,] GetRows<T>(T[,] source, int from, int to)
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

        public T[][,] Split<T>(T[,] source, int partCount)
        {
            var rowCount = source.GetLength(0);
            var segmentHeight = rowCount / partCount;
            var result = new T[partCount][,];

            for (var i = 0; i < partCount; i++)
            {
                var from = i * segmentHeight;
                var to = Math.Min(segmentHeight, rowCount - from) - 1 + from;

                if (i == partCount - 1) to = Math.Max(to, rowCount - 1);

                result[i] = GetRows(source, from, to);
            }

            return result;
        }


        public T[,] MergeTwo<T>(T[,] partOne, T[,] partTwo)
        {
            if (partOne.GetLength(1) != partTwo.GetLength(1))
                throw new InvalidOperationException("Column width must match.");

            var result = new T[partOne.GetLength(0) + partTwo.GetLength(0), partOne.GetLength(1)];

            Array.Copy(partOne, 0, result, 0, partOne.Length);
            Array.Copy(partTwo, 0, result, partOne.Length, partTwo.Length);

            return result;
        }

        public T[,] Merge<T>(T[][,] parts)
        {
            return parts.Aggregate(MergeTwo);
        }
    }
}