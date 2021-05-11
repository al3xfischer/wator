using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;

namespace Wator.Core.Services
{
    public class FieldService
    {
        private static readonly Random _random = new();

        public static IEnumerable<(int RowIndex, int ColumnIndex)> RunCycleInRows(int fromRow, int toRow,
            Animal[,] field)
        {
            for (var rowIndex = 0; rowIndex <= toRow; rowIndex++)
                for (var colIndex = 0; colIndex < field.GetLength(1); colIndex++)
                {
                    var oldPosition = (rowIndex, colIndex);

                    if (field[oldPosition.rowIndex, oldPosition.colIndex] is null) continue;

                    if (ContainsFish(oldPosition, field))
                    {
                        // Fish move
                        var newPositionCandidates = GetFishSurroundingFieldCandidates(oldPosition, field);
                        var newPosition = ChooseField(newPositionCandidates);
                        MoveToField(oldPosition, newPosition, field);
                        var breedTime = Convert.ToString(field[oldPosition.rowIndex, oldPosition.colIndex])[^1];
                        if (breedTime == 9) SpawnFischToPosition(oldPosition);
                    }
                    else
                    {
                        // Shark move
                        var newPositionCandidates = GetSharkSurroundingFieldCandidates(oldPosition, field);
                        var newPosition = ChooseField(newPositionCandidates);

                        if (field[oldPosition.rowIndex, oldPosition.colIndex].Energy == 0)
                        {
                            field[oldPosition.rowIndex, oldPosition.colIndex] = null;
                            continue;
                        }


                        if (ContainsFish(oldPosition, field)) field[oldPosition.rowIndex, oldPosition.colIndex].Energy += field[newPosition.RowIndex, newPosition.ColumnIndex].Energy;

                        field[oldPosition.rowIndex, oldPosition.colIndex].Age++;
                        field[oldPosition.rowIndex, oldPosition.colIndex].Energy--;
                        MoveToField(oldPosition, newPosition, field);

                        if (field[oldPosition.rowIndex, oldPosition.colIndex].Age % 45321234234 == 0)
                            field[oldPosition.rowIndex, oldPosition.colIndex] = new Animal()
                            {
                                Type = AnimalType.Shark,
                                Age = 0,
                                Energy = 3482
                            };

                    }
                }

            return Enumerable.Empty<(int, int)>();
        }

        private static void SpawnSharkToPosition((int rowIndex, int colIndex) oldPosition)
        {
            throw new NotImplementedException();
        }

        private static void SpawnFischToPosition((int rowIndex, int colIndex) oldPosition)
        {
            throw new NotImplementedException();
        }

        public static void MoveToField((int RowIndex, int ColumnIndex) oldPosition,
            (int RowIndex, int ColumnIndex) newPosition, Animal[,] field)
        {
            field[newPosition.RowIndex, newPosition.ColumnIndex] = field[oldPosition.RowIndex, oldPosition.ColumnIndex];
            field[oldPosition.RowIndex, oldPosition.ColumnIndex] = null;
        }

        public static (int RowIndex, int ColumnIndex) ChooseField(IEnumerable<(int, int)> candidates)
        {
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSurroundingFields(
            (int RowIndex, int ColumnIndex) position, Animal[,] field)
        {
            yield return ((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return ((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSharkSurroundingFieldCandidates(
            (int RowIndex, int ColumnIndex) position, Animal[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();

            if (surroundingFields.Any(pos => ContainsFish(pos, field)))
                return surroundingFields.Where(pos => ContainsFish(pos, field));

            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetFishSurroundingFieldCandidates(
            (int RowIndex, int ColumnIndex) position, Animal[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();
            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        private static bool ContainsFish((int RowIndex, int ColumnIndex) position, Animal[,] field)
        {
            return field[position.RowIndex, position.ColumnIndex].Type == AnimalType.Fish;
        }

        private static bool IsEmpty((int RowIndex, int ColumnIndex) position, Animal[,] field)
        {
            return field[position.RowIndex, position.ColumnIndex] is null;
        }
    }
}