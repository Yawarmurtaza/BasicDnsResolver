namespace InfraServiceJobPackage.Library.DnsHelper.DnsProtocol
{
    using InfraServiceJobPackage.Library.DnsHelper.Records;

    /// <summary>
    /// The query type field appear in the question part of a query.
    /// Query types are a superset of <see cref="ResourceRecordType"/>.
    /// </summary>
    public enum QueryType : short
    {
        /// <summary>
        /// A host address.
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        /// <seealso cref="ARecord"/>
        A = ResourceRecordType.A
    }
}