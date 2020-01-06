namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    using System.Net;

    /// <summary> Base class for DnsResourceRecords transporting an IPAddress </summary>
    /// <seealso cref="ARecord"/>
    public class AddressRecord : DnsResourceRecord
    {
        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public IPAddress Address { get; }

        /// <summary> Creates a new instance of AddressRecord type.</summary>
        /// <param name="baseRecordInfo">Base record info.</param>
        /// <param name="address">The address.</param>        
        public AddressRecord(BaseResourceRecordInfo baseRecordInfo, IPAddress address) : base(baseRecordInfo)
        {
            Address = address;
        }

        protected override string RecordToString()
        {
            return Address.ToString();
        }
    }
}