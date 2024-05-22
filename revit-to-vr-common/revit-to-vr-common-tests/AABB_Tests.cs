using NUnit.Framework;
using NUnit;
using revit_to_vr_common;

namespace revit_to_vr_common.Tests
{
    [TestFixture]
    public class AABBTests
    {
        [Test]
        public void TestFromMinMax_Case1()
        {
            // Arrange
            Vector3 min = new Vector3 { x = 1, y = 2, z = 3 };
            Vector3 max = new Vector3 { x = 7, y = 8, z = 9 };

            // Act
            AABB aabb = AABB.FromMinMax(min, max);

            // Assert
            Assert.That(new Vector3 { x = 4, y = 5, z = 6 }, Is.EqualTo(aabb.center));
            Assert.That(new Vector3 { x = 3, y = 3, z = 3 }, Is.EqualTo(aabb.extents));
        }

        [Test]
        public void TestFromMinMax_Case2()
        {
            // Arrange
            Vector3 min = new Vector3 { x = -1, y = -2, z = -3 };
            Vector3 max = new Vector3 { x = 1, y = 2, z = 3 };

            // Act
            AABB aabb = AABB.FromMinMax(min, max);

            // Assert
            Assert.That(new Vector3 { x = 0, y = 0, z = 0 }, Is.EqualTo(aabb.center));
            Assert.That(new Vector3 { x = 1, y = 2, z = 3 }, Is.EqualTo(aabb.extents));
        }

        [Test]
        public void TestFromMinMax_Case3()
        {
            // Arrange
            Vector3 min = new Vector3 { x = 0, y = 0, z = 0 };
            Vector3 max = new Vector3 { x = 10, y = 20, z = 30 };

            // Act
            AABB aabb = AABB.FromMinMax(min, max);

            // Assert
            Assert.That(new Vector3 { x = 5, y = 10, z = 15 }, Is.EqualTo(aabb.center));
            Assert.That(new Vector3 { x = 5, y = 10, z = 15 }, Is.EqualTo(aabb.extents));
        }

        [Test]
        public void TestFromMinMax_Case4()
        {
            // Arrange
            Vector3 min = new Vector3 { x = -5, y = -10, z = -15 };
            Vector3 max = new Vector3 { x = 5, y = 10, z = 15 };

            // Act
            AABB aabb = AABB.FromMinMax(min, max);

            // Assert
            Assert.That(new Vector3 { x = 0, y = 0, z = 0 }, Is.EqualTo(aabb.center));
            Assert.That(new Vector3 { x = 5, y = 10, z = 15 }, Is.EqualTo(aabb.extents));
        }
    }
}
