using System;
using System.Net;
using System.Text;
using InfraServiceJobPackage.Library.DnsHelper.MessageReaders;
using InfraServiceJobPackage.Library.DnsHelper.Records;
using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
using Moq;
using NUnit.Framework;

namespace BasicUdpDnsTester.ConsoleRunner.Tests.MessageReaders
{
    //[TestFixture]
    //public class StringBuilderObjectPoolTests
    //{
    //    [Test]
    //    public void GetTest()
    //    {
    //        // Arrange.
    //        IStringBuilderObjectPool stringBuilder = new StringBuilderObjectPool();

    //        // Act.
    //        StringBuilder result = stringBuilder.Get();
    //        StringBuilder result2 = stringBuilder.Get();
    //        StringBuilder result3 = stringBuilder.Get();

    //        // Assert.
    //        Assert.IsNotNull(result);
    //    }
    //}

    [TestFixture]
    public class DnsRecordFactoryTests : BaseTestHelper
    {
        private Mock<IDnsDatagramReader> mockReader;

        [SetUp]
        public void Setup()
        {
            mockReader = new Mock<IDnsDatagramReader>();
        }


        [Test]
        public void ReadRecordInfo_should_return_BaseResourceRecordInfo_from_responseMessage()
        {
            // Arrange.
            const string query = "www.msn.com";
            const string queryRoot = query + ".";
            const ushort recordType = 1; // A - Host Address.
            const ushort queryClass = 1; // IN
            const int timeToLive = 1000;
            const int rawDataLen = 10;

            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            mockReader.Setup(r => r.ReadQuestionQueryString(responseData))
                .Returns(new DnsString(query, queryRoot));
            mockReader.SetupSequence(r => r.ReadUInt16NetworkOrder(responseData))
                .Returns(recordType)
                .Returns(queryClass)
                .Returns(rawDataLen);
            mockReader.Setup(r => r.ReadUInt32NetworkOrder(responseData))
                .Returns(timeToLive);

            IDnsRecordFactory factory = new DnsRecordFactory(mockReader.Object);

            // Act.
            BaseResourceRecordInfo recordInfo = factory.ReadRecordInfo(responseData);

            // Assert.
            // no need to check id recordInfo is null because we are already asserting on its properties.
            Assert.AreEqual(queryRoot, recordInfo.DomainName.ToString());
            Assert.AreEqual(recordType, (int)recordInfo.RecordType);
            Assert.AreEqual(queryClass, (int)recordInfo.RecordClass);
            Assert.AreEqual(rawDataLen, recordInfo.RawDataLength);
            Assert.AreEqual(timeToLive, recordInfo.TimeToLive);
        }

        [Test]
        public void GetRecord_should_return_RecordTypeA()
        {
            // Arrange.
            const string query = "www.msn.com";
            const string queryRoot = query + ".";
            const ResourceRecordType recordType = ResourceRecordType.A; // A - Host Address.
            const QueryClass queryClass =  QueryClass.IN; // IN
            const int timeToLive = 1000;
            const int rawDataLen = 10;
            IPAddress ipaddress = IPAddress.Parse("10.1.1.100");

            IDnsString dnsString = new DnsString(query,queryRoot);

            BaseResourceRecordInfo info = new BaseResourceRecordInfo(recordType, queryClass, timeToLive, rawDataLen, dnsString);
            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            mockReader.SetupSequence(r => r.Index).Returns(100).Returns(100 + rawDataLen);
            mockReader.Setup(r => r.ReadIPAddress(responseData)).Returns(ipaddress);
            
            IDnsRecordFactory factory = new DnsRecordFactory(mockReader.Object);

            // Act.

            DnsResourceRecord result = factory.GetRecord(info, responseData);

            // Assert.

            Assert.IsInstanceOf(typeof(ARecord), result);
            ARecord record = (ARecord) result;

            Assert.AreEqual(queryRoot, record.DomainName.ToString());
            Assert.AreEqual(ipaddress.ToString(), record.Address.ToString());
            Assert.AreEqual(recordType, record.RecordType);
            Assert.AreEqual(queryClass, record.RecordClass);
            Assert.AreEqual(timeToLive, record.TimeToLive);
            Assert.AreEqual(rawDataLen, record.RawDataLength);
        }

        [Test]
        public void GetRecord_should_return_EmptyRecord_type_when_unknown_recordType_is_found()
        {
            // Arrange.
            const string query = "www.msn.com";
            const string queryRoot = query + ".";
            const ResourceRecordType recordType = ResourceRecordType.OPT; // possibly make it to default case in the switch statement.
            const QueryClass queryClass = QueryClass.IN; // IN
            const int timeToLive = 1000;
            const int rawDataLen = 10;
            IPAddress ipaddress = IPAddress.Parse("10.1.1.100");

            IDnsString dnsString = new DnsString(query, queryRoot);

            BaseResourceRecordInfo info = new BaseResourceRecordInfo(recordType, queryClass, timeToLive, rawDataLen, dnsString);
            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            mockReader.SetupSequence(r => r.Index).Returns(100).Returns(100 + rawDataLen);
            mockReader.Setup(r => r.ReadIPAddress(responseData)).Returns(ipaddress);

            IDnsRecordFactory factory = new DnsRecordFactory(mockReader.Object);

            // Act.
            DnsResourceRecord result = factory.GetRecord(info, responseData);

            // Assert.

            Assert.IsInstanceOf(typeof(EmptyRecord), result);
            EmptyRecord record = (EmptyRecord)result;
        }

        [Test]
        public void GetRecord_should_throw_InvalidOperationException_when_Record_reader_index_becomes_out_of_sync()
        {
            // Arrange.
            const string query = "www.msn.com";
            const string queryRoot = query + ".";
            const ResourceRecordType recordType = ResourceRecordType.A;
            const QueryClass queryClass = QueryClass.IN; // IN
            const int timeToLive = 1000;
            const int rawDataLen = 10;
            IPAddress ipaddress = IPAddress.Parse("10.1.1.100");

            IDnsString dnsString = new DnsString(query, queryRoot);

            BaseResourceRecordInfo info = new BaseResourceRecordInfo(recordType, queryClass, timeToLive, rawDataLen, dnsString);
            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);
            mockReader.Setup(r => r.Index).Returns(100);
            mockReader.Setup(r => r.ReadIPAddress(responseData)).Returns(ipaddress);

            IDnsRecordFactory factory = new DnsRecordFactory(mockReader.Object);

            // Act.

            Assert.Throws<InvalidOperationException>(() => factory.GetRecord(info, responseData), "Record reader index out of sync.");
        }
    }
}
