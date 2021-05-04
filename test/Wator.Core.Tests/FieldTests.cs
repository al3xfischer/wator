using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wator.Core.Entities;
using Xunit;

namespace Wator.Core.Tests
{
    public class FieldTests
    {
        [Theory]
        [InlineData(10,10,100)]
        [InlineData(1,1,1)]
        [InlineData(2,2,4)]
        [InlineData(5,5,25)]
        public void Init_Cells_Amount(int width,int height,int expectedCells)
        {
            var field = new Field(width,height);
            Assert.Equal(field.Cells.Count(), expectedCells);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Width_Less_Than_One_Throws(int width)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Field(width, 1));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Height_Less_Than_One_Throws(int height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Field(1, height));
        }

        [Theory]
        [InlineData(1,1,0)]
        [InlineData(10,1,0)]
        [InlineData(10,10,5)]
        [InlineData(10,10,9)]
        public void GetRow_Correct_Amount(int width, int height, int row)
        {
            var field = new Field(width, height);
            var actual = field.GetRow(row);

            Assert.Equal(width, actual.Count);
        }

        public void GetRow_Correct_Cells()
        {

        }

        public static IEnumerable<object[]> GetCorrectCellsData(int numTests)
        {
        }
    }
}
