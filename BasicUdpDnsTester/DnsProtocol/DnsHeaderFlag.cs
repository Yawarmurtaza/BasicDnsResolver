using System;

namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
    /// <summary>
    /// The flags of the header's second 16bit value
    /// </summary>
    [Flags]
    public enum DnsHeaderFlag : ushort
    {
        IsCheckingDisabled = 0x0010,
        IsAuthenticData = 0x0020,
        FutureUse = 0x0040,             // Z bit seems to be ignored now, see https://tools.ietf.org/html/rfc6895#section-2.1
        RecursionAvailable = 0x0080,
        RecursionDesired = 0x0100,
        ResultTruncated = 0x0200,
        HasAuthorityAnswer = 0x0400,
        HasQuery = 0x8000,
    }
}