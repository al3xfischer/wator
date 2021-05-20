using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;
using Wator.Rendering;

namespace Wator.Multithreading
{
    public class Program
    {
        public const string OutputDir = @".\images";
        public const bool DoRender = false;

        public const int FishCount = 1000_000;
        public const int SharkCount = 500_000;

        public const int Rows = 10_000;
        public const int Columns = 10_000;

        public const int Iterations = 20;

        public static readonly WatorConfiguration Config = new();

        public static void Main(string[] args)
        {
            if (!Directory.Exists(OutputDir)) Directory.CreateDirectory(OutputDir);

            var threadCount = Convert.ToInt32(args[0]);
            var simulation = new WatorSimulation(CreateField(), Config);
            var splitBoundaries = FieldHelper.GetSplitBoundaries(simulation.Field, threadCount);

            var runtimeInMs = MeasureRuntime(() => ProcessIterations(simulation, splitBoundaries));
            var averageRuntimeInMs = runtimeInMs / Iterations;

            Console.WriteLine($"Ran with {threadCount} threads.");
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

            var renderer = new BitmapRenderer();

            for (var i = 0; i < Iterations; i++)
            {
                if (DoRender) renderer.Render(@$"{OutputDir}\iteration_{i}.png", simulation.Field);

                ProcessIteration(simulation, splitBoundaries);
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

        private static void RenderField(int[,] field)
        {
            for (var i = 0; i < field.GetLength(0); i++)
            {
                for (var j = 0; j < field.GetLength(1); j++)
                {
                    var cell = field[i, j];

                    switch (cell)
                    {
                        case < 0:
                            RenderShark();
                            break;
                        case > 0:
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

        private static void RenderFish()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("F");
            Console.ResetColor();
        }

        private static void RenderShark()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("S");
            Console.ResetColor();
        }

        private static void RenderWater()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("0");
            Console.ResetColor();
        }
    }
}