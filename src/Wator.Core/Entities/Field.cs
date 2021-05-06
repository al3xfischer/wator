using System;

namespace Wator.Core.Entities
{
    public record Field
    {
        public Field(int width, int height, Cell[] cells)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("The width must be at least one.");
            }

            if (height < 1 )
            {
                throw new ArgumentOutOfRangeException("The height must be at least one.");
            }

            Width = width;
            Height = height;

            Cells = cells ?? throw new ArgumentNullException(nameof(cells));
        }

        public Cell[] Cells { get; init; }

        public int Width { get; init; }

        public int Height { get; init; }

        public ReadOnlyMemory<Cell> GetRows(int start, int count)
        {
            var length = count * Width;
            return Cells.AsMemory().Slice(start, length);

        }
    }
}
