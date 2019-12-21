using System;

namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    /// <summary>
    /// Base class for all DNS resource records.
    /// </summary>
    /// <seealso cref="ResourceRecordInfo" />
    public abstract class DnsResourceRecord : BaseResourceRecordInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DnsResourceRecord" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is null.</exception>
        protected DnsResourceRecord(BaseResourceRecordInfo info) : base(info.RecordType, info.RecordClass, info.InitialTimeToLive, info.RawDataLength, info.DomainName)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(0);
        }

        /// <summary>
        /// by <paramref name="offset"/>.
        /// Set the offset to -32 for example to make it print nicely in consols.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>A string representing this instance.</returns>
        public virtual string ToString(int offset = 0)
        {
            string returnString = $"{DomainName}, {offset}{TimeToLive}\t{RecordClass}\t{RecordType}\t{RecordToString()}";            
            return returnString;
        }

        /// <summary>
        /// Returns a string representation of the record's value only.
        /// <see cref="ToString(int)"/> uses this to compose the full string value of this instance.
        /// </summary>
        /// <returns>A string representing this record.</returns>
        protected abstract string RecordToString();
    }
}