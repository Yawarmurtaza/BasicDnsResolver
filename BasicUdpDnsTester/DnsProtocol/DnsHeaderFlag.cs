using System;

namespace InfraServiceJobPackage.Library.DnsHelper.DnsProtocol
{

    /*
                                        1  1  1  1  1  1
          0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                      ID                       |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                    QDCOUNT                    |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                    ANCOUNT                    |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                    NSCOUNT                    |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        |                    ARCOUNT                    |
        +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    *
     *
     * Flags: 0x0100 Standard query
        0... .... .... .... = Response: Message is a query
        .000 0... .... .... = Opcode: Standard query (0)
        .... ..0. .... .... = Truncated: Message is not truncated
        .... ...1 .... .... = Recursion desired: Do query recursively
        .... .... .0.. .... = Z: reserved (0)
        .... .... ...0 .... = Non-authenticated data: Unacceptable
     *
     * */

    /// <summary>
    /// The flags of the header's second 16bit value
    /// </summary>
    [Flags]
    public enum DnsHeaderFlag : ushort
    {
        IsCheckingDisabled = 0x0010,
        IsAuthenticData = 0x0020,
        FutureUse = 0x0040,             // Z bit seems to be ignored now, see https://tools.ietf.org/html/rfc6895#section-2.1
        
        /// <summary>
        /// Represents the RA part of the header. Only in response, 1 = recursive response is avialable.
        /// </summary>
        RecursionAvailable = 0x0080,

        /// <summary>
        /// Represents Recursion Desired (RD) part of the header. Used in both query and response messages. 1 = recursion is desired.
        /// </summary>
        RecursionDesired = 0x0100,

        /// <summary>
        /// Represents the TC = truncated part of header. 1 = response was more than 512 bytes and has been truncated. 
        /// </summary>
        ResultTruncated = 0x0200,

        /// <summary>
        /// Represents AA part of header. 1 = name server is authoritative server, 0 otherwise. This field is updated by the DNS server only.
        /// </summary>
        HasAuthorityAnswer = 0x0400,

        /// <summary>
        /// Represents the QR part of header. 0 = query, 1 = response.
        /// </summary>
        HasQuery = 0x8000,
    }
}