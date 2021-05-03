using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Wator.MPI.Tests
{
    public class ProcessingUnitTarget
    {
        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void CalculateUpperTarget(int currentRank, int expectedTargetRank)
        {
            var size = 4;
            var actual = (currentRank - 1 + size) % size;
            var result = actual == 0 ? size - 1 : actual;
            Assert.Equal(result, expectedTargetRank);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 1)]
        public void CalculateLowerTarget(int currentRank, int expectedTargetRank)
        {
            var size = 4;
            var actual = (currentRank + 1 + size) % size;
            var result = actual == 0 ? 1 : actual;
            Assert.Equal(result, expectedTargetRank);
        }
    }
}
