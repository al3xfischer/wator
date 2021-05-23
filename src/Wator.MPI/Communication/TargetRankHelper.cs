namespace Wator.MPI.Communication
{
    public class TargetRankHelper
    {
        /// <summary>
        /// Gets the lower process rank.
        /// </summary>
        /// <param name="currentRank">The current rank.</param>
        /// <param name="worldSize">Size of the world.</param>
        /// <returns></returns>
        public static int GetLowerProcessRank(int currentRank, int worldSize)
        {
            var result = (currentRank + 1) % worldSize;
            return result;
        }

        /// <summary>
        /// Gets the upper process rank.
        /// </summary>
        /// <param name="currentRank">The current rank.</param>
        /// <param name="worldSize">Size of the world.</param>
        /// <returns></returns>
        public static int GetUpperProcessRank(int currentRank, int worldSize)
        {
            var result = (currentRank - 1 + worldSize) % worldSize;
            return result;
        }
    }
}
