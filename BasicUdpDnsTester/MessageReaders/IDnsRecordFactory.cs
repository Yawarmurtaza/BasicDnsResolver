using System;
using InfraServiceJobPackage.Library.DnsHelper.Records;

namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    public interface IDnsRecordFactory
    {
        DnsResourceRecord GetRecord(BaseResourceRecordInfo info, ArraySegment<byte> data);
        BaseResourceRecordInfo ReadRecordInfo(ArraySegment<byte> data);
    }
}