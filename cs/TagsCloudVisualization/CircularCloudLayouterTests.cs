using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouterTests
    {
        private CircularCloudLayouter layouter;
        private Point center;

        [SetUp]
        public void SetUp()
        {
            center = new Point(0, 0);
            layouter = new CircularCloudLayouter(center);
        }

        [TestCase(0, 0, TestName = "Zero coordinates")]
        [TestCase(-10, -10, TestName = "Negative coordinates")]
        [TestCase(100500, 100500, TestName = "Big positive coordinates")]
        public void Constructor_ShouldNotThrowArgumentException_OnCorrectInput(int x, int y)
        {
            Assert.DoesNotThrow(() => new CircularCloudLayouter(new Point(x, y)));
        }

        [Test]
        public void PutNextRectangle_ShouldReturnRectangleWithCorrectSize()
        {
            var size = new Size(1000, 10000);
            var rectangle = layouter.PutNextRectangle(size);
            rectangle.Size.Should().Be(size);
        }

        [Test]
        public void PutNextRectangle_ShouldPlaceFirstRectangleInCenter()
        {
            layouter.PutNextRectangle(new Size(8, 8))
                .Should()
                .BeEquivalentTo(new Rectangle(new Point(center.X - 4, center.Y - 4), new Size(8, 8)));
        }

        [TestCase(1, -3, TestName = "Y < 0")]
        [TestCase(-3, 1, TestName = "X < 0")]
        [TestCase(-3, -3, TestName = "X < 0, Y < 0")]
        public void PutNextRectangle_ShouldThrowArgumentException_OnIncorrectInput(int x, int y)
        {
            Assert.Throws<ArgumentException>(() => layouter.PutNextRectangle(new Size(x, y)));
        }

        [TestCase(0, 3, TestName = "X = 0, Y > 0")]
        [TestCase(3, 0, TestName = "X > 0, Y = 0")]
        [TestCase(0, 0, TestName = "X = 0, Y = 0")]
        [TestCase(10000, 10000, TestName = "Big rectangle")]
        public void PutNextRectangle_ShouldNotThrowException_OnCorrectInput(int x, int y)
        {
            Assert.DoesNotThrow(() => layouter.PutNextRectangle(new Size(x, y)));
        }

        [Test]
        public void PutNextRectangle_ShouldNotIntersectRectangles()
        {
            var rnd = new Random();
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < 100; i++)
            {
                var rectangle = layouter.PutNextRectangle(new Size(rnd.Next(1, 10), rnd.Next(1, 5)));
                rectangles.Where(x => x.IntersectsWith(rectangle)).ToList().Should().BeEmpty();
                rectangles.Add(rectangle);
            }
        }
    }
}