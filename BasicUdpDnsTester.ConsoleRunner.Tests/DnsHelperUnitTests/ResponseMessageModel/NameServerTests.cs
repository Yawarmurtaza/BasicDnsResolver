using System.Collections.Generic;
using System.Net;
using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;
using NUnit.Framework;

namespace InfraServiceJobPackageUnitTest.DnsHelperUnitTests.ResponseMessageModel
{
    [TestFixture]
    public class NameServerTests
    {

        [Test]
        public void Implicit_should_be_able_to_return_a_non_null_NameServer_object_when_newedup_using_IPEndpoint()
        {
            // Arrange.
            const string ipAddressString = "10.1.1.10";
            const int port = 53;

            // Act.
            NameServer tempServer = new IPEndPoint(IPAddress.Parse(ipAddressString), port);

            // Assert.

            Assert.IsNotNull(tempServer);
            Assert.AreEqual(ipAddressString, tempServer.IPEndPoint.Address.ToString());
            Assert.AreEqual(port, tempServer.Port);
        }

        [Test]
        public void Implicit_should_be_able_to_return_a_non_null_NameServer_object_when_newedup_using_IPAddress()
        {
            // Arrange.
            const string ipAddressString = "10.1.1.10";
            const int port = 53;

            // Act.
            NameServer tempServer = IPAddress.Parse(ipAddressString);

            // Assert.

            Assert.IsNotNull(tempServer);
            Assert.AreEqual(ipAddressString, tempServer.IPEndPoint.Address.ToString());
            Assert.AreEqual(port, tempServer.Port);
        }

        [Test]
        public void ToString_should_return_XX()
        {
            // arrange.
            const string ipAddress = "10.1.1.10";
            const int port = 53;
            string toStringValue = $"{ipAddress}:{port} (Udp: 512)";

            // Act.
            NameServer ns = new NameServer(IPAddress.Parse(ipAddress));

            // Assert.
            Assert.AreEqual(toStringValue, ns.ToString());
        }

        [TestCaseSource(nameof(GetNameServerTestObjects))]
        public void Equals_should_return_true(NameServer ns2, bool expectedResult)
        {
            // Arrange.
            const string ipAddress = "10.1.1.10";
            const int port = 53;

            NameServer ns = new NameServer(IPAddress.Parse(ipAddress));

            // Act.

            bool result = ns.Equals(ns2);

            // Assert.

            Assert.AreEqual(expectedResult, result);
        }

        private static IEnumerable<TestCaseData> GetNameServerTestObjects()
        {
            yield return new TestCaseData(new NameServer(IPAddress.Parse("10.1.1.10")), true);
            yield return new TestCaseData(new NameServer(IPAddress.Parse("10.0.0.1")), false);

        }

    }
}
