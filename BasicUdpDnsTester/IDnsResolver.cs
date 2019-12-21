namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System.Net;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
    using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;

    public interface IDnsResolver
    {
        DnsResponseMessage Resolve(string dseServerName, string domainNameToResolve);
        DnsResponseMessage Resolve(IPEndPoint server, string domainNameToResolve);
        DnsResponseMessage Resolve(IPEndPoint server, DnsRequestMessage dnsRequest);
    }
}