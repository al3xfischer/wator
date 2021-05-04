using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wator.MPI.Communication
{
    public class TargetRankHelper
    {
        public static int GetNextProcessRank(int currentRank, int worldSize)
        {
            var intermediateResult = (currentRank + 1) % worldSize;
            var result = intermediateResult == 0 ? 1 : intermediateResult;
            return result;
        }

        public static int GetPreviousProcessRank(int currentRank, int worldSize)
        {
            var intermediateResult = (currentRank - 1 + worldSize) % worldSize;
            var result = intermediateResult == 0 ? worldSize - 1 : intermediateResult;
            return result;
        }
    }
}
