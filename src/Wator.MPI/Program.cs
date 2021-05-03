using System;
using System.Net.Mail;
using MPI;
using Wator.MPI;
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

                if (comm.Rank == 0)
                {
                    // Render
                    // Send sub fields to processors.
                    // ...
                    // Receive and combine sub fields from processors.
                }
                else if (comm.Rank == 1)
                {
                    // receive from master

                    // Calculate non border fields

                    // Send top border to top processor
                    // Receive updated top border from top processor
                    // Copy updated to local

                    // Send bottom border to bottom processor
                    // Receive updated bottom border from bottom processor
                    // Copy updated to local

                    comm.Send($"Hi from {comm.Rank}", 3, 0);
                    var received = comm.Receive<string>(Communicator.anySource, 0);
                    Console.WriteLine(received);

                    // Send to master
                }
                else if (comm.Rank == 2)
                {
                    comm.Send($"Hi from {comm.Rank}", 1, 0);
                    var received = comm.Receive<string>(Communicator.anySource, 0);
                    Console.WriteLine(received);
                }
                else if (comm.Rank == 3)
                {
                    var received = comm.Receive<string>(Communicator.anySource, 0);
                    comm.Send($"Hi from {comm.Rank}", 2, 0);
                    Console.WriteLine(received);
                }

            }
        }
    }
}