using MPI;
using System;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Interfaces;
using Wator.Core.Services;
using Wator.MPI.Communication;
using Environment = MPI.Environment;

namespace Wator.MPI
{
    public class ProcessingUnit : IProcessingUnit
    {
        private readonly Intracommunicator _comm;

        public ProcessingUnit(Intracommunicator communicator)
        {
            _comm = communicator ?? throw new ArgumentNullException(nameof(communicator));
        }

        public void Process(Animal[,] field, WatorConfiguration configuration = null)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            // Algorithm for processing unit:
            // wait for processing task from master process
            // Process field except borders
            // Send/Receive field from border process
            // If sender
            // send local border
            // receive updated border
            // update local border
            // Else
            // received local border
            // calculate updated borders
            // send updated border
            // Send complete result to master 

            //TODO: add args to config and use it for environment
            //I belive if we dont use the args we are not able to change the comm.size to anything other than 1
            //Not testet if we need the args 
            var args = new string[0];
            using (new Environment(ref args))
            {
                var comm = Communicator.world;
                Animal[][,] subFields = new Animal[comm.Size][,];
                // Master splits field to equal sub fields.
                if (IsMaster()) subFields = FieldHelper.Split(field, comm.Size);

                // Distribute sub field to each process.
                var subField = comm.Scatter(subFields, 0);

                // Calculate non border fields
                var simulation = new WatorSimulation(subField, configuration);
                simulation.RunCycleInRows(1, subField.GetLength(0) - 2);

                var lowerBorder = FieldHelper.GetRows(field, subField.GetLength(0) - 2, subField.GetLength(0) - 1);
                var (lowerBorderFromUpperProcess, _) = comm.SendLowerReceiveUpper(lowerBorder, 0, 0);

                // update local sub
                // create method to apply lowerBorderFromUpperProcess to local subfield
                //field = FieldHelper.ApplyChanges()j;

                // simulate upper border with new information

                var upperBorder = FieldHelper.GetRows(field, 0, 0);
                var (myUpdatedLowerBorder, _) = comm.SendUpperReceiveLower(upperBorder, 0, 0);

                // apply myUpdatedLowerBorder
                // calc lower border
                simulation.RunCycleInRows(subField.GetLength(0) - 1, subField.GetLength(0) - 1);


                // Each process sends its updated subfield to the master.
                // Only the master process has an initialized results array all other processes receive null.
                var results = comm.Gather(subField, 0);

                if (results is null) return;

                var simulatedSubfields = FieldHelper.Merge(results);

                // Master handles gathered updated subfields.
                // we may should return the simulatedSubfields to render then 
                //return simulatedSubfields;

                //if (IsMaster())
                //{
                //    foreach (var result in results)
                //    {
                //        Console.WriteLine(result);
                //    }
                //}
            }


        }

        private static bool IsMaster()
        {
            return Communicator.world.Rank == 0;
        }
    }
}
