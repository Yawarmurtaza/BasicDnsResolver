namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    using System.Net;

    /*
    3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    where:

    ADDRESS         A 32 bit Internet address.

    Hosts that have multiple Internet addresses will have multiple A
    records.
    *
    */

    /// <summary> A <see cref="DnsResourceRecord"/> representing an IPv4 IPAddress. Hosts that have multiple Internet addresses will have multiple A records. </summary>
    /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
    public class ARecord : AddressRecord
    {
        /// <summary> Initializes a new instance of the ARecord class. </summary>
        public ARecord(BaseResourceRecordInfo info, IPAddress address) : base(info, address)
        {
        }
    }
}