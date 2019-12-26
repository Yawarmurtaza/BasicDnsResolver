namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using System.Net;
    using Records;
    using RequestMessageModel;

    public class DnsRecordFactory : IDnsRecordFactory
    {
        private readonly IDnsDatagramReader _reader;

        public DnsRecordFactory(IDnsDatagramReader reader)
        {
            _reader = reader;
        }

        /*
        0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                                               |
        /                                               /
        /                      NAME                     /
        |                                               |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                      TYPE                     |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                     CLASS                     |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                      TTL                      |
        |                                               |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                   RDLENGTH                    |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
        /                     RDATA                     /
        /                                               /
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        */

        public BaseResourceRecordInfo ReadRecordInfo(ArraySegment<byte> data)
        {
            IDnsString domainName = _reader.ReadQuestionQueryString(data);                              // domain name
            ResourceRecordType recordType = (ResourceRecordType) _reader.ReadUInt16NetworkOrder(data);  // record type
            QueryClass queryClass = (QueryClass) _reader.ReadUInt16NetworkOrder(data);                  // query class
            int timeToLive = (int) _reader.ReadUInt32NetworkOrder(data);                                // ttl - 32bit!!
            int rawDataLength = _reader.ReadUInt16NetworkOrder(data);                                   // RDLength

            return new BaseResourceRecordInfo(
                recordType ,    // record type
                queryClass,     // query class
                timeToLive,     // ttl - 32bit!!
                rawDataLength,  // RDLength
                domainName      // domain name
            );
        }

        public DnsResourceRecord GetRecord(BaseResourceRecordInfo info, ArraySegment<byte> data)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            int oldIndex = _reader.Index;
            DnsResourceRecord result;

            switch (info.RecordType)
            {
                case ResourceRecordType.A:
                    IPAddress ipaddress = _reader.ReadIPAddress(data);
                    result = new ARecord(info, ipaddress);
                    break;
                default:
                    // update reader index because we don't read full data for the empty record
                    _reader.Advance(info.RawDataLength);
                    result = new EmptyRecord(info);
                    break;
            }

            // sanity check
            if (_reader.Index != oldIndex + info.RawDataLength)
            {
                throw new InvalidOperationException("Record reader index out of sync.");
            }

            return result;
        }
    }
}