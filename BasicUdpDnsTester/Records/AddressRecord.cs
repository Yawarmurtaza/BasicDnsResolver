using System;
using System.Net;

namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    /// <summary>
    /// Base class for <see cref="DnsResourceRecord"/>s transporting an <see cref="IPAddress"/>.
    /// </summary>
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

        /// <summary>
        /// </summary>
        /// <param name="info">The information.</param>
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