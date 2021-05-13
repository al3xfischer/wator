using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;

namespace Wator.Core.Services
{
    public class FieldService
    {
        private static readonly Random _random = new();

        public static IEnumerable<Position> RunCycleInRows(int fromRow, int toRow,
            Animal[,] field)
        {
            var moved = new List<Position>();

            for (var rowIndex = fromRow; rowIndex <= toRow; rowIndex++)
            for (var colIndex = 0; colIndex < field.GetLength(1); colIndex++)
            {
                var oldPosition = new Position(rowIndex, colIndex);
                var animal = GetAnimalAtPosition(field, oldPosition);

                if (animal is null || moved.Contains(oldPosition)) continue;

                if (animal.Type == AnimalType.Fish)
                {
                    // Fish move
                    var newPositionCandidates = GetFishSurroundingFieldCandidates(field, oldPosition);
                    var newPosition = ChooseField(newPositionCandidates);
                    animal.Age++;
                    animal.Energy--;
                    MoveToField(field, oldPosition, newPosition);
                    moved.Add(newPosition);
                    if (animal.Age % 3 == 0)
                        SetAnimalAtPosition(field,
                            oldPosition, new Animal {Type = AnimalType.Fish, Age = 0, Energy = 3482});
                }
                else
                {
                    // Shark move
                    var newPositionCandidates = GetSharkSurroundingFieldCandidates(field, oldPosition);
                    var newPosition = ChooseField(newPositionCandidates);
                    var prey = GetAnimalAtPosition(field, newPosition);

                    if (animal.Energy == 0)
                    {
                        SetAnimalAtPosition(field, oldPosition, null);
                        continue;
                    }

                    if (prey is not null) animal.Energy += prey.Energy;

                    animal.Age++;
                    animal.Energy--;
                    MoveToField(field, oldPosition, newPosition);
                    moved.Add(newPosition);

                    if (animal.Age % 4 == 0)
                        SetAnimalAtPosition(field,
                            oldPosition, new Animal {Type = AnimalType.Shark, Age = 0, Energy = 3482});
                }
            }

            return moved;
        }

        public static void MoveToField(Animal[,] field, Position oldPosition, Position newPosition)
        {
            SetAnimalAtPosition(field, newPosition, GetAnimalAtPosition(field, oldPosition));
            SetAnimalAtPosition(field, oldPosition, null);
        }

        public static Position ChooseField(IEnumerable<Position> candidates)
        {
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        public static IEnumerable<Position> GetSurroundingFields<T>(T[,] field, Position position)
        {
            yield return new Position((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0),
                position.ColumnIndex);
            yield return new Position(position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return new Position((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return new Position(position.RowIndex,
                (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }

        public static IEnumerable<Position> GetSharkSurroundingFieldCandidates(Animal[,] field, Position position)
        {
            var surroundingFields = GetSurroundingFields(field, position).ToArray();

            if (surroundingFields.Any(pos => ContainsFish(field, pos)))
                return surroundingFields.Where(pos => ContainsFish(field, pos));

            return surroundingFields.Where(pos => IsEmpty(field, pos));
        }

        public static IEnumerable<Position> GetFishSurroundingFieldCandidates(Animal[,] field, Position position)
        {
            var surroundingFields = GetSurroundingFields(field, position).ToArray();
            return surroundingFields.Where(pos => IsEmpty(field, pos));
        }

        private static bool ContainsFish(Animal[,] field, Position position)
        {
            return GetAnimalAtPosition(field, position)?.Type == AnimalType.Fish;
        }

        private static bool IsEmpty(Animal[,] field, Position position)
        {
            return GetAnimalAtPosition(field, position) is null;
        }

        private static Animal GetAnimalAtPosition(Animal[,] field, Position position)
        {
            return field[position.RowIndex, position.ColumnIndex];
        }

        private static void SetAnimalAtPosition(Animal[,] field, Position position, Animal animal)
        {
            field[position.RowIndex, position.ColumnIndex] = animal;
        }
    }
}