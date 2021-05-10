using System;
using System.Collections.Generic;
using System.Linq;

namespace Wator.Core.Services
{
    public class FieldService
    {
        private static readonly Random _random = new();

        public static IEnumerable<(int RowIndex, int ColumnIndex)> RunCycleInRows(int fromRow, int toRow,
            sbyte[,] field)
        {
            for (var rowIndex = 0; rowIndex <= toRow; rowIndex++)
            for (var colIndex = 0; colIndex < field.GetLength(1); colIndex++)
            {
                var oldPosition = (rowIndex, colIndex);

                if (field[oldPosition.rowIndex, oldPosition.colIndex] == 0) continue;

                if (field[oldPosition.rowIndex, oldPosition.colIndex] > 0)
                {
                    // Fish move
                    var newPositionCandidates = GetFishSurroundingFieldCandidates(oldPosition, field);
                    var newPosition = ChooseField(newPositionCandidates);
                    MoveToField(oldPosition, newPosition, field);
                    // Eier legen ;)
                }
                else
                {
                    // Shark move
                    var newPositionCandidates = GetSharkSurroundingFieldCandidates(oldPosition, field);
                }
            }

            return Enumerable.Empty<(int, int)>();
        }

        public static void MoveToField((int RowIndex, int ColumnIndex) oldPosition,
            (int RowIndex, int ColumnIndex) newPosition, sbyte[,] field)
        {
            field[newPosition.RowIndex, newPosition.ColumnIndex] = field[oldPosition.RowIndex, oldPosition.ColumnIndex];
            field[oldPosition.RowIndex, oldPosition.ColumnIndex] = 0;
        }

        public static (int RowIndex, int ColumnIndex) ChooseField(IEnumerable<(int, int)> candidates)
        {
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSurroundingFields(
            (int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            yield return ((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return ((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSharkSurroundingFieldCandidates(
            (int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();

            if (surroundingFields.Any(pos => ContainsFish(pos, field)))
                return surroundingFields.Where(pos => ContainsFish(pos, field));

            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetFishSurroundingFieldCandidates(
            (int RowIndex, int ColumnIndex) position, sbyte[,] field)
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