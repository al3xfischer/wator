using MPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;
using Wator.MPI.Communication;
using Environment = MPI.Environment;

namespace wator.mpi
{
    internal class Program
    {
        public const int Rows = 10_000;
        public const int Columns = 10_000;

        public const int FishCount = 100_000;
        public const int SharkCount = 100_000;

        public const int Iterations = 20;

        private static void Main(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;
                var subfields = new int[comm.Size][,];
                int[,] field = new int[0, 0];
                var config = new WatorConfiguration();
                //Console.WriteLine(comm.Rank);
                if (IsMaster())
                {
                    (field, _) = CreateField();
                }

                var stopwatch = new Stopwatch();
                for (var iteration = 0; iteration < Iterations; iteration++)
                {
                    int[][,] subFields = new int[comm.Size][,];
                    if (IsMaster()) subFields = FieldHelper.Split(field, comm.Size);
                    var subfield = comm.Scatter(subFields, 0);
                    //Console.WriteLine("got sub");
                    if (IsMaster()) stopwatch.Start();
                    var subResult = ProcessIterion(config, subfield, comm);
                    //Console.WriteLine("calculated sub");
                    var results = comm.Gather(subResult, 0);
                    //Console.WriteLine("gathered subs");

                    if (results is null) continue;

                    field = FieldHelper.Merge(results);
                    if (IsMaster())
                    {
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                        stopwatch.Reset();
                    }

                    //Console.WriteLine("merged subs");
                    //Console.WriteLine(field);
                    //RenderField(field);
                    Console.WriteLine(iteration);
                }
            }
        }

        private static HashSet<Position> AddOffset(IEnumerable<Position> innerMoved, int offset)
        {
            return innerMoved.Select(element => element with { RowIndex = element.RowIndex + offset }).ToHashSet();
        }

        private static bool IsMaster()
        {
            return Communicator.world.Rank == 0;
        }

        public static (int[,], WatorConfiguration) CreateField()
        {
            var configuration = new WatorConfiguration { FishBreedTime = 1, Seed = 999 };
            var field = new FieldBuilder()
                .WithConfiguration(configuration)
                .WithSeed(101)
                .WithDimensions(Rows, Columns)
                .WithFishCount(FishCount)
                .WithSharkCount(SharkCount)
                .Build();

            return (field, configuration);
        }

        static void RenderField(int[,] field)
        {
            Console.Clear();

            //for (var i = 0; i < field.GetLength(0); i++)
            //{
            //    Console.Write($"{i:000}: ");
            //    for (var j = 0; j < field.GetLength(1); j++)
            //    {
            //        var cell = field[i, j];

            //        switch (cell?.Type)
            //        {
            //            case AnimalType.Shark:
            //                RenderShark();
            //                break;
            //            case AnimalType.Fish:
            //                RenderFish();
            //                break;
            //            default:
            //                RenderWater();
            //                break;
            //        }
            //    }

            //    Console.WriteLine();
            //}

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

        private static int[,] ProcessIterion(WatorConfiguration configuration, int[,] field, Intracommunicator comm)
        {
            //var args = new string[0];
            //using (new Environment(ref args))
            //{
            //RenderField(field);
            //Animal[][,] subFields = new Animal[comm.Size][,];
            // Master splits field to equal sub fields.
            //if (IsMaster()) subFields = FieldHelper.Split(field, comm.Size);

            // Distribute sub field to each process.
            //var subfield = comm.Scatter(subFields, 0);

            // Calculate non border fields
            var simulation = new WatorSimulation(field, configuration);
            var innerFrom = 1;
            var innerTo = field.GetLength(0) - 2;
            var moved = simulation.RunCycleInRows(innerFrom, innerTo);
            //RenderField(subfield);

            // last two rows are required
            var lowerBorder = FieldHelper.GetRows(field, field.GetLength(0) - 2, field.GetLength(0) - 1);
            var (lowerBorderFromUpperProcess, _) = comm.SendLowerReceiveUpper(lowerBorder, 0, 0);
            var movedOffset = AddOffset(moved, 2);
            var incluedLowerFromUpper = FieldHelper.MergeTwo(lowerBorderFromUpperProcess, field);
            //RenderField(incluedLowerFromUpper);
            simulation.Field = incluedLowerFromUpper;
            var topBorderMoved = simulation.RunCycleInRows(2, 2, movedOffset);
            //RenderField(incluedLowerFromUpper);


            // simulate upper border with new information
            var upperBorder = FieldHelper.GetRows(incluedLowerFromUpper, 0, 2); // bottom of upper subfield
            var ((myUpdatedLowerBorder, lowerMoved), _) = comm.SendUpperReceiveLower((upperBorder, topBorderMoved), 0, 0);
            var lowerMovedOffset = AddOffset(lowerMoved, incluedLowerFromUpper.GetLength(0) - 2);

            // remove last two rows and append the three from the lower process
            var splitIndex = incluedLowerFromUpper.GetLength(0) - 2;
            var removedLastTwo = FieldHelper.CutBefore(incluedLowerFromUpper, splitIndex)[0];
            var includedUpperFromLower = FieldHelper.MergeTwo(removedLastTwo, myUpdatedLowerBorder);
            //RenderField(includedUpperFromLower);

            var localLastRowIndex = includedUpperFromLower.GetLength(0) - 2;
            //RenderField(includedUpperFromLower);
            simulation.Field = includedUpperFromLower;
            movedOffset.UnionWith(lowerMovedOffset);
            simulation.RunCycleInRows(localLastRowIndex, localLastRowIndex, movedOffset);
            //RenderField(includedUpperFromLower);

            var resultSubfield = FieldHelper.GetRows(includedUpperFromLower, 2, includedUpperFromLower.GetLength(0) - 2);

            return resultSubfield;

            // Each process sends its updated subfield to the master.
            // Only the master process has an initialized results array all other processes receive null.
            //var results = comm.Gather(resultSubfield, 0);

            //if (results is null) return null;

            //return FieldHelper.Merge(results);
            //}
        }

        public static long MeasureRuntime(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static void ProcessIterations(WatorConfiguration configuration, int[,] field, Intracommunicator comm)
        {
            DrawProgressInPercent(0, Iterations);

            for (var i = 0; i < Iterations; i++)
            {
                //ProcessIteration(simulation, splitBoundaries);
                var iteratedField = ProcessIterion(configuration, field, comm);
                DrawProgressInPercent(i + 1, Iterations);
            }
        }

        public static void DrawProgressInPercent(int currentItem, int totalAmount)
        {
            Console.Clear();
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var progressInPercent = (double)currentItem / totalAmount;
            Console.WriteLine($"{Math.Round(progressInPercent * 100, 2)}%");
            Console.SetCursorPosition(left, top);
        }



    }
}