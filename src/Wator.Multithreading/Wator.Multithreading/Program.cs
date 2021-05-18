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
        public const int FishCount = 100_000;
        public const int SharkCount = 50_000;

        public const int Rows = 10_000;
        public const int Columns = 10_000;
        public static readonly WatorConfiguration Config = new WatorConfiguration();

        public const int Iterations = 750;

        public const int ThreadCount = 32;

        public static void Main(string[] args)
        {
            var simulation = new WatorSimulation(CreateField(), Config);
            //Console.SetWindowSize(simulation.Field.GetLength(1) + 1, simulation.Field.GetLength(0) + 1);

            var splitBoundaries = FieldHelper.GetSplitBoundaries(simulation.Field, ThreadCount);

            var runtimeInMs = MeasureRuntime(() => ProcessIterations(simulation, splitBoundaries));
            var averageRuntimeInMs = runtimeInMs / Iterations;
            Console.WriteLine($"Running {Iterations} iterations took {runtimeInMs}ms.");
            Console.WriteLine($"Each iteration took {averageRuntimeInMs}ms on average.");
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
            DrawProgressInPercent(0, Iterations);

            for (var i = 0; i < Iterations; i++)
            {
                //RenderField(simulation.Field);
                ProcessIteration(simulation, splitBoundaries);
                //Thread.Sleep(750);
                DrawProgressInPercent(i + 1, Iterations);
            }
        }

        public static void ProcessIteration(WatorSimulation simulation, (int, int)[] splitBoundaries)
        {
            var ignoreInBorderProcessing = new HashSet<Position>[splitBoundaries.Length];
            Parallel.For(0, splitBoundaries.Length, i =>
            {
                var (fromRow, toRow) = splitBoundaries[i];
                var innerTopBorder = fromRow + 1;
                var outerTopBorder = toRow - 1;
                ignoreInBorderProcessing[i] = simulation.RunCycleInRows(innerTopBorder, outerTopBorder);
            });

            var ignoreAcc = ignoreInBorderProcessing.SelectMany(_ => _).ToHashSet();

            Parallel.For(0, splitBoundaries.Length, i =>
            {
                var (_, toRow) = splitBoundaries[i];
                var (nFromRow, _) = splitBoundaries[(i + 1) % splitBoundaries.Length];

                simulation.RunCycleInRows(toRow, nFromRow, ignoreAcc);
            });
        }

        public static int[,] CreateField()
        {
            return new FieldBuilder()
                .WithConfiguration(new WatorConfiguration {FishBreedTime = 1, Seed = 42})
                .WithSeed(101)
                .WithDimensions(Rows, Columns)
                .WithFishCount(FishCount)
                .WithSharkCount(SharkCount)
                .Build();
        }

        public static void DrawProgressInPercent(int currentItem, int totalAmount)
        {
            Console.Clear();
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var progressInPercent = (double) currentItem / totalAmount;
            Console.WriteLine($"{Math.Round(progressInPercent * 100, 2)}%");
            Console.SetCursorPosition(left, top);
        }
    }
}