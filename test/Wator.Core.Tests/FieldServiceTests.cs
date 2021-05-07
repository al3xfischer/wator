using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Services;
using Xunit;

namespace Wator.Core.Tests
{
    public class FieldServiceTests
    {
        [Fact]
        public void Two_Subfield_Can_Be_Merged()
        {
            var subOne = new Cell[10, 1];
            var subTwo = new Cell[10, 1];
            subTwo[0, 0] = new Cell();

            var service = new FieldService();
            var actual = service.MergeTwo(subOne, subTwo);

            var expected = subOne.Length + subTwo.Length;

            Assert.True(actual.Length == expected);
        }

        [Theory]
        [InlineData(10, 10, 3, 3, 4)]
        public void Split_Into_Subfields(int width, int heigth, int partCount, int notLastLength, int lastLength)
        {
            var field = new Cell[width, heigth];
            var service = new FieldService();
            var actual = service.Split(partCount, field);

            Assert.True(actual[0..^2].All(sub => sub.GetLength(1) == notLastLength));
            Assert.True(actual[^1].GetLength(1) == lastLength);
        }
    }
}
