namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    /// <summary>
    /// Represents the DNS query object that contains header and question sections.
    /// </summary>
    public class DnsRequestMessage
    {
        public DnsRequestHeader Header { get; }

        public DnsQuestion Question { get; }

        public DnsRequestMessage(DnsRequestHeader header, DnsQuestion question)
        {
            Header = header;
            Question = question;
        }
    }
}