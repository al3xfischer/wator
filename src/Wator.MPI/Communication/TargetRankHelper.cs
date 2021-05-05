namespace Wator.MPI.Communication
{
    public class TargetRankHelper
    {
        public static int GetMasterProcessRank() => 0;

        public static int GetLowerProcessRank(int currentRank, int worldSize)
        {
            var result = (currentRank + 1) % worldSize;
            return result;
        }

        public static int GetUpperProcessRank(int currentRank, int worldSize)
        {
            var result = (currentRank - 1 + worldSize) % worldSize;
            return result;
        }
    }
}
