using System;
using Wator.Core.Services;

namespace Wator.Core.Helpers
{
    public class FieldBuilder
    {
        private int _columns = 1;
        private WatorConfiguration _configuration = new();
        private int _fishCount;
        private Random _random = new();
        private int _rows = 1;
        private int _sharkCount;

        /// <summary>
        /// Withes the dimensions.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// nameof(rows)
        /// or
        /// nameof(rows)
        /// </exception>
        public FieldBuilder WithDimensions(int rows, int columns)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns));

            _rows = rows;
            _columns = columns;
            return this;
        }

        /// <summary>
        /// Withes the seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public FieldBuilder WithSeed(int seed)
        {
            _random = new Random(seed);
            return this;
        }

        /// <summary>
        /// Withes the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public FieldBuilder WithConfiguration(WatorConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

        /// <summary>
        /// Withes the fish count.
        /// </summary>
        /// <param name="fishCount">The fish count.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">nameof(fishCount)</exception>
        public FieldBuilder WithFishCount(int fishCount)
        {
            if (fishCount < 0) throw new ArgumentOutOfRangeException(nameof(fishCount));

            _fishCount = fishCount;
            return this;
        }

        /// <summary>
        /// Withes the shark count.
        /// </summary>
        /// <param name="sharkCount">The shark count.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">nameof(sharkCount)</exception>
        public FieldBuilder WithSharkCount(int sharkCount)
        {
            if (sharkCount < 0) throw new ArgumentOutOfRangeException(nameof(sharkCount));

            _sharkCount = sharkCount;
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Cannot place </exception>
        public int[,] Build()
        {
            if (_fishCount + _sharkCount > _rows * _columns) throw new InvalidOperationException("Cannot place ");

            var field = new int[_rows, _columns];
            RandomlyPlaceAnimalsToField(field);
            return field;
        }

        private void RandomlyPlaceAnimalsToField(int[,] field)
        {
            RandomlyPlaceToField(field, _sharkCount, CreateShark);
            RandomlyPlaceToField(field, _fishCount, CreateFish);
        }

        private void RandomlyPlaceToField(int[,] field, int count, Func<int> createFunc)
        {
            while (count > 0)
            {
                var rowIndex = _random.Next(_rows);
                var columnIndex = _random.Next(_columns);

                if (field[rowIndex, columnIndex] != 0) continue;
                field[rowIndex, columnIndex] = createFunc();
                count--;
            }
        }

        private int CreateFish()
        {
            return _configuration.FishInitialAge;
        }

        private int CreateShark()
        {
            return -_configuration.SharkInitialEnergy;
        }
    }
}