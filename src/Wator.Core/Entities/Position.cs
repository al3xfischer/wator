using System;

namespace Wator.Core.Entities
{
    [Serializable]
    public record Position
    {
        public Position(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        public int RowIndex { get; init; }
        public int ColumnIndex { get; init; }
    }
}
