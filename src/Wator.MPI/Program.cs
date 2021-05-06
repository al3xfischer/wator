using System;
using System.Linq;
using MPI;
using Wator.MPI.Communication;
using Environment = MPI.Environment;

namespace wator.mpi
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;

                int[] pseudoField = null;
                
                // Master splits field to equal sub fields.
                if(IsMaster()) pseudoField = Enumerable.Range(0, comm.Size).ToArray();

                // Distribute sub field to each process.
                var myPseudoSubfield = comm.Scatter(pseudoField, 0);

                // Calculate non border fields

                var myLowerBorder = $"lower border of {comm.Rank}";

                var (lowerBorderFromUpperProcess, _) = comm.SendLowerReceiveUpper(myLowerBorder, 0, 0);
                Console.WriteLine(lowerBorderFromUpperProcess);

                var updatedLowerBorderFromUpperProcess = $"updated {lowerBorderFromUpperProcess}";
                var (myUpdatedLowerBorder, _) = comm.SendUpperReceiveLower(updatedLowerBorderFromUpperProcess, 0, 0);
                Console.WriteLine(myUpdatedLowerBorder);

                // Each process updates the subfield (master also acts as a worker).
                var myUpdatedPseudoSubfield = 2 * myPseudoSubfield;

                // Each process sends its updated subfield to the master.
                // Only the master process has an initialized results array all other processes receive null.
                var results = comm.Gather(myUpdatedPseudoSubfield, 0);

                // Master handles gathered updated subfields.
                if (IsMaster())
                {
                    foreach (var result in results)
                    {
                        Console.WriteLine(result);
                    }
                }
            }
        }

        private static bool IsMaster()
        {
            return Communicator.world.Rank == 0;
        }
    }
}