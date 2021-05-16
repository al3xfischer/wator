using System;

namespace Wator.Core.Entities
{
    [Serializable]
    public class Animal
    {
        public int Energy { get; set; }
        public int Age { get; set; }
        public AnimalType Type { get; set; }
    }
}