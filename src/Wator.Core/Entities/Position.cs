namespace Wator.Core.Entities
{
    public record Position
    {
        public Position(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
    }
}
