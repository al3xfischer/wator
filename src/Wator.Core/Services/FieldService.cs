using System;
using System.Linq;
using Wator.Core.Entities;
namespace Wator.Core.Services
{
    public class FieldService
    {
        public Cell[,] GetRows(Cell[,] cells, int from, int to)
        {
            if (from >= cells.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(from), "Out of source index.");
            if (to >= cells.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(to), "Out of source index.");
            if (from > to) throw new ArgumentOutOfRangeException(nameof(from), "Must be lower or equal to to.");

            var rowCount = to - from + 1;
            var columnCount = cells.GetLength(0);

            var rows = new Cell[columnCount, rowCount];

            for (var cRow = 0; cRow < rowCount; cRow++)
            {
                for (var cCol = 0; cCol < columnCount; cCol++)
                {
                    rows[cCol, cRow] = cells[cCol, from + cRow];
                }
            }

            return rows;
        }

        public Cell[][,] Split(int partCount, Cell[,] field)
        {
            var rowCount = field.GetLength(0);
            var segmentHeight = rowCount / partCount;
            var result = new Cell[partCount][,];

            for (int i = 0; i < partCount; i++)
            {
                var from = i * segmentHeight;
                var to = Math.Min(segmentHeight, rowCount - from) - 1 + from;

                if (i == partCount - 1)
                {
                    to = Math.Max(to, rowCount - 1);
                }

                result[i] = GetRows(field, from, to);
            }

            return result;
        }


        public Cell[,] MergeTwo(Cell[,] partOne, Cell[,] partTwo)
        {
            var result = new Cell[Math.Max(partOne.GetLength(0), partTwo.GetLength(0)), partOne.GetLength(1) + partTwo.GetLength(1)];
            Array.Copy(partOne, 0, result, 0, partOne.Length);
            Array.Copy(partTwo, 0, result, partOne.Length, partTwo.Length);

            return result;
        }

        public Cell[,] Merge(Cell[][,] parts)
        {
            return parts.Aggregate(MergeTwo);
        }
    }
}
