namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using InfraServiceJobPackage.Library.DnsHelper.Records;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

    public class DnsRecordFactory
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
         * */

        public BaseResourceRecordInfo ReadRecordInfo()
        {
            IDnsString readQuestionQueryString = _reader.ReadQuestionQueryString();
            return new BaseResourceRecordInfo(                
                (ResourceRecordType)_reader.ReadUInt16NetworkOrder(),   // type
                (QueryClass)_reader.ReadUInt16NetworkOrder(),           // class
                (int)_reader.ReadUInt32NetworkOrder(),        // ttl - 32bit!!
                _reader.ReadUInt16NetworkOrder(), // RDLength
                readQuestionQueryString            // name
                );          
        }

        public DnsResourceRecord GetRecord(BaseResourceRecordInfo info)
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
                    result = new ARecord(info, _reader.ReadIPAddress());
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