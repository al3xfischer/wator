using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Helpers;

namespace Wator.Core.Services
{
    public class WatorSimulation
    {
        private const int FishBreedTime = 3;
        private const int SharkBreedTime = 3;
        private const int SharkInitialEnergy = 20;
        private const int FishInitialEnergy = 10;

        private readonly Random _random = new();

        public WatorSimulation(Animal[,] field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public Animal[,] Field { get; }

        public IEnumerable<Position> RunCycleInRows(int fromRow, int toRow)
        {
            var moved = new List<Position>();

            for (var rowIndex = fromRow; rowIndex <= toRow; rowIndex++)
            for (var colIndex = 0; colIndex < Field.GetLength(1); colIndex++)
            {
                var currentPosition = new Position(rowIndex, colIndex);

                if (IsEmpty(currentPosition) || moved.Contains(currentPosition)) continue;
                if (ContainsFish(currentPosition)) currentPosition = PerformFishChronon(currentPosition);
                if (ContainsShark(currentPosition))
                    currentPosition = PerformSharkChronon(currentPosition);
                moved.Add(currentPosition);
            }

            return moved;
        }

        public Position ChooseField(IEnumerable<Position> candidates)
        {
            return candidates.ElementAt(_random.Next(0, candidates.Count()));
        }

        private Position ChooseEmptySurroundingField(Position position)
        {
            return ChooseField(GetEmptySurroundingFields(position));
        }

        private Position ChoosePreyInSurroundingField(Position position)
        {
            return ChooseField(GetPreyInSurroundingFields(position));
        }

        private Position PerformSharkChronon(Position position)
        {
            if (!ContainsShark(position))
                throw new InvalidOperationException("Cannot perform action for non-shark cells.");

            var shark = GetAnimalAtPosition(position);
            if (shark.Energy == 0)
            {
                SetAnimalAtPosition(position, null);
                return position;
            }

            if (CanEatPreyOrMoveToEmptyField(position))
            {
                position = EatPreyOrMoveToEmptyField(position);
                if (shark.Age % SharkBreedTime == 0) BreedSharkToPosition(position);
            }

            shark.Age++;
            shark.Energy--;

            return position;
        }

        private Position PerformFishChronon(Position position)
        {
            if (!ContainsFish(position))
                throw new InvalidOperationException("Cannot perform action for non-fish cells.");

            var fish = GetAnimalAtPosition(position);

            if (CanMoveToEmptyField(position))
            {
                position = MoveToEmptyField(position);
                if (fish.Age % FishBreedTime == 0) BreedFishToPosition(position);
            }

            fish.Age++;

            return position;
        }

        private IEnumerable<Position> GetEmptySurroundingFields(Position position)
        {
            return FieldHelper.GetSurroundingFields(Field, position).Where(pos => IsEmpty(pos));
        }

        private IEnumerable<Position> GetPreyInSurroundingFields(Position position)
        {
            return FieldHelper.GetSurroundingFields(Field, position).Where(pos => ContainsFish(pos));
        }

        private Position EatPreyOrMoveToEmptyField(Position position)
        {
            if (CanEatPrey(position)) return EatPrey(position);
            return MoveToEmptyField(position);
        }

        private void MoveToField(Position oldPosition, Position newPosition)
        {
            SetAnimalAtPosition(newPosition, GetAnimalAtPosition(oldPosition));
            SetAnimalAtPosition(oldPosition, null);
        }

        private Position EatPrey(Position position)
        {
            if (!CanEatPrey(position)) throw new InvalidOperationException("Cannot eat prey.");

            var shark = GetAnimalAtPosition(position);
            var preyPosition = ChoosePreyInSurroundingField(position);
            shark.Energy += GetAnimalAtPosition(preyPosition).Energy;
            MoveToField(position, preyPosition);
            return preyPosition;
        }

        private Position MoveToEmptyField(Position position)
        {
            if (!CanMoveToEmptyField(position))
                throw new InvalidOperationException("Cannot move to empty field.");

            var newPosition = ChooseEmptySurroundingField(position);
            MoveToField(position, newPosition);
            return newPosition;
        }

        private bool CanEatPreyOrMoveToEmptyField(Position position)
        {
            return CanEatPrey(position) || CanMoveToEmptyField(position);
        }

        private bool CanEatPrey(Position position)
        {
            return GetPreyInSurroundingFields(position).Any();
        }

        private bool CanMoveToEmptyField(Position position)
        {
            return GetEmptySurroundingFields(position).Any();
        }

        private void BreedSharkToPosition(Position position)
        {
            var babyShark = new Animal {Type = AnimalType.Shark, Age = 0, Energy = SharkInitialEnergy};
            SetAnimalAtPosition(position, babyShark);
        }

        private void BreedFishToPosition(Position position)
        {
            var babyFish = new Animal {Type = AnimalType.Fish, Age = 0, Energy = FishInitialEnergy};
            SetAnimalAtPosition(position, babyFish);
        }

        private bool ContainsFish(Position position)
        {
            return GetAnimalAtPosition(position)?.Type == AnimalType.Fish;
        }

        private bool ContainsShark(Position position)
        {
            return GetAnimalAtPosition(position)?.Type == AnimalType.Shark;
        }

        private bool IsEmpty(Position position)
        {
            return GetAnimalAtPosition(position) is null;
        }

        private Animal GetAnimalAtPosition(Position position)
        {
            return Field[position.RowIndex, position.ColumnIndex];
        }

        private void SetAnimalAtPosition(Position position, Animal animal)
        {
            Field[position.RowIndex, position.ColumnIndex] = animal;
        }
    }
}