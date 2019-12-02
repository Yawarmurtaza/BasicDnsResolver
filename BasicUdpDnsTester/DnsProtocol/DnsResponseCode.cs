using BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.DnsProtocol
{
    /// <summary>
    /// Response codes of the <see cref="IDnsQueryResponse"/>.
    /// </summary>
    /// <seealso href="https://tools.ietf.org/html/rfc6895#section-2.3">RFC 6895</seealso>
    public enum DnsResponseCode : short
    {
        /// <summary>
        /// No error condition
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        NoError = 0,

        /// <summary>
        /// Format error. The name server was unable to interpret the query.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        FormatError = 1,

        /// <summary>
        /// Server failure. The name server was unable to process this query due to a problem with the name server.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        ServerFailure = 2,

        /// <summary>
        /// Name Error. Meaningful only for responses from an authoritative name server,
        /// this code signifies that the domain name referenced in the query does not exist.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        NotExistentDomain = 3,

        /// <summary>
        /// Not Implemented. The name server does not support the requested kind of query.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        NotImplemented = 4,

        /// <summary>
        /// Refused. The name server refuses to perform the specified operation for policy reasons.
        /// For example, a name server may not wish to provide the information to the particular requester,
        /// or a name server may not wish to perform a particular operation (e.g., zone transfer) for particular data.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        Refused = 5,

        /// <summary>
        /// Name Exists when it should not.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2136">RFC 2136</seealso>
        ExistingDomain = 6,

        /// <summary>
        /// Resource record set exists when it should not.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2136">RFC 2136</seealso>
        ExistingResourceRecordSet = 7,

        /// <summary>
        /// Resource record set that should exist but does not.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2136">RFC 2136</seealso>
        MissingResourceRecordSet = 8,

        /// <summary>
        /// Server Not Authoritative for zone / Not Authorized.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2136">RFC 2136</seealso>
        /// <seealso href="https://tools.ietf.org/html/rfc2845">RFC 2845</seealso>
        NotAuthorized = 9,

        /// <summary>
        /// Name not contained in zone.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2136">RFC 2136</seealso>
        NotZone = 10,

        /// <summary>
        /// Bad OPT Version or TSIG Signature Failure.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2671">RFC 2671</seealso>
        /// <seealso href="https://tools.ietf.org/html/rfc2845">RFC 2845</seealso>
        BadVersionOrBadSignature = 16,

        /// <summary>
        /// Key not recognized.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2845">RFC 2845</seealso>
        BadKey = 17,

        /// <summary>
        /// Signature out of time window.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2845">RFC 2845</seealso>
        BadTime = 18,

        /// <summary>
        /// Bad TKEY Mode.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2930">RFC 2930</seealso>
        BadMode = 19,

        /// <summary>
        /// Duplicate key name.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2930">RFC 2930</seealso>
        BadName = 20,

        /// <summary>
        /// Algorithm not supported.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2930">RFC 2930</seealso>
        BadAlgorithm = 21,

        /// <summary>
        /// Bad Truncation.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc4635">RFC 4635</seealso>
        BadTruncation = 22,

        /// <summary>
        /// Bad/missing Server Cookie
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc7873">RFC 7873</seealso>
        BadCookie = 23,

        /// <summary>
        /// Unknown error.
        /// </summary>
        Unassigned = 666
    }
}