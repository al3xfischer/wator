﻿using System;
using System.Net.Mail;
using System.Runtime.CompilerServices;
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

                if (IsMaster())
                {
                    // Render
                    // Send sub fields to processors.
                    // ...
                    // Receive and combine sub fields from processors.
                }
                else
                {
                    var client = new ProcessingUnitClient();

                    // receive from master

                    // Calculate non border fields

                    var myLowerBorder = $"lower border of {comm.Rank}";

                    var (lowerBorderFromUpperProcess, _) = client.SendLowerReceiveUpper(myLowerBorder, 0,0);
                    Console.WriteLine(lowerBorderFromUpperProcess);

                    var updatedLowerBorderFromUpperProcess = $"updated {lowerBorderFromUpperProcess}";
                    var (myUpdatedLowerBorder, _) = client.SendUpperReceiveLower(updatedLowerBorderFromUpperProcess, 0, 0);
                    Console.WriteLine(myUpdatedLowerBorder);
                    // Update my current field with received myUpdatedLowerBorder.

                    // Send to master
                }
            }
        }

        private static bool IsMaster()
        {
            return Communicator.world.Rank == 0;
        }
    }
}