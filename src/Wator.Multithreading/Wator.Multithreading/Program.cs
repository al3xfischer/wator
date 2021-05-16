﻿using System;
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
        public const int SharkCount = 100_000;

        public const int Rows = 10_000;
        public const int Columns = 10_000;

        public const int Iterations = 10;

        public const int ThreadCount = 32;

        public static void Main(string[] args)
        {
            var simulation = new WatorSimulation(CreateField());
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
            for (var i = 0; i < Iterations; i++) ProcessIteration(simulation, splitBoundaries);
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

            for (var i = 0; i < splitBoundaries.Length; i++)
            {
                var(fromRow, toRow) = splitBoundaries[i];
                var ignorePositions = ignoreInBorderProcessing[i];
                simulation.RunCycleInRows(fromRow, fromRow, ignorePositions);
                simulation.RunCycleInRows(toRow, toRow, ignorePositions);
            }
        }

        public static Animal[,] CreateField()
        {
            return new FieldBuilder()
                .WithConfiguration(new WatorConfiguration{ FishBreedTime = 1, Seed = 42})
                .WithSeed(101)
                .WithDimensions(Rows, Columns)
                .WithFishCount(FishCount)
                .WithSharkCount(SharkCount)
                .Build();
        }
    }
}