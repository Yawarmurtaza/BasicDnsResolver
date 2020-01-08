using System;
using System.Net;
using System.Net.Sockets;
using InfraServiceJobPackage.Library.DnsHelper;
using InfraServiceJobPackage.Library.DnsHelper.DnsProtocol;
using InfraServiceJobPackage.Library.DnsHelper.Records;
using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;
using Moq;
using NUnit.Framework;

namespace InfraServiceJobPackageUnitTest.DnsHelperUnitTests
{
    [TestFixture]
    public class DnsResolverTests
    {
        private const int ReadSize = 4096;
        const int dnsPort53 = 53;

        private Mock<IUdpCommunicator> mockCommunicator;
        private Mock<INetworkMessageProcessor> mockMessageProcessor;
        private Mock<IDnsString> mockDnsString;
        private Mock<IUdpAdapter> mockUdpSocket;

        [SetUp]
        public void Setup()
        {
            mockCommunicator = new Mock<IUdpCommunicator>();
            mockMessageProcessor = new Mock<INetworkMessageProcessor>();
            mockDnsString = new Mock<IDnsString>();
            mockUdpSocket = new Mock<IUdpAdapter>();
            mockCommunicator.Setup(m => m.Client).Returns(mockUdpSocket.Object);
        }

        [TearDown]
        public void Teardown()
        {
            mockCommunicator = null;
            mockMessageProcessor = null;
            mockDnsString = null;
            mockUdpSocket = null;
            GC.Collect();
        }


        [Test]
        public void Resolve_overload_with_IPEndpoint_and_DnsRequetMessage_should_throw_DnsResponseException_when_header_ids_dont_match()
        {
            // Arrange.
            const string domainNameToResolve = "www.A-Dummy-Domain.com";
            string domainNameToResolveRoot = domainNameToResolve + ".";
            const ushort headerId = 123; // because we are *only* setting this up in the request header, it should not match with the response header.

            DnsRequestHeader header = new DnsRequestHeader(headerId, true, DnsOpCode.Query);
            DnsQuestion question = new DnsQuestion(mockDnsString.Object, QueryType.A, QueryClass.IN);
            DnsRequestMessage dnsRequestMessage = new DnsRequestMessage(header, question);
            IPEndPoint dnsServerIP = new IPEndPoint(IPAddress.Parse("10.0.0.10"), dnsPort53);
            
            mockDnsString.Setup(m => m.Value).Returns(domainNameToResolveRoot);
            DnsResponseMessage responseMessage = GetDnsResponseMessage();
            mockMessageProcessor.Setup(m => m.ProcessResponse(It.IsAny<ArraySegment<byte>>())).Returns(responseMessage);
            IDnsResolver resolver = new DnsResolver(mockCommunicator.Object, mockMessageProcessor.Object);

            // Act => Act.

            Assert.Throws<DnsResponseException>(() => resolver.Resolve(dnsServerIP, dnsRequestMessage), "Header id mismatch.");
        }

        [Test]
        public void Resolve_should_successfully_resolve_in_ARecord_domainName_using_IPEndpoint_and_DnsMessageRequest_overload()
        {
            // Arrange.
            const string domainNameToResolve = "www.A-Dummy-Domain.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            const ushort headerId = 12345;
            const string ipAddress = "10.50.75.40";
            DnsRequestMessage requestMessage = GetDnsRequestMessage(headerId);
            IPEndPoint dnsServerIP = new IPEndPoint(IPAddress.Parse(ipAddress), dnsPort53);
            DnsResponseMessage responseMessage = GetDnsResponseMessage(headerId, ipAddress, domainNameToResolveRoot);
            mockMessageProcessor.Setup(m => m.ProcessResponse(It.IsAny<ArraySegment<byte>>())).Returns(responseMessage);
            mockDnsString.Setup(m => m.Value).Returns(domainNameToResolveRoot);
            mockUdpSocket.Setup(m => m.Receive(It.IsAny<byte[]>(), It.Is<int>(i => i == 0), It.Is<int>(i => i == ReadSize), It.Is<SocketFlags>( s => s == SocketFlags.None)))
               .Returns(1000);
            
            
            IDnsResolver resolver = new DnsResolver(mockCommunicator.Object, mockMessageProcessor.Object);

            // Act.
            DnsResponseMessage response = resolver.Resolve(dnsServerIP, requestMessage);

            // Assert.

            Assert.IsInstanceOf(typeof(ARecord), response.Answers[0]);
            ARecord arecord = response.Answers[0] as ARecord;
            Assert.AreEqual(domainNameToResolveRoot, response.Answers[0].DomainName.Value);
            Assert.AreEqual(ipAddress, arecord.Address.ToString());
        }

        [Test]
        public void Resolve_should_successfully_resolve_domainName_using_stringType_domainName_and_string_domainName()
        {
            // Arrange.
            const string domainNameToResolve = "www.A-Dummy-Domain.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            const ushort headerId = 12345;
            const string ipAddress = "10.50.75.40";
            
            DnsRequestMessage request = GetDnsRequestMessage(headerId);
            DnsResponseMessage responseMessage = GetDnsResponseMessage(headerId, ipAddress, domainNameToResolveRoot);

            mockMessageProcessor.Setup(m => m.ProcessRequest(domainNameToResolve)).Returns(request);
            mockMessageProcessor.Setup(m => m.ProcessResponse(It.IsAny<ArraySegment<byte>>())).Returns(responseMessage);
            mockUdpSocket.Setup(m => m.Receive(It.IsAny<byte[]>(), It.Is<int>(i => i == 0), It.Is<int>(i => i == ReadSize), It.Is<SocketFlags>(s => s == SocketFlags.None)))
                .Returns(1000);
            IDnsResolver resolver = new DnsResolver(mockCommunicator.Object, mockMessageProcessor.Object);

            // Act.

            DnsResponseMessage response = resolver.Resolve(ipAddress, domainNameToResolve);

            // Assert.

            Assert.IsInstanceOf(typeof(ARecord), response.Answers[0]);
            ARecord arecord = response.Answers[0] as ARecord;
            Assert.AreEqual(domainNameToResolveRoot, response.Answers[0].DomainName.Value);
            Assert.AreEqual(ipAddress, arecord.Address.ToString());

        }


        [Test]
        public void Resolve_should_successfully_resolve_ARecord_domaiName_using_IPEndpoint_and_string_domainName()
        {
            // Arrange.
            const string domainNameToResolve = "www.A-Dummy-Domain.com";
            const string domainNameToResolveRoot = domainNameToResolve + ".";
            const ushort headerId = 12345;
            const string ipAddress = "10.50.75.40";
            IPEndPoint dnsServerIP = new IPEndPoint(IPAddress.Parse(ipAddress), dnsPort53);
            DnsRequestMessage requestMessage = GetDnsRequestMessage(headerId);
            DnsResponseMessage responseMessage = GetDnsResponseMessage(headerId, ipAddress, domainNameToResolveRoot);
            mockMessageProcessor.Setup(m => m.ProcessRequest(domainNameToResolve)).Returns(requestMessage);
            mockMessageProcessor.Setup(m => m.ProcessResponse(It.IsAny<ArraySegment<byte>>())).Returns(responseMessage);
            mockUdpSocket.Setup(m => m.Receive(It.IsAny<byte[]>(), It.Is<int>(i => i == 0), It.Is<int>(i => i == ReadSize), It.Is<SocketFlags>(s => s == SocketFlags.None)))
                .Returns(1000);
            IDnsResolver resolver = new DnsResolver(mockCommunicator.Object, mockMessageProcessor.Object);
            // Act.

            DnsResponseMessage response = resolver.Resolve(ipAddress, domainNameToResolve);

            // Assert.

            Assert.IsInstanceOf(typeof(ARecord), response.Answers[0]);
            ARecord arecord = response.Answers[0] as ARecord;
            Assert.AreEqual(domainNameToResolveRoot, response.Answers[0].DomainName.Value);
            Assert.AreEqual(ipAddress, arecord.Address.ToString());


        }


        #region helper methods.
        private DnsResponseMessage GetDnsResponseMessage(int headerId = 0, string ipAddress = "10.0.0.10", string domainNameToResolveRoot = "www.microsoft.com.", int ttl = 10, int rawDataLen = 1024)
        {
            DnsResponseHeader header = new DnsResponseHeader(headerId, 0, 0, 0, 0, 0);
            DnsResponseMessage message = new DnsResponseMessage(header, 0);
            mockDnsString.Setup(m => m.Value).Returns(domainNameToResolveRoot);

            message.Answers.Add(new ARecord(new BaseResourceRecordInfo(ResourceRecordType.A, QueryClass.IN, ttl, rawDataLen, mockDnsString.Object), IPAddress.Parse(ipAddress)));
            return message;
        }

        private DnsRequestMessage GetDnsRequestMessage(ushort headerId = 0)
        {
            DnsRequestHeader header = new DnsRequestHeader(headerId, true, DnsOpCode.Query);
            DnsQuestion question = new DnsQuestion(mockDnsString.Object, QueryType.A, QueryClass.IN);
            DnsRequestMessage message = new DnsRequestMessage(header, question);
            return message;
        }

        #endregion
    }
}
