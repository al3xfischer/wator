using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Services;

namespace Wator.Core.Helpers
{
    public class FieldBuilder
    {
        private readonly Random _random = new();
        private int _columns = 1;
        private WatorConfiguration _configuration = new();
        private int _fishCount;
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

            _sharkCount = sharkCount;
            return this;
        }

        public Animal[,] Build()
        {
            if (_fishCount + _sharkCount > _rows * _columns) throw new InvalidOperationException("Cannot place ");

            var field = new Animal[_rows, _columns];
            RandomlyPlaceAnimalsToField(field);
            return field;
        }

        private void RandomlyPlaceAnimalsToField(Animal[,] field)
        {
            RandomlyPlaceToField(field, _sharkCount, CreateShark);
            RandomlyPlaceToField(field, _fishCount, CreateFish);
        }

        private void RandomlyPlaceToField(Animal[,] field, int count, Func<Animal> createFunc)
        {
            while (count > 0)
            {
                var rowIndex = _random.Next(_rows);
                var columnIndex = _random.Next(_columns);

                if (field[rowIndex, columnIndex] is not null) continue;
                field[rowIndex, columnIndex] = createFunc();
                count--;
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
    }
}