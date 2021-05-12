﻿using System;
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
                    var newPositionCandidates = GetFishSurroundingFieldCandidates(oldPosition, field);
                    var newPosition = ChooseField(newPositionCandidates);
                    animal.Age++;
                    animal.Energy--;
                    MoveToField(oldPosition, newPosition, field);
                    moved.Add(newPosition);
                    if (animal.Age % 3 == 0)
                        SetAnimalAtPosition(new Animal {Type = AnimalType.Fish, Age = 0, Energy = 3482}, field,
                            oldPosition);
                }
                else
                {
                    // Shark move
                    var newPositionCandidates = GetSharkSurroundingFieldCandidates(oldPosition, field);
                    var newPosition = ChooseField(newPositionCandidates);
                    var prey = GetAnimalAtPosition(field, newPosition);

                    if (animal.Energy == 0)
                    {
                        SetAnimalAtPosition(null, field, oldPosition);
                        continue;
                    }

                    if (prey is not null) animal.Energy += prey.Energy;

                    animal.Age++;
                    animal.Energy--;
                    MoveToField(oldPosition, newPosition, field);
                    moved.Add(newPosition);

                    if (animal.Age % 4 == 0)
                        SetAnimalAtPosition(new Animal {Type = AnimalType.Shark, Age = 0, Energy = 3482}, field,
                            oldPosition);
                }
            }

            return moved;
        }

        private static void SpawnSharkToPosition((int rowIndex, int colIndex) oldPosition)
        {
            throw new NotImplementedException();
        }

        private static void SpawnFischToPosition((int rowIndex, int colIndex) oldPosition)
        {
            throw new NotImplementedException();
        }

        public static void MoveToField(Position oldPosition, Position newPosition, Animal[,] field)
        {
            SetAnimalAtPosition(GetAnimalAtPosition(field, oldPosition), field, newPosition);
            SetAnimalAtPosition(null, field, oldPosition);
        }

        public static Position ChooseField(IEnumerable<Position> candidates)
        {
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        public static IEnumerable<Position> GetSurroundingFields<T>(
            Position position, T[,] field)
        {
            yield return new Position((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0),
                position.ColumnIndex);
            yield return new Position(position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return new Position((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return new Position(position.RowIndex,
                (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }

        public static IEnumerable<Position> GetSharkSurroundingFieldCandidates(
            Position position, Animal[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();

            if (surroundingFields.Any(pos => ContainsFish(pos, field)))
                return surroundingFields.Where(pos => ContainsFish(pos, field));

            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        public static IEnumerable<Position> GetFishSurroundingFieldCandidates(
            Position position, Animal[,] field)
        {
            var surroundingFields = GetSurroundingFields(position, field).ToArray();
            return surroundingFields.Where(pos => IsEmpty(pos, field));
        }

        private static bool ContainsFish(Position position, Animal[,] field)
        {
            return GetAnimalAtPosition(field, position)?.Type == AnimalType.Fish;
        }

        private static bool IsEmpty(Position position, Animal[,] field)
        {
            return GetAnimalAtPosition(field, position) is null;
        }

        private static Animal GetAnimalAtPosition(Animal[,] field, Position position)
        {
            return field[position.RowIndex, position.ColumnIndex];
        }

        private static void SetAnimalAtPosition(Animal animal, Animal[,] field, Position position)
        {
            field[position.RowIndex, position.ColumnIndex] = animal;
        }
    }
}