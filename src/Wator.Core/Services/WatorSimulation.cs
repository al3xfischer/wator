using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Helpers;

namespace Wator.Core.Services
{
    public class WatorSimulation
    {
        private readonly Random _random;

        public WatorSimulation(int[,] field, WatorConfiguration configuration = null)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Configuration = configuration ?? new WatorConfiguration();
            _random = Configuration.Seed.HasValue ? new Random(Configuration.Seed.Value) : new Random();
        }

        public int[,] Field { get; set; }
        public WatorConfiguration Configuration { get; }

        public int FishCount => Field.Cast<int>().Count(a => a > 0);
        public int SharkCount => Field.Cast<int>().Count(a => a < 0);

        public IEnumerable<Position> RunCycle()
        {
            return RunCycleInRows(0, Field.GetLength(0) - 1);
        }

        public HashSet<Position> RunCycleInRows(int fromRow, int toRow, HashSet<Position> ignoredPositions = null)
        {
            ignoredPositions ??= new HashSet<Position>();

            //var ignoreInNextRow = new HashSet<Position>();
            var outOfBorderPositions = new HashSet<Position>();

            (fromRow, toRow) = Normalize(fromRow, toRow);

            for (var rowIndex = fromRow; rowIndex <= toRow; rowIndex++)
            {
                var normalizedRowIndex = rowIndex % Field.GetLength(0);
                //var ignoreInCurrentRow = ignoreInNextRow;
                //ignoreInNextRow = new HashSet<Position>();

                for (var colIndex = 0; colIndex < Field.GetLength(1); colIndex++)
                {
                    var currentPosition = new Position(normalizedRowIndex, colIndex);

                    if (ignoredPositions.Contains(currentPosition)) continue;
                    //if (ignoreInCurrentRow.Contains(currentPosition)) continue;
                    if (IsEmpty(currentPosition)) continue;

                    var newPosition = ContainsFish(currentPosition)
                        ? PerformFishChronon(currentPosition)
                        : PerformSharkChronon(currentPosition);

                    if (currentPosition == newPosition) continue;
                    ignoredPositions.Add(newPosition);

                    //if (newPosition.RowIndex > normalizedRowIndex) ignoreInNextRow.Add(newPosition);
                    //if (newPosition.ColumnIndex > colIndex) ignoreInCurrentRow.Add(newPosition);
                    if (newPosition.RowIndex < fromRow || newPosition.RowIndex > toRow)
                        outOfBorderPositions.Add(newPosition);
                }
            }

            return outOfBorderPositions;
        }

        private (int FromRow, int ToRow) Normalize(int fromRow, int toRow)
        {
            var rowCount = Field.GetLength(0);
            fromRow %= rowCount;
            toRow %= rowCount;
            if (fromRow > toRow) toRow += fromRow + Field.GetLength(0) - fromRow;
            return (fromRow, toRow);
        }

        private Position ChooseField(IEnumerable<Position> candidates)
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

            var newPosition = position;
            var currentEnergy = GetAnimalAtPosition(position);

            if (currentEnergy == 0)
            {
                SetAnimalAtPosition(position, 0);
                return newPosition;
            }

            if (CanEatPreyOrMoveToEmptyField(position))
            {
                newPosition = EatPreyOrMoveToEmptyField(position);
                if (currentEnergy % Configuration.SharkBreedTime == 0) BreedSharkToPosition(position);
            }

            SetAnimalAtPosition(newPosition, GetAnimalAtPosition(newPosition) + 1);

            return newPosition;
        }

        private Position PerformFishChronon(Position position)
        {
            if (!ContainsFish(position))
                throw new InvalidOperationException("Cannot perform action for non-fish cells.");

            var newPosition = position;
            var currentEnergy = GetAnimalAtPosition(position);
            var updatedEnergy = currentEnergy + 1;

            if (CanMoveToEmptyField(position))
            {
                newPosition = MoveToEmptyField(position);
                if (currentEnergy % Configuration.FishBreedTime == 0) BreedFishToPosition(position);
            }

            SetAnimalAtPosition(newPosition, updatedEnergy);

            return newPosition;
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
            var oldPositionAnimal = GetAnimalAtPosition(oldPosition);
            var newPositionAnimal = GetAnimalAtPosition(newPosition);

            SetAnimalAtPosition(newPosition, oldPositionAnimal - newPositionAnimal);
            SetAnimalAtPosition(oldPosition, 0);
        }

        private Position EatPrey(Position position)
        {
            if (!CanEatPrey(position)) throw new InvalidOperationException("Cannot eat prey.");

            var preyPosition = ChoosePreyInSurroundingField(position);
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
            SetAnimalAtPosition(position, -Configuration.SharkInitialEnergy);
        }

        private void BreedFishToPosition(Position position)
        {
            SetAnimalAtPosition(position, Configuration.FishInitialAge);
        }

        private bool ContainsFish(Position position)
        {
            return GetAnimalAtPosition(position) > 0;
        }

        private bool ContainsShark(Position position)
        {
            return GetAnimalAtPosition(position) < 0;
        }

        private bool IsEmpty(Position position)
        {
            return GetAnimalAtPosition(position) == 0;
        }

        private int GetAnimalAtPosition(Position position)
        {
            return Field[position.RowIndex, position.ColumnIndex];
        }

        private void SetAnimalAtPosition(Position position, int animal)
        {
            Field[position.RowIndex, position.ColumnIndex] = animal;
        }
    }
}