using System;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Helpers;
using Xunit;

namespace Wator.Core.Tests
{
    public class FieldServiceTests
    {
        [Fact]
        public void Two_Subfield_Can_Be_Merged()
        {
            var subOne = new[,] {{1, 2, 3}};
            var subTwo = new[,] {{4, 5, 6}};

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

            var subFields = new[] {subOne, subTwo, subThree};

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
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 },
                { 9, 10, 11 },
            };

            var actual = service.Split(completeField, 2);

            var firstExpectedSubfield = new[,]
            {
                {0, 1, 2},
                {3, 4, 5},
            };

            var secondExpectedSubfield = new[,]
            {
                { 6, 7, 8 },
                { 9, 10, 11 },
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
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 },
            };

            var actual = service.Split(completeField, 2);

            var firstExpectedSubfield = new[,]
            {
                {0, 1, 2},
            };

            var secondExpectedSubfield = new[,]
            {
                {3, 4, 5},
                { 6, 7, 8 },
            };

            Assert.Equal(firstExpectedSubfield, actual[0]);
            Assert.Equal(secondExpectedSubfield, actual[1]);
        }
    }
}