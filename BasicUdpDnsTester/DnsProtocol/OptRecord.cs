using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
   public class OptRecord : DnsResourceRecord
   {
        public OptRecord(int size = 4096, int version = 0, int length = 0)
            : base(new ResourceRecordInfo(DnsString.RootLabel, ResourceRecordType.OPT, (QueryClass)size, version, length))
        {
        }

        private protected override string RecordToString()
        {
            return $"OPT {RecordClass}.";
        }
    }
}