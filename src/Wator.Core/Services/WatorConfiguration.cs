namespace Wator.Core.Services
{
    public class WatorConfiguration
    {
        public int FishBreedTime { get; init; } = 10;
        public int SharkBreedTime { get; init; } = 25;
        public int SharkInitialEnergy { get; init; } = 20;
        public int FishInitialAge { get; init; } = 1;
        public int? Seed { get; set; }

        public int FishWorthEnergy { get; init; } = 20;
    }
}
