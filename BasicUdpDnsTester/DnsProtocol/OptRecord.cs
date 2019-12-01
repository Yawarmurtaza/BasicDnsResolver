using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
   public class OptRecord : DnsResourceRecord
    {
        private const uint ResponseCodeMask = 0xff000000;
        private const int ResponseCodeShift = 20;
        private const uint VersionMask = 0x00ff0000;
        private const int VersionShift = 16;

        public DnsResponseCode ResponseCodeEx
        {
            get
            {
                return (DnsResponseCode)((InitialTimeToLive & ResponseCodeMask) >> ResponseCodeShift);
            }
            set
            {
                InitialTimeToLive &= (int)~ResponseCodeMask;
                InitialTimeToLive |= (int)(((int)value << ResponseCodeShift) & ResponseCodeMask);
            }
        }

        public short UdpSize
        {
            get { return (short)RecordClass; }
        }

        public byte Version
        {
            get
            {
                return (byte)((InitialTimeToLive & VersionMask) >> VersionShift);
            }
            set
            {
                InitialTimeToLive = (int)((uint)InitialTimeToLive & ~VersionMask);
                InitialTimeToLive |= (int)((value << VersionShift) & VersionMask);
            }
        }

        public bool IsDnsSecOk
        {
            get { return (InitialTimeToLive & 0x8000) != 0; }
            set
            {
                if (value)
                {
                    InitialTimeToLive |= 0x8000;
                }
                else
                {
                    InitialTimeToLive &= 0x7fff;
                }
            }
        }

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