namespace Wator.MPI.Communication
{
    public class TargetRankHelper
    {
        public static int GetMasterProcessRank() => 0;

        public static int GetLowerProcessRank(int currentRank, int worldSize)
        {
            var intermediateResult = (currentRank + 1) % worldSize;
            var result = intermediateResult == 0 ? 1 : intermediateResult;
            return result;
        }

        public static int GetUpperProcessRank(int currentRank, int worldSize)
        {
            var intermediateResult = (currentRank - 1 + worldSize) % worldSize;
            var result = intermediateResult == 0 ? worldSize - 1 : intermediateResult;
            return result;
        }
    }
}
