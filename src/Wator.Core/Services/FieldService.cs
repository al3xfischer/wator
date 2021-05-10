using System.Collections.Generic;
using System.Linq;

namespace Wator.Core.Services
{
    public class FieldService
    {
        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSurroundingFields(
            (int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            yield return ((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return ((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSharkSurroundingFieldCandidates((int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();

            if (surroundingFields.Any(pos => ContainsFish(pos, field)))
                return surroundingFields.Where(pos => ContainsFish(pos, field));

            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetFishSurroundingFieldCandidates((int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();
            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        private static bool ContainsFish((int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            return field[position.RowIndex, position.ColumnIndex] > 0;
        }

        private static bool IsEmpty((int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            return field[position.RowIndex, position.ColumnIndex] == 0;
        }
    }
}
