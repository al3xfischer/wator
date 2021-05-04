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

            return (currentRank % (worldSize - 1)) + 1;
        }

        public static int GetPreviousProcessRank(int currentRank, int worldSize)
        {
            return (currentRank + 1) % (worldSize - 1) + 1;
        }
    }
}
