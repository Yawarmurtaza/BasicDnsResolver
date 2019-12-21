namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    using System;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

    /// <summary>Base type for resource records.</summary>
    public class BaseResourceRecordInfo
    {
        private readonly int ticks;

        /// <summary>The domain name used to query.</summary>
        public IDnsString DomainName { get; }

        /// <summary>Specifies type of resource record.</summary>
        public ResourceRecordType RecordType { get; }

        /// <summary>Specifies type class of resource record, mostly IN but can be CS, CH or HS .</summary>
        public QueryClass RecordClass { get; }

        /// <summary>Gets the current time to live value for the record.</summary>
        public int TimeToLive
        {
            get
            {
                int curTicks = Environment.TickCount & int.MaxValue;
                if (curTicks < ticks)
                {
                    return 0;
                }

                var ttl = InitialTimeToLive - ((curTicks - ticks) / 1000);
                return ttl < 0 ? 0 : ttl;
            }
        }

        /// <summary>Gets or sets the original time to live returned from the server.</summary>
        public int InitialTimeToLive { get; internal set; }

        /// <summary>Gets the number of bytes for this resource record stored in RDATA.</summary>
        public int RawDataLength { get; }

        /// <summary>Initializes a new instance of BaseResourceRecordInfo class.</summary>
        /// <param name="domainName">The <see cref="DnsString" /> used by the query.</param>
        /// <param name="recordType">Type of the record.</param>
        /// <param name="recordClass">The record class.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="rawDataLength">Length of the raw data.</param>        
        public BaseResourceRecordInfo(ResourceRecordType recordType, QueryClass recordClass, int timeToLive, int rawDataLength, IDnsString domainName)
        {
            DomainName = domainName;
            RecordType = recordType;
            RecordClass = recordClass;
            RawDataLength = rawDataLength;
            InitialTimeToLive = timeToLive;
            ticks = Environment.TickCount;
        }
    }
}