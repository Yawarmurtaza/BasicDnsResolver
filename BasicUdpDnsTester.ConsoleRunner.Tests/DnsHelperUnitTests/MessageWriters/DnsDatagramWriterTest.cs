using System;
using System.Text;
using InfraServiceJobPackage.Library.DnsHelper.MessageWriters;
using NUnit.Framework;

namespace InfraServiceJobPackageUnitTest.DnsHelperUnitTests.MessageWriters
{
    [TestFixture]
    public class DnsDatagramWriterTest
    {
        private IDnsDatagramWriter writer;
        private int index;

        [SetUp]
        public void Setup()
        {
            writer = new DnsDatagramWriter();
            index = 0;
        }

        [TearDown]
        public void TearDown()
        {
            writer = null;
            GC.Collect();
        }


        [Test]
        public void WriteHostName_should_write_query_name_in_the_data_array_using_utf8_encoding()
        {
            // Arrange.
            const string queryNameRoot = "www.microsoft.com.";

            // Act.
            writer.WriteHostName(queryNameRoot);

            // Assert.
            Assert.NotNull(writer.Data);
            string result = Encoding.UTF8.GetString(writer.Data.Array, 1, writer.Data.Count);
            Assert.AreEqual("www\tmicrosoft\u0003com\0\0", result);
        }

        [Test]
        public void WriteInt16NetworkOrder_should_write_value_in_the_data_array()
        {
            // Arrange.
            const short value = 45;
            // Act.

            writer.WriteInt16NetworkOrder(value);

            // not so 'Act'
            byte[] data = SubArray(writer.Data.Array, 0, writer.Data.Count);
            ArraySegment<byte> arrData = new ArraySegment<byte>(data);
            ushort result = Read(arrData); // (ushort)(ReadByte(arrData, 0) << 8 | ReadByte(arrData, 1));
            
            // Assert.
            Assert.AreEqual(value, result);
        }


        [Test]
        public void WriteUInt32NetworkOrder_should_write_value_in_the_data_array()
        {
            // Arrange.
            const uint value = 45;
            // Act.

            writer.WriteUInt32NetworkOrder(value);

            // not so 'Act'
            byte[] data = SubArray(writer.Data.Array, 0, writer.Data.Count);
            ArraySegment<byte> arrData = new ArraySegment<byte>(data);
            uint result = (uint)(Read(arrData) << 16 | Read(arrData));

            // Assert.
            Assert.AreEqual(value, result);
        }

        private ushort Read(ArraySegment<byte> arrData)
        {
            return (ushort)(ReadByte(arrData) << 8 | ReadByte(arrData));
        }


        private byte ReadByte(ArraySegment<byte> data)
        {
            byte requiredByte = data.Array[data.Offset + index++];
            return requiredByte;
        }

        public static ArrayType[] SubArray<ArrayType>(ArrayType[] data, int index, int length)
        {
            ArrayType[] newArray = new ArrayType[length];
            Array.Copy(data, index, newArray, 0, length);
            return newArray;
        }
    }
}
