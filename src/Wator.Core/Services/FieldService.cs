using System;
using Wator.Core.Entities;

namespace Wator.Core.Services
{
    public class FieldService
    {
        public ReadOnlyMemory<ReadOnlyMemory<Cell>> Split(int partCount,Field field)
        {
            var segmentHeight = field.Height / partCount;
            var parts = new ReadOnlyMemory<Cell>[partCount];
            for (int i = 0; i < partCount; i++)
            {
                var start = i * segmentHeight;
                parts[i] = field.GetRows(start, segmentHeight);
            }

            return parts;
        }


        public ReadOnlyMemory<Cell> Merge(ReadOnlyMemory<ReadOnlyMemory<Cell>> parts) {
            var partLength = parts.Length;
            var length = parts.Length;
            var merged = new Cell[partLength * length];
            parts.Span[0].CopyTo(merged);

            for (int i = 1; i < length; i++)
            {
                parts.Span[i].CopyTo(merged.AsMemory(partLength));
            }

            return merged;
        }
    }
}
