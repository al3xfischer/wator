using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Services;

namespace Wator.Core.Helpers
{
    public class FieldBuilder
    {
        private int _columns = 1;
        private WatorConfiguration _configuration = new();
        private int _fishCount;
        private readonly Random _random = new();
        private int _rows = 1;
        private int _sharkCount;

        public FieldBuilder WithDimensions(int rows, int columns)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns));

            _rows = rows;
            _columns = columns;
            return this;
        }

        public FieldBuilder WithConfiguration(WatorConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

        public FieldBuilder WithFishCount(int fishCount)
        {
            if (fishCount < 0) throw new ArgumentOutOfRangeException(nameof(fishCount));

            _fishCount = fishCount;
            return this;
        }

        public FieldBuilder WithSharkCount(int sharkCount)
        {
            if (sharkCount < 0) throw new ArgumentOutOfRangeException(nameof(sharkCount));

            _fishCount = sharkCount;
            return this;
        }

        public Animal[,] Build()
        {
            if (_fishCount * _sharkCount > _rows * _columns) throw new InvalidOperationException("Cannot place more animals than cells to field");

            var field = new Animal[_rows, _columns];
            RandomlyPlaceToField(field);
            return field;
        }

        private void RandomlyPlaceToField(Animal[,] field)
        {
            var emptyPositions = GetAllPositions();

            for (var i = 0; i < _fishCount; i++)
            {
                var randomIndex = _random.Next(emptyPositions.Count);
                var position = emptyPositions[randomIndex];
                emptyPositions.RemoveAt(randomIndex);
                field[position.RowIndex, position.ColumnIndex] = CreateFish();
            }

            for (var i = 0; i < _sharkCount; i++)
            {
                var randomIndex = _random.Next(emptyPositions.Count);
                var position = emptyPositions[randomIndex];
                emptyPositions.RemoveAt(randomIndex);
                field[position.RowIndex, position.ColumnIndex] = CreateShark();
            }
        }

        private Animal CreateFish()
        {
            return new() {Age = 0, Energy = _configuration.FishInitialEnergy, Type = AnimalType.Fish};
        }

        private Animal CreateShark()
        {
            return new() {Age = 0, Energy = _configuration.SharkInitialEnergy, Type = AnimalType.Shark};
        }

        private IList<Position> GetAllPositions()
        {
            var allPositions = from column in Enumerable.Range(0, _columns)
                from row in Enumerable.Range(0, _rows)
                select new Position(row, column);
            return allPositions.ToList();
        }
    }
}