namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
    public static class DnsHeader
    {
        public static readonly ushort OPCodeMask = 0x7800;
        public static readonly ushort OPCodeShift = 11;
        public static readonly ushort RCodeMask = 0x000F;
    }
}