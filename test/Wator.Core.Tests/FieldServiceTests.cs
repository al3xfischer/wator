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
        public void Two_Subfield_Can_Be_Merged()
        {
            var subOne = new[,] { { 1, 2, 3 } };
            var subTwo = new[,] { { 4, 5, 6 } };

            var service = new FieldHelper();
            var actual = service.MergeTwo(subOne, subTwo);

            var expected = subOne.Length + subTwo.Length;

            Assert.True(actual.Length == expected);
        }

        [Fact]
        public void Multiple_Subfields_Can_Be_Merged()
        {
            var subOne = new[,] { { 1, 2, 3 } };
            var subTwo = new[,] { { 4, 5, 6 } };
            var subThree = new[,] { { 7, 8, 9 } };

            var subFields = new[] { subOne, subTwo, subThree };

            var service = new FieldHelper();
            var actual = service.Merge(subFields);

            var expected = subFields.Select(sub => sub.Length)
                .Aggregate(0, (x, y) => x + y);

            Assert.True(actual.Length == expected);
        }

        [Fact]
        public void Split_Into_Subfields()
        {
            var service = new FieldHelper();

            var completeField = new[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8},
                {9, 10, 11}
            };

            var actual = service.Split(completeField, 2);

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
            var service = new FieldHelper();

            var completeField = new[,]
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };

            var actual = service.Split(completeField, 2);

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

            var actual = FieldService.GetSurroundingFields((1, 1), completeField);
            var expected = new List<(int, int)> { (0, 1), (1, 2), (2, 1), (1, 0) };

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

            var actual = FieldService.GetSurroundingFields((0, 0), completeField);
            var expected = new List<(int, int)> { (2, 0), (0, 1), (1, 0), (0, 2) };

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

            var actual = FieldService.GetSurroundingFields((2, 2), completeField);
            var expected = new List<(int, int)> { (1, 2), (2, 0), (0, 2), (2, 1) };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Shark_Eats_Nearby_Fish()
        {
            var shark = new Animal { Type = AnimalType.Shark, Age = 5, Energy = 3 };
            var fish = new Animal { Type = AnimalType.Fish, Age = 2, Energy = 2 };
            var field = new Animal[,] {
                {null, null,shark},
                {null, null,null },
                {null, null,fish }
            };

            var actualChanges = FieldService.RunCycleInRows(0, 2, field);

            var expectedChanges = new List<(int, int)> {
                (2,2)
            };

            var expectedField = new Animal[,] {
                {null, null,null},
                {null, null,null },
                {null, null,shark }
            };

            Assert.Equal(expectedField, field);
            Assert.Equal(expectedChanges, actualChanges);
            Assert.Equal(6, shark.Age);
            Assert.Equal(4, shark.Energy);
        }

        [Fact]
        public void Shark_Hits_Zero_Energy_And_Dies()
        {
            var shark = new Animal { Type = AnimalType.Shark, Age = 5, Energy = 0 };
            var field = new Animal[,] {
                {null, null,shark},
                {null, null,null },
                {null, null,null }
            };

            var actualChanges = FieldService.RunCycleInRows(0, 2, field);

            var expectedChanges = Enumerable.Empty<(int, int)>();

            var expectedField = new Animal[,] {
                {null, null,null},
                {null, null,null },
                {null, null,null }
            };

            Assert.Equal(expectedField, field);
            Assert.Equal(expectedChanges, actualChanges);
        }

        [Fact]
        public void Fish_Moves_One_Step()
        {
            var fish = new Animal { Type = AnimalType.Fish, Age = 2, Energy = 2 };
            var field = new Animal[,] {
                {null, null,null},
                {null, null,null },
                {null, null,fish }
            };

            var actualChanges = FieldService.RunCycleInRows(0, 2, field);

            Assert.DoesNotContain((2, 2), actualChanges);
        }

    }
}