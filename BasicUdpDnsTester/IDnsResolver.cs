namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System.Net;
    using RequestMessageModel;
    using ResponseMessageModel;

    public interface IDnsResolver
    {
        DnsResponseMessage Resolve(string dseServerName, string domainNameToResolve);
        DnsResponseMessage Resolve(IPEndPoint server, string domainNameToResolve);
        DnsResponseMessage Resolve(IPEndPoint server, DnsRequestMessage dnsRequest);
    }
}