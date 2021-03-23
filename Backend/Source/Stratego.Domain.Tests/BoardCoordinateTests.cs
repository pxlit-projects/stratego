using System;
using System.Collections.Generic;
using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Domain.BoardDomain;

namespace Stratego.Domain.Tests
{
    public class BoardCoordinateTests
    {
        private static readonly Random RandomGenerator = new Random();

        [SetUp]
        public void Setup()
        {
            
        }

        [MonitoredTest("Constructors - Should initialize properly")]
        public void Constructors_ShouldInitializeProperly()
        {
            //Act
            var defaultCoordinate = new BoardCoordinate();
            int row = RandomGenerator.Next(1, 8);
            int column = RandomGenerator.Next(1, 8);
            var randomCoordinate = new BoardCoordinate(row, column);

            //Assert
            Assert.That(defaultCoordinate.Row, Is.EqualTo(0),
                "The 'Row' property should be 0 after using the default constructor.");
            Assert.That(defaultCoordinate.Column, Is.EqualTo(0),
                "The 'Column' property should be 0 after using the default constructor.");
            Assert.That(randomCoordinate.Row, Is.EqualTo(row),
                $"The 'Row' property should be {row} when the cow parameter of the constructor is {row}.");
            Assert.That(randomCoordinate.Column, Is.EqualTo(column),
                $"The 'Column' property should be {column} when the column parameter of the constructor is {column}.");

        }

        [MonitoredTest("IsOutOfBounds - Should return true when coordinate is not within board")]
        [TestCase(-1, 0, 8, true)]
        [TestCase(0, -1, 8, true)]
        [TestCase(8, 0, 8, true)]
        [TestCase(0, 8, 8, true)]
        [TestCase(3, 4, 8, false)]
        [TestCase(0, 0, 1, false)]
        [TestCase(1, 1, 1, true)]
        public void IsOutOfBounds_ShouldReturnTrueWhenCoordinateIsNotWithinBoard(int row, int column, int gridSize, bool expected)
        {
            BoardCoordinate coordinate = new BoardCoordinate(row, column);
            Assert.That(coordinate.IsOutOfBounds(gridSize), Is.EqualTo(expected),
                $"Coordinate ({row},{column}) should result in '{expected}'");
        }

        [MonitoredTest("GetStraightPathTo - Left to right - Should return correct path")]
        public void GetStraightPathTo_LeftToRight_ShouldReturnCorrectPath()
        {
            //Arrange
            BoardCoordinate start = new BoardCoordinate(RandomGenerator.Next(0, 3), RandomGenerator.Next(0, 3));
            BoardCoordinate target = new BoardCoordinate(start.Row, start.Column + RandomGenerator.Next(2,6));

            var expectedPath = new List<BoardCoordinate>();
            for (int column = start.Column + 1; column <= target.Column; column++)
            {
                expectedPath.Add(new BoardCoordinate(start.Row, column));
            }

            //Act
            IList<BoardCoordinate> path = start.GetStraightPathTo(target);

            //Assert
            Assert.That(path, Is.Not.Null, "The returned path should not be null.");
            Assert.That(path, Is.EquivalentTo(expectedPath),
                $"The returned path from {start} to {target} is not the expected path.");
        }

        [MonitoredTest("GetStraightPathTo - Right to left - Should return correct path")]
        public void GetStraightPathTo_RightToLeft_ShouldReturnCorrectPath()
        {
            //Arrange
            BoardCoordinate start = new BoardCoordinate(RandomGenerator.Next(5, 8), RandomGenerator.Next(5, 8));
            BoardCoordinate target = new BoardCoordinate(start.Row, start.Column - RandomGenerator.Next(2, 6));

            var expectedPath = new List<BoardCoordinate>();
            for (int column = start.Column - 1; column >= target.Column; column--)
            {
                expectedPath.Add(new BoardCoordinate(start.Row, column));
            }

            //Act
            IList<BoardCoordinate> path = start.GetStraightPathTo(target);

            //Assert
            Assert.That(path, Is.Not.Null, "The returned path should not be null.");
            Assert.That(path, Is.EquivalentTo(expectedPath),
                $"The returned path from {start} to {target} is not the expected path.");
        }

        [MonitoredTest("GetStraightPathTo - Up to down - Should return correct path")]
        public void GetStraightPathTo_UpToDown_ShouldReturnCorrectPath()
        {
            //Arrange
            BoardCoordinate start = new BoardCoordinate(RandomGenerator.Next(0, 3), RandomGenerator.Next(0, 3));
            BoardCoordinate target = new BoardCoordinate(start.Row + RandomGenerator.Next(2, 6), start.Column);

            var expectedPath = new List<BoardCoordinate>();
            for (int row = start.Row + 1; row <= target.Row; row++)
            {
                expectedPath.Add(new BoardCoordinate(row, start.Column));
            }

            //Act
            IList<BoardCoordinate> path = start.GetStraightPathTo(target);

            //Assert
            Assert.That(path, Is.Not.Null, "The returned path should not be null.");
            Assert.That(path, Is.EquivalentTo(expectedPath),
                $"The returned path from {start} to {target} is not the expected path.");
        }

        [MonitoredTest("GetStraightPathTo - Down to up - Should return correct path")]
        public void GetStraightPathTo_DownToUp_ShouldReturnCorrectPath()
        {
            //Arrange
            BoardCoordinate start = new BoardCoordinate(RandomGenerator.Next(5, 8), RandomGenerator.Next(5, 8));
            BoardCoordinate target = new BoardCoordinate(start.Row - RandomGenerator.Next(2, 6), start.Column);

            var expectedPath = new List<BoardCoordinate>();
            for (int row = start.Row - 1; row >= target.Row; row--)
            {
                expectedPath.Add(new BoardCoordinate(row, start.Column));
            }

            //Act
            IList<BoardCoordinate> path = start.GetStraightPathTo(target);

            //Assert
            Assert.That(path, Is.Not.Null, "The returned path should not be null.");
            Assert.That(path, Is.EquivalentTo(expectedPath),
                $"The returned path from {start} to {target} is not the expected path.");
        }

        [MonitoredTest("GetStraightPathTo - No straight path possible - Should return an empty list")]
        public void GetStraightPathTo_NoStraightPathPossible_ShouldReturnAnEmptyList()
        {
            //Arrange
            BoardCoordinate start = new BoardCoordinate(RandomGenerator.Next(0, 3), RandomGenerator.Next(0, 3));
            BoardCoordinate target = new BoardCoordinate(start.Row + RandomGenerator.Next(1, 6), start.Column + RandomGenerator.Next(1, 6));

            //Act
            IList<BoardCoordinate> path = start.GetStraightPathTo(target);

            //Assert
            Assert.That(path, Is.Not.Null, "The returned path should not be null.");
            Assert.That(path, Is.Empty, "The returned path should be empty");
        }
    }
}