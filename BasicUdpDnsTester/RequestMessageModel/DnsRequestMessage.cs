namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    /// <summary> Represents the DNS query request object that contains header and question sections. </summary>
    public class DnsRequestMessage
    {
        /// <summary> Gets the DNS query request header. </summary>
        public DnsRequestHeader Header { get; }

        /// <summary> Gets the DNS query request question. </summary>
        public DnsQuestion Question { get; }

        public DnsRequestMessage(DnsRequestHeader header, DnsQuestion question)
        {
            Header = header;
            Question = question;
        }
    }
}