using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;

namespace Wator.Core.Services
{
    public class FieldService
    {
        private const int FishBreedTime = 3;
        private const int SharkBreedTime = 3;

        private static readonly Random _random = new();

        public static IEnumerable<Position> RunCycleInRows(int fromRow, int toRow,
            Animal[,] field)
        {
            var moved = new List<Position>();

            for (var rowIndex = fromRow; rowIndex <= toRow; rowIndex++)
            for (var colIndex = 0; colIndex < field.GetLength(1); colIndex++)
            {
                var currentPosition = new Position(rowIndex, colIndex);

                if (currentPosition is null)
                {
                    var x = "asdfasf";
                }

                if (IsEmpty(field, currentPosition) || moved.Contains(currentPosition)) continue;
                if (ContainsFish(field, currentPosition)) currentPosition = PerformFishChronon(field, currentPosition);
                if (ContainsShark(field, currentPosition)) currentPosition = PerformSharkChronon(field, currentPosition);
                if (currentPosition is not null) moved.Add(currentPosition);
            }

            return moved;
        }

        public static void MoveToField(Animal[,] field, Position oldPosition, Position newPosition)
        {
            SetAnimalAtPosition(field, newPosition, GetAnimalAtPosition(field, oldPosition));
            SetAnimalAtPosition(field, oldPosition, null);
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

        public static Position ChooseField(IEnumerable<Position> candidates)
        {
            if (!candidates.Any()) return null;
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        private static Position ChooseEmptySurroundingField(Animal[,] field, Position position)
        {
            return ChooseField(GetEmptySurroundingFields(field, position));
        }

        private static Position ChoosePreyInSurroundingField(Animal[,] field, Position position)
        {
            return ChooseField(GetPreyInSurroundingFields(field, position));
        }

        private static IEnumerable<Position> GetEmptySurroundingFields(Animal[,] field, Position position)
        {
            return GetSurroundingFields(field, position).Where(pos => IsEmpty(field, pos));
        }

        private static IEnumerable<Position> GetPreyInSurroundingFields(Animal[,] field, Position position)
        {
            return GetSurroundingFields(field, position).Where(pos => ContainsFish(field, pos));
        }

        private static Position PerformSharkChronon(Animal[,] field, Position position)
        {
            if (!ContainsShark(field, position))
                throw new InvalidOperationException("Cannot perform action for non-shark cells.");

            var shark = GetAnimalAtPosition(field, position);
            if (shark.Energy == 0)
            {
                SetAnimalAtPosition(field, position, null);
                return position;
            }

            var newPosition = ChoosePreyInSurroundingField(field, position) ??
                              ChooseEmptySurroundingField(field, position);

            if (newPosition is not null)
            {
                var prey = newPosition is null ? null : GetAnimalAtPosition(field, newPosition);
                if (prey is not null) shark.Energy += prey.Energy;

                MoveToField(field, position, newPosition);

                if (shark.Age % SharkBreedTime == 0) BreedSharkToPosition(field, position);
            }

            shark.Age++;
            shark.Energy--;

            return newPosition;
        }

        private static Position PerformFishChronon(Animal[,] field, Position position)
        {
            if (!ContainsFish(field, position))
                throw new InvalidOperationException("Cannot perform action for non-fish cells.");

            var fish = GetAnimalAtPosition(field, position);
            var newPosition = ChooseEmptySurroundingField(field, position);
            if (newPosition is not null) MoveToField(field, position, newPosition);

            if (fish.Age % FishBreedTime == 0) BreedFishToPosition(field, position);
            fish.Age++;

            return newPosition;
        }

        private static void BreedSharkToPosition(Animal[,] field, Position position)
        {
            var babyShark = new Animal {Type = AnimalType.Shark, Age = 0, Energy = 20};
            SetAnimalAtPosition(field, position, babyShark);
        }

        private static void BreedFishToPosition(Animal[,] field, Position position)
        {
            var babyFish = new Animal {Type = AnimalType.Fish, Age = 0, Energy = 10};
            SetAnimalAtPosition(field, position, babyFish);
        }

        private static bool ContainsFish(Animal[,] field, Position position)
        {
            return GetAnimalAtPosition(field, position)?.Type == AnimalType.Fish;
        }

        private static bool ContainsShark(Animal[,] field, Position position)
        {
            return GetAnimalAtPosition(field, position)?.Type == AnimalType.Shark;
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