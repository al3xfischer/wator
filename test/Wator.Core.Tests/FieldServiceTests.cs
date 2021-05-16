using System;
using System.Collections.Generic;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Wator.Core.Services;
using Xunit;

namespace Wator.Core.Tests
{
    public class FieldServiceTests
    {
        [Fact]
        public void SanityTestSimulation()
        {
            var simulation = new WatorSimulation(new Animal[100, 100]);
            var initFishCount = 300;
            var initSharkCount = 300;
            var cycleCount = 10;

            var random = new Random(5);

            for (var i = 0; i < initFishCount; i++)
            {
                var rowIndex = random.Next(0, simulation.Field.GetLength(0));
                var colIndex = random.Next(0, simulation.Field.GetLength(1));

                simulation.Field[rowIndex, colIndex] = new Animal {Age = 0, Energy = 1, Type = AnimalType.Fish};
            }

            for (var i = 0; i < initSharkCount; i++)
            {
                var rowIndex = random.Next(0, simulation.Field.GetLength(0));
                var colIndex = random.Next(0, simulation.Field.GetLength(1));

                simulation.Field[rowIndex, colIndex] = new Animal {Age = 0, Energy = 5, Type = AnimalType.Shark};
            }

            for (var i = 0; i < cycleCount; i++) simulation.RunCycle();
        }

        [Fact]
        public void Two_Subfield_Can_Be_Merged()
        {
            var subOne = new[,] {{1, 2, 3}};
            var subTwo = new[,] {{4, 5, 6}};

            var actual = FieldHelper.MergeTwo(subOne, subTwo);

            var expected = subOne.Length + subTwo.Length;

            Assert.True(actual.Length == expected);
        }

        [Fact]
        public void Multiple_Subfields_Can_Be_Merged()
        {
            var subOne = new[,] {{1, 2, 3}};
            var subTwo = new[,] {{4, 5, 6}};
            var subThree = new[,] {{7, 8, 9}};

            var subFields = new[] {subOne, subTwo, subThree};

            var actual = FieldHelper.Merge(subFields);

            var expected = subFields.Select(sub => sub.Length)
                .Aggregate(0, (x, y) => x + y);

            Assert.True(actual.Length == expected);
        }

        [Fact]
        public void Split_Into_Subfields()
        {
            var completeField = new[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8},
                {9, 10, 11}
            };

            var actual = FieldHelper.Split(completeField, 2);

            var firstExpectedSubfield = new[,]
            {
                {0, 1, 2},
                {3, 4, 5}
            };

            var secondExpectedSubfield = new[,]
            {
                {6, 7, 8},
                {9, 10, 11}
            };

            Assert.Equal(firstExpectedSubfield, actual[0]);
            Assert.Equal(secondExpectedSubfield, actual[1]);
        }

        [Fact]
        public void Split_Into_Non_Dividable_Subfields()
        {
            var completeField = new[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };

            var actual = FieldHelper.Split(completeField, 2);

            var firstExpectedSubfield = new[,]
            {
                {0, 1, 2}
            };

            var secondExpectedSubfield = new[,]
            {
                {3, 4, 5},
                {6, 7, 8}
            };

            Assert.Equal(firstExpectedSubfield, actual[0]);
            Assert.Equal(secondExpectedSubfield, actual[1]);
        }

        [Fact]
        public void Sourrounding_Fields_Can_Be_Calculated()
        {
            var completeField = new sbyte[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };

            var actual = FieldHelper.GetSurroundingFields(completeField, new Position(1, 1));
            var expected = new List<Position> {new(0, 1), new(1, 2), new(2, 1), new(1, 0)};

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Sourrounding_Fields_Of_Top_Left_Position_Can_Be_Calculated()
        {
            var completeField = new sbyte[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };

            var actual = FieldHelper.GetSurroundingFields(completeField, new Position(0, 0));
            var expected = new List<Position> {new(2, 0), new(0, 1), new(1, 0), new(0, 2)};

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Sourrounding_Fields_Of_Bottom_Right_Position_Can_Be_Calculated()
        {
            var completeField = new sbyte[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };

            var actual = FieldHelper.GetSurroundingFields(completeField, new Position(2, 2));
            var expected = new List<Position> {new(1, 2), new(2, 0), new(0, 2), new(2, 1)};

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Shark_Eats_Nearby_Fish()
        {
            var shark = new Animal {Type = AnimalType.Shark, Age = 5, Energy = 3};
            var fish = new Animal {Type = AnimalType.Fish, Age = 2, Energy = 2};
            var field = new[,]
            {
                {null, null, shark},
                {null, null, null},
                {null, null, fish}
            };
            var simulation = new WatorSimulation(field);

            var actualChanges = simulation.RunCycle();

            var expectedChanges = new List<Position>
            {
                new(2, 2)
            };

            var expectedField = new[,]
            {
                {null, null, null},
                {null, null, null},
                {null, null, shark}
            };

            Assert.Equal(expectedField, field);
            Assert.Equal(expectedChanges, actualChanges);
            Assert.Equal(6, shark.Age);
            Assert.Equal(4, shark.Energy);
        }

        [Fact]
        public void Shark_Hits_Zero_Energy_And_Dies()
        {
            var shark = new Animal {Type = AnimalType.Shark, Age = 5, Energy = 1};
            var configuration = new WatorConfiguration {SharkBreedTime = 10};
            var field = new[,]
            {
                {null, null, shark},
                {null, null, null},
                {null, null, null}
            };
            var simulation = new WatorSimulation(field, configuration);

            simulation.RunCycle();
            simulation.RunCycle();

            Assert.Equal(0, simulation.SharkCount);
        }

        [Fact]
        public void Shark_Breeds_When_Reaching_BreedTime()
        {
            var fish = new Animal { Type = AnimalType.Shark, Age = 0, Energy = 50 };
            var configuration = new WatorConfiguration { SharkBreedTime = 2 };
            var field = new[,]
            {
                {null, null, null},
                {null, null, null},
                {null, null, fish}
            };

            var simulation = new WatorSimulation(field, configuration);

            simulation.RunCycle();
            simulation.RunCycle();

            Assert.Equal(2, simulation.SharkCount);
        }

        [Fact]
        public void Fish_Moves_One_Step()
        {
            var fish = new Animal {Type = AnimalType.Fish, Age = 2, Energy = 2};
            var field = new[,]
            {
                {null, null, null},
                {null, null, null},
                {null, null, fish}
            };
            var simulation = new WatorSimulation(field);

            var actualChanges = simulation.RunCycle();

            Assert.DoesNotContain(new Position(2, 2), actualChanges);
        }

        [Fact]
        public void Fish_Breeds_When_Reaching_BreedTime()
        {
            var fish = new Animal { Type = AnimalType.Fish, Age = 0, Energy = 5 };
            var configuration = new WatorConfiguration {FishBreedTime = 2};
            var field = new[,]
            {
                {null, null, null},
                {null, null, null},
                {null, null, fish}
            };

            var simulation = new WatorSimulation(field, configuration);
            
            simulation.RunCycle();
            simulation.RunCycle();

            Assert.Equal(2, simulation.FishCount);
        }
    }
}