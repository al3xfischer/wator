using System.Threading.Tasks;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;

namespace Wator.Multithreading
{
    public class Program
    {
        public const int FishCount = 15;
        public const int SharkCount = 15;
        public const int Rows = 10;
        public const int Columns = 10;

        public const int ThreadCount = 1;

        public static void Main(string[] args)
        {
            var simulation = new WatorSimulation(CreateField());
            var splitIndices = FieldHelper.GetSplitIndices(simulation.Field, ThreadCount);

            Parallel.For(0, ThreadCount, i =>
            {
                var workerIndices = splitIndices[i];
                var innerTopBorder = workerIndices.FromRow + 1;
                var outerTopBorder = workerIndices.ToRow - 1;
                simulation.RunCycleInRows(innerTopBorder, outerTopBorder);
            });
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
