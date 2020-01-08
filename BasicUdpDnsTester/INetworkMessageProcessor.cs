namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System;
    using RequestMessageModel;
    using ResponseMessageModel;

    /// <summary> Handles the request and response messages. </summary>
    public interface INetworkMessageProcessor
    {
        /// <summary> Constructs a DNS query request object for given domain name to resolve. </summary>
        /// <param name="domainNameToResolve">Domain name to resolve. E.g. www.microsoft.com</param>
        /// <returns>Dns query request object.</returns>
        DnsRequestMessage ProcessRequest(string domainNameToResolve);

        /// <summary> Constructs the response object based upon the data received from the DNS server. </summary>
        /// <param name="responseData">The data (byte[]) from the DNS server.</param>
        /// <returns>Response message object.</returns>
        DnsResponseMessage ProcessResponse(ArraySegment<byte> responseData);
    }
}