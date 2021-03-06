﻿namespace InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /*
    /// <summary> The readonly version of DnsQueryOptions used to customize settings per query. </summary>
    public class DnsQuerySettings : IEquatable<DnsQuerySettings>
    {
        private NameServer[] _endpoints;

        /// <summary> Gets a flag indicating whether each IDnsQueryResponse will contain a full documentation of the response(s). Default is <c>False</c>. </summary>
        /// <seealso cref="IDnsQueryResponse.AuditTrail
        public bool EnableAuditTrail { get; }

        /// <summary> Gets a flag indicating whether DNS queries should use response caching or not. The cache duration is calculated by the resource record of the response. Usually, the lowest TTL is used. Default is <c>True</c>. </summary>
        /// <remarks> In case the DNS Server returns records with a TTL of zero. The response cannot be cached. </remarks>
        public bool UseCache { get; }

        /// <summary> Gets a collection of name servers which should be used to query. </summary>
        public IReadOnlyCollection<NameServer> NameServers => _endpoints;

        /// <summary> Gets a flag indicating whether DNS queries should instruct the DNS server to do recursive lookups, or not. Default is <c>True</c>. </summary>
        /// <value>The flag indicating if recursion should be used or not.</value>
        public bool Recursion { get; }

        /// <summary> Gets the number of tries to get a response from one name server before trying the next one. Only transient errors, like network or connection errors will be retried. Default is <c>5</c>. <para> If all configured NameServers error out after retries, an exception will be thrown at the end. </para> </summary>
        /// <value>The number of retries.</value>
        public int Retries { get; }

        /// <summary> Gets a flag indicating whether the ILookupClient should throw a DnsResponseException in case the query result has a DnsResponseCode other than DnsResponseCode.NoError. Default is <c>False</c>. </summary>
        /// <remarks>
        /// <para> If set to <c>False</c>, the query will return a result with an IDnsQueryResponse.ErrorMessage which contains more information. </para>
        /// <para> If set to <c>True</c>, any query method of IDnsQuery will throw an DnsResponseException if the response header indicates an error. </para>
        /// <para> If both, ContinueOnDnsError and ThrowDnsErrors are set to <c>True</c>, ILookupClient will continue to query all configured NameServers. If none of the servers yield a valid response, a DnsResponseException will be thrown with the error of the last response. </para>
        /// </remarks>
        public bool ThrowDnsErrors { get; }

        /// <summary> Gets a flag indicating whether the ILookupClient can cycle through all configured NameServers on each consecutive request, basically using a random server, or not. Default is <c>True</c>. If only one NameServer is configured, this setting is not used. </summary>
        /// <remarks>
        /// <para> If <c>False</c>, configured endpoint will be used in random order. If <c>True</c>, the order will be preserved. </para>
        /// <para> Even if UseRandomNameServer is set to <c>True</c>, the endpoint might still get disabled and might not being used for some time if it errors out, e.g. no connection can be established. </para>
        /// </remarks>
        public bool UseRandomNameServer { get; }

        /// <summary>
        /// Gets a flag indicating whether to query the next configured NameServers in case the response of the last query
        /// returned a DnsResponseCode other than DnsResponseCode.NoError.
        /// Default is <c>True</c>.
        /// </summary>
        /// <remarks>
        /// If <c>True</c>, lookup client will continue until a server returns a valid result, or,
        /// if no NameServers yield a valid result, the last response with the error will be returned.
        /// In case no server yields a valid result and ThrowDnsErrors is also enabled, an exception
        /// will be thrown containing the error of the last response.
        /// </remarks>
        /// <seealso cref="ThrowDnsErrors
        public bool ContinueOnDnsError { get; }

        /// <summary> Gets the request timeout in milliseconds. Timeout is used for limiting the connection and request time for one operation. Timeout must be greater than zero and less than int.MaxValue. If System.Threading.Timeout.InfiniteTimeSpan (or -1) is used, no timeout will be applied. Default is 5 seconds. </summary>
        /// <remarks> If a very short timeout is configured, queries will more likely result in TimeoutExceptions.
        /// <para>
        /// Important to note, TimeoutExceptions will be retried, if Retries are not disabled (set to <c>0</c>).
        /// This should help in case one or more configured DNS servers are not reachable or under load for example.
        /// </para>
        /// </remarks>
        public TimeSpan Timeout { get; }

        /// <summary> Gets a flag indicating whether Tcp should be used in case a Udp response is truncated. Default is <c>True</c>. <para> If <c>False</c>, truncated results will potentially yield no or incomplete answers. </para> </summary>
        public bool UseTcpFallback { get; }

        /// <summary> Gets a flag indicating whether Udp should not be used at all. Default is <c>False</c>. <para> Enable this only if Udp cannot be used because of your firewall rules for example. Also, zone transfers (see QueryType.AXFR) must use TCP only. </para> </summary>
        public bool UseTcpOnly { get; }

        /// <summary> Gets a flag indicating whether the name server collection was manually defined or automatically resolved </summary>
        public bool AutoResolvedNameServers { get; }

        /// <inheritdocs />
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsQuerySettings);
        }

        /// <inheritdocs />
        public bool Equals(DnsQuerySettings other)
        {
            return other != null &&
                   NameServers.SequenceEqual(other.NameServers) &&
                   EnableAuditTrail == other.EnableAuditTrail &&
                   UseCache == other.UseCache &&
                   Recursion == other.Recursion &&
                   Retries == other.Retries &&
                   ThrowDnsErrors == other.ThrowDnsErrors &&
                   UseRandomNameServer == other.UseRandomNameServer &&
                   ContinueOnDnsError == other.ContinueOnDnsError &&
                   Timeout.Equals(other.Timeout) &&
                   UseTcpFallback == other.UseTcpFallback &&
                   UseTcpOnly == other.UseTcpOnly &&
                   AutoResolvedNameServers == other.AutoResolvedNameServers;
        }

        /// <inheritdocs />
        public override int GetHashCode()
        {
            var hashCode = -1775804580;
            hashCode = hashCode * -1521134295 + EqualityComparer<NameServer[]>.Default.GetHashCode(_endpoints);
            hashCode = hashCode * -1521134295 + EnableAuditTrail.GetHashCode();
            hashCode = hashCode * -1521134295 + UseCache.GetHashCode();
            hashCode = hashCode * -1521134295 + Recursion.GetHashCode();
            hashCode = hashCode * -1521134295 + Retries.GetHashCode();
            hashCode = hashCode * -1521134295 + ThrowDnsErrors.GetHashCode();
            hashCode = hashCode * -1521134295 + UseRandomNameServer.GetHashCode();
            hashCode = hashCode * -1521134295 + ContinueOnDnsError.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(Timeout);
            hashCode = hashCode * -1521134295 + UseTcpFallback.GetHashCode();
            hashCode = hashCode * -1521134295 + UseTcpOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + AutoResolvedNameServers.GetHashCode();
            return hashCode;
        }
    }*/
}