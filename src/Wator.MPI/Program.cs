using MPI;
using System;
using System.Collections.Generic;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;
using Wator.MPI.Communication;
using Environment = MPI.Environment;

namespace wator.mpi
{
    internal class Program
    {
        public const int Rows = 10;
        public const int Columns = 10;

        public const int FishCount = 5;
        public const int SharkCount = 5;

        private static void Main(string[] args)
        {
            var configuration = new WatorConfiguration();
            var field = CreateField();
            using (new Environment(ref args))
            {
                RenderField(field);
                var comm = Communicator.world;
                Animal[][,] subFields = new Animal[comm.Size][,];
                // Master splits field to equal sub fields.
                if (IsMaster()) subFields = FieldHelper.Split(field, comm.Size);

                // Distribute sub field to each process.
                var subfield = comm.Scatter(subFields, 0);

                // Calculate non border fields
                var simulation = new WatorSimulation(subfield, configuration);
                var innerFrom = 1;
                var innerTo = subfield.GetLength(0) - 2;
                simulation.RunCycleInRows(innerFrom, innerTo);
                var innerSubfield = FieldHelper.GetRows(subfield, innerFrom, innerTo);

                // last two rows are required
                var lowerBorder = FieldHelper.GetRows(field, subfield.GetLength(0) - 2, subfield.GetLength(0) - 1);
                var (lowerBorderFromUpperProcess, _) = comm.SendLowerReceiveUpper(lowerBorder, 0, 0);

                var incluedLowerFromUpper = FieldHelper.MergeTwo(lowerBorderFromUpperProcess, subfield);
                simulation.Field = incluedLowerFromUpper;
                simulation.RunCycleInRows(2, 2);


                // simulate upper border with new information

                var upperBorder = FieldHelper.GetRows(incluedLowerFromUpper, 0, 2); // bottom of upper subfield
                var (myUpdatedLowerBorder, _) = comm.SendUpperReceiveLower(upperBorder, 0, 0);

                // remove last two rows and append the three from the lower process
                //TODO: implement the methods to do so
                var splitIndex = incluedLowerFromUpper.GetLength(0) - 2;
                var removedLastTwo = FieldHelper.CutBefore(incluedLowerFromUpper, splitIndex)[0];
                var includedUpperFromLower = FieldHelper.MergeTwo(removedLastTwo, myUpdatedLowerBorder);

                var localLastRowIndex = includedUpperFromLower.GetLength(0) - 2;
                simulation.Field = includedUpperFromLower;
                simulation.RunCycleInRows(localLastRowIndex, localLastRowIndex);

                var resultSubfield = FieldHelper.GetRows(includedUpperFromLower, 2, includedUpperFromLower.GetLength(0) - 2);

                // Each process sends its updated subfield to the master.
                // Only the master process has an initialized results array all other processes receive null.
                var results = comm.Gather(resultSubfield, 0);

                if (results is null) return;

                var simulatedSubfields = FieldHelper.Merge(results);

                RenderField(simulatedSubfields);

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
            //using (new Environment(ref args))
            //{
            //    var comm = Communicator.world;

            //    int[] pseudoField = null;

            //    // Master splits field to equal sub fields.
            //    if(IsMaster()) pseudoField = Enumerable.Range(0, comm.Size).ToArray();

            //    // Distribute sub field to each process.
            //    var myPseudoSubfield = comm.Scatter(pseudoField, 0);

            //    // Calculate non border fields

            //    var myLowerBorder = $"lower border of {comm.Rank}";

            //    var (lowerBorderFromUpperProcess, _) = comm.SendLowerReceiveUpper(myLowerBorder, 0, 0);
            //    Console.WriteLine(lowerBorderFromUpperProcess);

            //    var updatedLowerBorderFromUpperProcess = $"updated {lowerBorderFromUpperProcess}";
            //    var (myUpdatedLowerBorder, _) = comm.SendUpperReceiveLower(updatedLowerBorderFromUpperProcess, 0, 0);
            //    Console.WriteLine(myUpdatedLowerBorder);

            //    // Each process updates the subfield (master also acts as a worker).
            //    var myUpdatedPseudoSubfield = 2 * myPseudoSubfield;

            //    // Each process sends its updated subfield to the master.
            //    // Only the master process has an initialized results array all other processes receive null.
            //    var results = comm.Gather(myUpdatedPseudoSubfield, 0);

            //    // Master handles gathered updated subfields.
            //    if (IsMaster())
            //    {
            //        foreach (var result in results)
            //        {
            //            Console.WriteLine(result);
            //        }
            //    }
            //}
        }

        private static bool IsMaster()
        {
            return Communicator.world.Rank == 0;
        }

        public static Animal[,] CreateField()
        {
            return new FieldBuilder()
                .WithConfiguration(new WatorConfiguration { FishBreedTime = 1, Seed = 42 })
                .WithSeed(101)
                .WithDimensions(Rows, Columns)
                .WithFishCount(FishCount)
                .WithSharkCount(SharkCount)
                .Build();
        }

        public static IEnumerable<Animal> ToRow(Animal[,] subfield)
        {
            if (subfield.GetLength(0) != 1)
            {
                throw new ArgumentException("No able to tranform an (2-n)D array mith more than one row.");
            }

            for (var i = 0; i < subfield.GetLength(1); i++) yield return subfield[0, i];
        }
        static void RenderField(Animal[,] field)
        {
            for (var i = 0; i < field.GetLength(0); i++)
            {
                for (var j = 0; j < field.GetLength(1); j++)
                {
                    var cell = field[i, j];

                    switch (cell?.Type)
                    {
                        case AnimalType.Shark:
                            RenderShark();
                            break;
                        case AnimalType.Fish:
                            RenderFish();
                            break;
                        default:
                            RenderWater();
                            break;
                    }
                }

                Console.WriteLine();
            }

            Console.SetCursorPosition(0, 0);
        }

        static void RenderFish()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("F");
            Console.ResetColor();
        }

        static void RenderShark()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("S");
            Console.ResetColor();
        }

        static void RenderWater()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("0");
            Console.ResetColor();
        }
    }
}