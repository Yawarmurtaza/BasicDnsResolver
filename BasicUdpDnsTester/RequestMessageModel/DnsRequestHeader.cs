namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    using InfraServiceJobPackage.Library.DnsHelper.DnsProtocol;

    public class DnsRequestHeader
    {
        public ushort RawFlags { get; private set; } = 0;

        public DnsHeaderFlag HeaderFlags
        {
            get
            {
                return (DnsHeaderFlag)RawFlags;
            }
            set
            {
                RawFlags &= (ushort)~DnsHeaderFlag.IsCheckingDisabled;
                RawFlags &= (ushort)~DnsHeaderFlag.IsAuthenticData;
                RawFlags &= (ushort)~DnsHeaderFlag.FutureUse;
                RawFlags &= (ushort)~DnsHeaderFlag.HasQuery;
                RawFlags &= (ushort)~DnsHeaderFlag.HasAuthorityAnswer;
                RawFlags &= (ushort)~DnsHeaderFlag.ResultTruncated;
                RawFlags &= (ushort)~DnsHeaderFlag.RecursionDesired;
                RawFlags &= (ushort)~DnsHeaderFlag.RecursionAvailable;
                RawFlags |= (ushort)value;
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
                return (DnsOpCode)((DnsHeader.OPCodeMask & RawFlags) >> DnsHeader.OPCodeShift);
            }
            set
            {
                RawFlags &= (ushort)~(DnsHeader.OPCodeMask);
                RawFlags |= (ushort)(((ushort)value << DnsHeader.OPCodeShift) & DnsHeader.OPCodeMask);
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
                    RawFlags |= (ushort)DnsHeaderFlag.RecursionDesired;
                }
                else
                {
                    RawFlags &= (ushort)~(DnsHeaderFlag.RecursionDesired);
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