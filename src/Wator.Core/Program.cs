using System;
using Wator.Core.Entities;
using Wator.Core.Services;

var simulation = new WatorSimulation(new Animal[10, 20]);
var initFishCount = 15;
var initSharkCount = 5;
var cycleCount = 1000;

var random = new Random(5);

for (var i = 0; i < initFishCount; i++)
{
    var rowIndex = random.Next(0, simulation.Field.GetLength(0));
    var colIndex = random.Next(0, simulation.Field.GetLength(1));

    simulation.Field[rowIndex, colIndex] = new Animal { Age = 0, Energy = 1, Type = AnimalType.Fish };
}

for (var i = 0; i < initSharkCount; i++)
{
    var rowIndex = random.Next(0, simulation.Field.GetLength(0));
    var colIndex = random.Next(0, simulation.Field.GetLength(1));

    simulation.Field[rowIndex, colIndex] = new Animal { Age = 0, Energy = 3, Type = AnimalType.Shark };
}

for (var i = 0; i < cycleCount; i++)
{
    simulation.RunCycle();
    RenderField(simulation.Field);
    Console.ReadKey(false);
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