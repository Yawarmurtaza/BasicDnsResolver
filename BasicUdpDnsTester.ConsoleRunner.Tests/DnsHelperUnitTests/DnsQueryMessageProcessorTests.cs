using System;
using System.Net;
using InfraServiceJobPackage.Library.DnsHelper;
using InfraServiceJobPackage.Library.DnsHelper.DnsProtocol;
using InfraServiceJobPackage.Library.DnsHelper.MessageReaders;
using InfraServiceJobPackage.Library.DnsHelper.Records;
using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;
using InfraServiceJobPackageUnitTest.DnsHelperUnitTests.MessageReaders;
using Moq;
using NUnit.Framework;

namespace InfraServiceJobPackageUnitTest.DnsHelperUnitTests
{
    [TestFixture]
    public class DnsQueryMessageProcessorTests : BaseTestHelper
    {
        private Mock<IDnsDatagramReader> mockReader;
        private Mock<IDnsString> mockDnsString;

        [SetUp]
        public void Setup()
        {
            mockDnsString = new Mock<IDnsString>();
            mockReader = new Mock<IDnsDatagramReader>();
        }

        [TearDown]
        public void Teardown()
        {
            mockDnsString = null;
            mockReader = null;
            GC.Collect();
        }
        
        [Test]
        public void ProcessRequest_should_return_DnsRequestMessageObject()
        {
            // Arrange.
            const string domainNameToResolve = "www.microsoft.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            mockDnsString.Setup(m => m.Parse(domainNameToResolve))
                .Returns(new DnsString(domainNameToResolve, domainNameToResolveRoot));

            INetworkMessageProcessor messageProcessor = new DnsQueryMessageProcessor(mockReader.Object, mockDnsString.Object);

            // Act.
            DnsRequestMessage result = messageProcessor.ProcessRequest(domainNameToResolve);

            // Assert.

            // header checks
            Assert.Greater(result.Header.Identifier, 0);
            Assert.IsTrue(result.Header.UseRecursion);
            Assert.AreEqual(DnsHeaderFlag.RecursionDesired, result.Header.HeaderFlags);
            Assert.AreEqual(DnsOpCode.Query, result.Header.OpCode);
            Assert.AreEqual(256, result.Header.RawFlags);

            // question checks.
            Assert.AreEqual(QueryClass.IN, result.Question.QuestionClass);
            Assert.AreEqual(QueryType.A, result.Question.QuestionType);
            Assert.AreEqual(domainNameToResolveRoot, result.Question.QueryName.Value);
        }

        [Test]
        public void ProcessResponse_should_return_DnsResponseMessage_object_with_1_answer_1_question_only_successfully()
        {
            // Arrange.
            const string domainNameToResolve = "www.microsoft.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            const string ipAddress = "10.1.1.100";
            const ushort headerId = 1001;
            IDnsString dnsString = new DnsString(domainNameToResolve, domainNameToResolveRoot);

            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);

            mockReader.SetupSequence(m => m.ReadUInt16NetworkOrder(responseData))
                .Returns(headerId) // id
                .Returns(33152) // flags
                .Returns(1) // questionCount
                .Returns(1) // answerCount
                .Returns(0) // nameServerCount - setting it to zero for this unit test
                .Returns(0) // additionCount
                .Returns(1) // QueryType 
                .Returns(1) // QueryClass
                // BaseResourceRecordInfo
                .Returns(1) // record type
                .Returns(1) //query class
                .Returns(4); //// RDLength
            mockReader.Setup(m => m.ReadUInt32NetworkOrder(responseData)).Returns(12); // ttl - 32bit!!

            // question
            mockReader.SetupSequence(m => m.ReadQuestionQueryString(responseData))
                .Returns(dnsString)     // question
                .Returns(dnsString);    // ReadRecordInfo
            mockReader.Setup(m => m.ReadIPAddress(responseData)).Returns(IPAddress.Parse(ipAddress));
            
            mockReader.SetupSequence(m => m.Index).Returns(8).Returns(12);

            INetworkMessageProcessor messageProcessor = new DnsQueryMessageProcessor(mockReader.Object, mockDnsString.Object);

            // Act.
            DnsResponseMessage response = messageProcessor.ProcessResponse(responseData);

            // Assert.

            Assert.AreEqual(1, response.Answers.Count);
            Assert.AreEqual(1, response.Questions.Count);

            Assert.AreEqual(0, response.Additionals.Count);
            Assert.AreEqual(0, response.Authorities.Count);

            Assert.AreEqual(headerId, response.Header.Identifier);
            Assert.IsInstanceOf<ARecord>(response.Answers[0]);
            ARecord arecord = response.Answers[0] as ARecord;
            Assert.AreEqual(domainNameToResolveRoot, response.Answers[0].DomainName.Value);
            Assert.AreEqual(ipAddress, arecord.Address.ToString());

            // question check
            Assert.AreEqual(QueryClass.IN, response.Questions[0].QuestionClass);
            Assert.AreEqual(QueryType.A, response.Questions[0].QuestionType);
            Assert.AreEqual(domainNameToResolveRoot, response.Questions[0].QueryName.Value);
        }

        [Test]
        public void ProcessResponse_should_return_DnsResponseMessage_object_with_2_answers_1_question_only_successfully()
        {
            // Arrange.
            const string domainNameToResolve = "www.microsoft.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            const string ipAddress = "10.1.1.100";
            const string ipAddress1 = "101.0.0.101";
            const ushort headerId = 1001;
            IDnsString dnsString = new DnsString(domainNameToResolve, domainNameToResolveRoot);

            ArraySegment<byte> responseData = GetSampleData(ResponseDataSampleFile.QuestionAndAnswerQuery);

            mockReader.SetupSequence(m => m.ReadUInt16NetworkOrder(responseData))
                .Returns(headerId) // id
                .Returns(33152) // flags
                .Returns(1) // questionCount
                .Returns(2) // answerCount
                .Returns(0) // nameServerCount - setting it to zero for this unit test
                .Returns(0) // additionCount
                .Returns(1) // QueryType 
                .Returns(1) // QueryClass
                
                 // BaseResourceRecordInfo - answer 1
                .Returns(1) // record type
                .Returns(1) // query class
                .Returns(4) // RDLength
                            
                 // BaseResourceRecordInfo - answer 2
                .Returns(1) // record type
                .Returns(1) //query class
                .Returns(4); //// RDLength
            
            mockReader.Setup(m => m.ReadUInt32NetworkOrder(responseData)).Returns(12); // ttl - 32bit!!

            // question
            mockReader.SetupSequence(m => m.ReadQuestionQueryString(responseData))
                .Returns(dnsString)     // question
                .Returns(dnsString)    // ReadRecordInfo for answer 1
                .Returns(dnsString);    // ReadRecordInfo for answer 2
            mockReader.SetupSequence(m => m.ReadIPAddress(responseData))
                .Returns(IPAddress.Parse(ipAddress)) // answer 1
                .Returns(IPAddress.Parse(ipAddress1)); // answer 2

            mockReader.SetupSequence(m => m.Index)
                .Returns(8).Returns(12) // answer 1
                .Returns(8).Returns(12); // answer 2

            INetworkMessageProcessor messageProcessor = new DnsQueryMessageProcessor(mockReader.Object, mockDnsString.Object);

            // Act.
            DnsResponseMessage response = messageProcessor.ProcessResponse(responseData);

            // Assert.

            Assert.AreEqual(2, response.Answers.Count);
            Assert.AreEqual(1, response.Questions.Count);

            Assert.AreEqual(0, response.Additionals.Count);
            Assert.AreEqual(0, response.Authorities.Count);

            Assert.AreEqual(headerId, response.Header.Identifier);
            Assert.IsInstanceOf<ARecord>(response.Answers[0]); // first answer record.
            Assert.IsInstanceOf<ARecord>(response.Answers[1]); // second answer record. 
            
            // second Answer check
            ARecord arecord = response.Answers[1] as ARecord;
            Assert.AreEqual(domainNameToResolveRoot, response.Answers[1].DomainName.Value);
            Assert.AreEqual(ipAddress1, arecord.Address.ToString());

            // question check
            Assert.AreEqual(QueryClass.IN, response.Questions[0].QuestionClass);
            Assert.AreEqual(QueryType.A, response.Questions[0].QuestionType);
            Assert.AreEqual(domainNameToResolveRoot, response.Questions[0].QueryName.Value);
        }
    }
}