using System;
using System.Collections.Generic;
using System.Linq;

namespace Wator.Core.Entities
{
    public class Field
    {
        private readonly int _width;
        private readonly int _height;

        public Field(int width, int height)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("The width must be at least one.");
            }

            if (height < 1 )
            {
                throw new ArgumentOutOfRangeException("The height must be at least one.");
            }

            _width = width;
            _height = height;

            Cells = Enumerable.Range(0, _width * _height).Select(_ => new Cell()).ToArray();
        }
        public Cell[] Cells { get; set; }

        public ICollection<Cell> GetRow(int index)
        {
            var start = index * _width;
            var end = start + _width;

            return Cells[start..end];
        }
    }
}
