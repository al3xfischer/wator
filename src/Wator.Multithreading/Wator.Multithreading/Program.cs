using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;

namespace Wator.Multithreading
{
    public class Program
    {
        public const int FishCount = 10_000;
        public const int SharkCount = 10_000;

        public const int Rows = 10_000;
        public const int Columns = 10_000;

        public const int Iterations = 10;

        public const int ThreadCount = 32;

        public static void Main(string[] args)
        {
            var simulation = new WatorSimulation(CreateField());
            var splitBoundaries = FieldHelper.GetSplitBoundaries(simulation.Field, ThreadCount);

            var runtimeInMs = MeasureRuntime(() => ProcessIterations(simulation, splitBoundaries));

            Console.WriteLine($"Running {Iterations} iterations took {runtimeInMs}ms.");
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey(false);
        }

        public static long MeasureRuntime(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static void ProcessIterations(WatorSimulation simulation, (int, int)[] splitBoundaries)
        {
            for (var i = 0; i < Iterations; i++) ProcessIteration(simulation, splitBoundaries);
        }

        public static void ProcessIteration(WatorSimulation simulation, (int, int)[] splitBoundaries)
        {
            var ignoreInBorderProcessing = new IEnumerable<Position>[splitBoundaries.Length];
            Parallel.For(0, splitBoundaries.Length, i =>
            {
                var (fromRow, toRow) = splitBoundaries[i];
                var innerTopBorder = fromRow + 1;
                var outerTopBorder = toRow - 1;
                ignoreInBorderProcessing[i] = simulation.RunCycleInRows(innerTopBorder, outerTopBorder);
            });

            foreach (var (fromRow, toRow) in splitBoundaries)
            {
                simulation.RunCycleInRows(fromRow, fromRow);
                simulation.RunCycleInRows(toRow, toRow);
            }
        }

        public static Animal[,] CreateField()
        {
            return new FieldBuilder()
                .WithDimensions(Rows, Columns)
                .WithFishCount(FishCount)
                .WithSharkCount(SharkCount)
                .Build();
        }
    }
}