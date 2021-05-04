using System;
using System.Net.Mail;
using MPI;
using Wator.MPI;
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
                var client = new ProcessingUnitClient();

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

                    var (message, _) = client.ReceiveFromPreviousProcess<string>(0);
                    client.SendToNextProcess($"Hi from {comm.Rank}", 0);
                    Console.WriteLine(message);

                    // Send to master
                }
                else
                {
                    client.SendToNextProcess($"Hi from {comm.Rank}", 0);
                    var (message, _) = client.ReceiveFromPreviousProcess<string>(0);
                    Console.WriteLine(message);
                }
            }
        }
    }
}