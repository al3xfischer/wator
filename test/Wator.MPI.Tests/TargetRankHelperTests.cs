using Wator.MPI.Communication;
using Xunit;

namespace Wator.MPI.Tests
{
    public class TargetRankHelperTests
    {
        [Theory]
        [InlineData(0, 4, 3)]
        [InlineData(1, 4, 0)]
        [InlineData(2, 4, 1)]
        [InlineData(3, 4, 2)]
        public void Can_Calculate_Upper_Rank_From_Current_Rank(int currentRank, int worldSize, int expectedTargetRank)
        {
            var actual = TargetRankHelper.GetUpperProcessRank(currentRank, worldSize);
            Assert.Equal(actual, expectedTargetRank);
        }

        [Theory]
        [InlineData(0, 4, 1)]
        [InlineData(1, 4, 2)]
        [InlineData(2, 4, 3)]
        [InlineData(3, 4, 0)]
        public void Can_Calculate_Lower_Rank_From_Current_Rank(int currentRank, int worldSize, int expectedTargetRank)
        {
            var actual = TargetRankHelper.GetLowerProcessRank(currentRank, worldSize);
            Assert.Equal(actual, expectedTargetRank);
        }
    }
}
