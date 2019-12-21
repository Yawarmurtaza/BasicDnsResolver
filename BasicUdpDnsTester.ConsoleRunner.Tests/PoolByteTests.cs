
using InfraServiceJobPackage.Library.DnsHelper.MessageWriters;
using NUnit.Framework;
using System;

namespace BasicUdpDnsTester.ConsoleRunner.Tests
{
    [TestFixture]
    public class PoolByteTests
    {
        [Test]
        public void BufferTest()
        {
            // Arrange.
            PooledBytes pool = new PooledBytes(3);

            // Act.

            byte[] buffer = pool.Buffer;

            // Assert.

            Assert.IsNotNull(buffer);
            Assert.AreEqual(16, buffer.Length);
        }

        [Test]
        public void BufferSegmentTest()
        {
            // Arrange.
            int count = 10;
            PooledBytes pool = new PooledBytes(count);

            // Act.
            ArraySegment<byte> segmentedArray = pool.BufferSegment;

            // Act.
            Assert.AreEqual(count, segmentedArray.Count);
        }



        [Test]
        public void TestDispose()
        {
            // Arrange.

            PooledBytes pool = new PooledBytes(10);

            // act.
            pool.Dispose();

            // assert.
            Assert.Throws<ObjectDisposedException>(() =>
            {
                ArraySegment<byte> buffer = pool.BufferSegment;
            });
        }
    }
}
