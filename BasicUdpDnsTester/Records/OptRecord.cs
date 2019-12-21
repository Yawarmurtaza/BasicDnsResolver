namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

    public class OptRecord : DnsResourceRecord
    {
        public OptRecord(int size = 4096, int version = 0, int length = 0)
            : base(new BaseResourceRecordInfo(ResourceRecordType.OPT, (QueryClass)size, version, length, new DnsString()))
        {
        }

        protected override string RecordToString()
        {
            return $"OPT {RecordClass}.";
        }
    }
}