using System;

namespace BasicUdpDnsTester.ConsoleRunner.RequestMessageModel
{
    public class DnsRequestMessage
    {
        public DnsRequestHeader Header { get; }

        public DnsQuestion Question { get; }

        public DnsRequestMessage(DnsRequestHeader header, DnsQuestion question)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Question = question ?? throw new ArgumentNullException(nameof(question));
        }
    }
}