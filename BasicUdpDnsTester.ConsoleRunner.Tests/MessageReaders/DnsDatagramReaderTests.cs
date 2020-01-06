using System.Net;
using System.Text;

namespace BasicUdpDnsTester.ConsoleRunner.Tests.MessageReaders
{
    using InfraServiceJobPackage.Library.DnsHelper.MessageReaders;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
    using Moq;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class DnsDatagramReaderTests : BaseTestHelper
    {
        private Mock<IDnsString> mockDnsString;
        //private Mock<IStringBuilderObjectPool> mockStringbuilder;
        
        [SetUp]
        public void Setup()
        {
            mockDnsString = new Mock<IDnsString>();
            //mockStringbuilder = new Mock<IStringBuilderObjectPool>();
            //mockStringbuilder.Setup(m => m.Get()).Returns(new StringBuilder());
            //mockStringbuilder.Setup(m => m.Return(It.IsAny<StringBuilder>()));
        }

        [Test]
        public void ReadQuestionQueryString_should_return_queryString_successfully()
        {
            // Arrange.
            
            const string query = "microsoft.com";
            const string queryDot = query + ".";
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionQuery);
            mockDnsString.Setup(m => m.FromResponseQueryString(queryDot)).Returns(new DnsString(query, queryDot));
            

            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);

            // move the index to 12th byte where the query starts in the response data.
            reader.Advance(12);

            // Act.
            
            IDnsString dnsString = reader.ReadQuestionQueryString(sampleData);

            // Assert.
            Assert.AreEqual(dnsString.Original, query);
            Assert.AreEqual(dnsString.Value, queryDot);
        }

        [TestCase(10, 20)]
        [TestCase(60, 120)]
        public void Advance_should_move_index_to_given_position(int length, int expectedIndexValue)
        {
            // Arrange.

            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);

            // Act.
            reader.Advance(length);
            reader.Advance(length);

            // Assert.
            Assert.AreEqual(expectedIndexValue, reader.Index);
        }

        [Test]
        public void ReadUInt16NetworkOrder_should_return_AnswerCount_of_6_from_data_starting_at_Index_0()
        {
            // Arrange.
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionQuery);
            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);
            reader.Advance(6);
            // Act.
            
            ushort answerCout = reader.ReadUInt16NetworkOrder(sampleData);

            // Assert.

            Assert.AreEqual(5, answerCout);
        }

        [Test]
        public void ReadUInt16NetworkOrder_should_throw_IndexOutOfRangeException_when_index_exceeds_data_count()
        {
            // Arrange.
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionQuery);
            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);
            reader.Advance(100);
            // Act.

            Assert.Throws<IndexOutOfRangeException>(() => reader.ReadUInt16NetworkOrder(sampleData), "Cannot read more data.");
        }

        // Index = 37

        /*
          *  Queries
          *      microsoft.com: type A, class IN
          *          Name: microsoft.com
          *          [Name Length: 13]
          *          [Label Count: 2]
          *          Type: A (Host Address) (1)
          *          Class: IN (0x0001)
          *      Answers
          *          microsoft.com: type A, class IN, addr 40.76.4.15
          *              Name: microsoft.com
          *              Type: A (Host Address) (1)
          *              Class: IN (0x0001)
          *              Time to live: 2585
          *              Data length: 4
          *              Address: 40.76.4.15
          *
         */

        [Test]
        public void ReadUInt32NetworkOrder_should_return_TTL_value_in_int32()
        {
            // Arrange.
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);

            // advance the index to Time To Live position.
            reader.Advance(37);

            // Act.
            uint timeToLive = reader.ReadUInt32NetworkOrder(sampleData);

            // Assert.
            Assert.AreEqual(2585, timeToLive);
        }

        // Index = 43

        /*
        *  Queries
        *      microsoft.com: type A, class IN
        *          Name: microsoft.com
        *          [Name Length: 13]
        *          [Label Count: 2]
        *          Type: A (Host Address) (1)
        *          Class: IN (0x0001)
        *      Answers
        *          microsoft.com: type A, class IN, addr 40.76.4.15
        *              Name: microsoft.com
        *              Type: A (Host Address) (1)
        *              Class: IN (0x0001)
        *              Time to live: 2585
        *              Data length: 4
        *              Address: 40.76.4.15 <<=============== Assert on this IP address.
        *
       */

        [Test]
        public void ReadIPAddress_should_return_ipAddress()
        {
            // Arrange.
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);
            // advance the index to IP Address position.
            reader.Advance(43);

            // Act.
            IPAddress ipAddress = reader.ReadIPAddress(sampleData);

            // Assert.

            Assert.AreEqual("40.76.4.15", ipAddress.ToString());
        }

        [Test]
        public void ReadIPAddress_should_throw_IndexOutOfRangeException_exception_when_less_than_4_bytes_are_present()
        {
            // Arrange.
            ArraySegment<byte> sampleData = GetSampleData(ResponseDataSampleFile.QuestionQuery);
            IDnsDatagramReader reader = new DnsDatagramReader(mockDnsString.Object);
            // advance the index to IP Address position + 1 to make it throw exception.
            reader.Advance(43);

            // Act => Assert.
            Assert.Throws<IndexOutOfRangeException>(() => reader.ReadIPAddress(sampleData),
                "Error while reading IPv4 address, expected 4 bytes. Index was outside the bounds of the array.");

        }
    }
}
