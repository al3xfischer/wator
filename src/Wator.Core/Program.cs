using System;
using Wator.Core.Helpers;
using Wator.Core.Services;

var field = new FieldBuilder()
    .WithFishCount(50)
    .WithSharkCount(50)
    .WithDimensions(50, 100)
    .Build();
var simulation = new WatorSimulation(field);

Console.SetWindowSize(simulation.Field.GetLength(1) + 1, simulation.Field.GetLength(0) + 1);

while (true)
{
    RenderField(simulation.Field);
    simulation.RunCycle();
}

static void RenderField(int[,] field)
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