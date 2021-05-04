using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Wator.MPI.Communication;

namespace Wator.MPI.Tests
{
    public class TargetRankHelperTests
    {
        [Theory]
        [InlineData(1, 3, 3)]
        [InlineData(2, 3, 1)]
        [InlineData(3, 3, 2)]
        public void Can_Calculate_Previous_Rank_From_Current_Rank(int currentRank, int worldSize, int expectedTargetRank)
        {
            var actual = TargetRankHelper.GetPreviousProcessRank(currentRank, worldSize);
            Assert.Equal(actual, expectedTargetRank);
        }

        [Theory]
        [InlineData(1, 3, 2)]
        [InlineData(2, 3, 3)]
        [InlineData(3, 3, 1)]
        public void Can_Calculate_Next_Rank_From_Current_Rank(int currentRank, int worldSize, int expectedTargetRank)
        {
            var actual = TargetRankHelper.GetNextProcessRank(currentRank, worldSize);
            Assert.Equal(actual, expectedTargetRank);
        }
    }
}
