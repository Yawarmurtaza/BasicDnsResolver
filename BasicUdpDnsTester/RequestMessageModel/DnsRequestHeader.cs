using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;

namespace BasicUdpDnsTester.ConsoleRunner.RequestMessageModel
{
    public class DnsRequestHeader
    {
        private ushort _flags = 0;

        public ushort RawFlags => _flags;

        public DnsHeaderFlag HeaderFlags
        {
            get
            {
                return (DnsHeaderFlag)_flags;
            }
            set
            {
                _flags &= (ushort)~DnsHeaderFlag.IsCheckingDisabled;
                _flags &= (ushort)~DnsHeaderFlag.IsAuthenticData;
                _flags &= (ushort)~DnsHeaderFlag.FutureUse;
                _flags &= (ushort)~DnsHeaderFlag.HasQuery;
                _flags &= (ushort)~DnsHeaderFlag.HasAuthorityAnswer;
                _flags &= (ushort)~DnsHeaderFlag.ResultTruncated;
                _flags &= (ushort)~DnsHeaderFlag.RecursionDesired;
                _flags &= (ushort)~DnsHeaderFlag.RecursionAvailable;
                _flags |= (ushort)value;
            }
        }

        /// <summary> Gets the identifier. It is used to match the response with the query that was sent by the client.
        /// We use different (random) identifier number each time sending a query to the DNS server.
        /// The DNS server duplicates this number in the corresponding response. This helps up to pair up the query and response messages.</summary>
        public int Identifier { get; set; }

        public DnsOpCode OpCode
        {
            get
            {
                return (DnsOpCode)((DnsHeader.OPCodeMask & _flags) >> DnsHeader.OPCodeShift);
            }
            set
            {
                _flags &= (ushort)~(DnsHeader.OPCodeMask);
                _flags |= (ushort)(((ushort)value << DnsHeader.OPCodeShift) & DnsHeader.OPCodeMask);
            }
        }

        public bool UseRecursion
        {
            get
            {
                return HeaderFlags.HasFlag(DnsHeaderFlag.RecursionDesired);
            }
            set
            {
                if (value)
                {
                    _flags |= (ushort)DnsHeaderFlag.RecursionDesired;
                }
                else
                {
                    _flags &= (ushort)~(DnsHeaderFlag.RecursionDesired);
                }
            }
        }

        public DnsRequestHeader(int id, bool useRecursion, DnsOpCode queryKind)
        {
            Identifier = id;
            OpCode = queryKind;
            UseRecursion = useRecursion;
        }

        public override string ToString()
        {
            return $"{Identifier} - Qs: {1} Recursion: {UseRecursion} OpCode: {OpCode}";
        }
    }
}