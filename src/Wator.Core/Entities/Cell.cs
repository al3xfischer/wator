using Wator.Core.Interfaces;

namespace Wator.Core.Entities
{
    public record Cell
    {
        public IAnimal Animal { get; set; }
    }
}
