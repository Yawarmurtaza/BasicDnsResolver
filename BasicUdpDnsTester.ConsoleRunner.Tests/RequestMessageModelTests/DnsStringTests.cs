using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
using NUnit.Framework;
using System;

namespace BasicUdpDnsTester.ConsoleRunner.Tests.RequestMessageModelTests
{
    [TestFixture]
    public class DnsStringTests
    {
        private IDnsString dnsString;

        [SetUp]
        public void Setup()
        {
            dnsString = new DnsString();
        }

        [TestCase("microsoft.com.", "microsoft.com.")]
        [TestCase("microsoft.com", "microsoft.com.")]
        [TestCase("www.microsoft.com", "www.microsoft.com.")]
        [TestCase("123", "123.")]
        [TestCase("", ".")] // should return the root label "."
        [TestCase(  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                    "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                    "cooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooom", 
                    "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                    "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                    "cooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooom.")]
        [TestCase("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "miccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "mic","wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "miccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "mic.")]
        public void Parse_should_successfully_parse_given_queryString(string query, string parsedValue)
        {
            // Arrange.

            // Act.
            IDnsString resultString = dnsString.Parse(query);

            // Assert.
            Assert.AreEqual(parsedValue, resultString.Value);
        }

        [TestCase(null)]
        public void Parse_should_throw_ArgumentNullException_when_null_query_is_given(string query)
        {
            // Act => Assert.
            Assert.Throws<ArgumentNullException>(() => dnsString.Parse(query), "Value cannot be null.\r\nParameter name: query");
        }

        [TestCase(".www.microsoft.com")]
        public void Parse_should_throw_ArgumentException_when_a_leading_dot_query_is_given(string query)
        {
            // Act => Assert.
            Assert.Throws<ArgumentException>(() => dnsString.Parse(query), $"{query} is not a legal name, found leading root label.");
        }

        [TestCase("", ".")] // should return the root label "."
        public void Parse_should_return_RootLabel_when_empty_or_dot_is_given(string query, string parsedValue)
        {
            // Act.
            IDnsString resultString = dnsString.Parse(query);

            // Assert.
            Assert.AreEqual(parsedValue, resultString.Value);
        }

        [TestCase("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww.microsoft.com", 1)]
        [TestCase("www.miccccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft.com", 2)]
        [TestCase("www.microsoft.commmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm", 3)]
        public void Parse_should_throw_ArgumentException_when_label_is_longer_than_63(string query, int labelNumber)
        { 
            // Act => Assert.
            Assert.Throws<ArgumentException>(() => dnsString.Parse(query), $"Label {labelNumber} is longer than 63 bytes.");
        }

        [TestCase("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "micccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft")]
        [TestCase("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +                  
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "miccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "mic")]
        [TestCase("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww." +
                  "miccccccccccccccccccccccccccccccccccccccccccccccccccccccrosoft." +
                  "mic")]

        public void Parse_should_throw_ArgumentException_when_octate_length_exceeds(string query)
        {
            // Act => Assert.
            Assert.Throws<ArgumentException>(() => dnsString.Parse(query), $"Octet length of '{query}' exceeds maximum of 255 bytes.");
        }
    }
}
