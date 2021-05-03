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
            var size = 3;
            var actual = (currentRank + 1) % size + 1;
            Assert.Equal(actual, expectedTargetRank);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 1)]
        public void CalculateLowerTarget(int currentRank, int expectedTargetRank)
        {
            var size = 3;
            var actual = (currentRank % size) + 1;
            Assert.Equal(actual, expectedTargetRank);
        }
    }
}
