using System;

namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
    /*
     * RFC 1035 (https://tools.ietf.org/html/rfc1035#section-3.2.3)
     * */

    /*
     * RFC 1035 (https://tools.ietf.org/html/rfc1035#section-3.2.2)
     * */

    /* Reference: https://tools.ietf.org/html/rfc6895#section-2
     * Response header fields
     *
                                            1  1  1  1  1  1
              0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
             |                      ID                       |
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
       ==>   |QR|   OpCode  |AA|TC|RD|RA| Z|AD|CD|   RCODE   |  <==
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
             |                QDCOUNT/ZOCOUNT                |
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
             |                ANCOUNT/PRCOUNT                |
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
             |                NSCOUNT/UPCOUNT                |
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
             |                    ARCOUNT                    |
             +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

     * */

    /// <summary>
    /// The resource record types. The <c>enum</c> contains only the types supported by this library at this moment.
    /// The <see cref="ResourceRecordType"/> is used to identify any <see cref="DnsResourceRecord"/>.
    /// </summary>
    /// <para>
    /// Resource record types are a subset of <see cref="QueryType"/>.
    /// </para>
    /// </summary>
    /// <seealso cref="DnsResourceRecord"/>
    /// <seealso cref="ResourceRecordType"/>
    public enum ResourceRecordType : short
    {
        /// <summary>
        /// A host address.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        /// <seealso cref="ARecord"/>
        A = 1,

        /// <summary>
        /// Option record.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc6891">RFC 6891</seealso>
        /// <seealso cref="OptRecord"/>
        OPT = 41
    }
}