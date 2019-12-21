using System;
using System.Configuration;
using InfraServiceJobPackage.Library.DnsHelper;
using InfraServiceJobPackage.Library.DnsHelper.Records;
using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner
{
    class Program
    {   
        static void Main(string[] args)
        {
            Tuple<string, string> userInput = GetUserInput();
            string domainName = userInput.Item1;
            string dnsServerIp = userInput.Item2;

            try
            {
                // use IoC to resolve these...
                ICommunicator communicator = new UdpCommunicator();
                IDnsString dnsString = new DnsString();
                DnsResolver resolver = new DnsResolver(communicator, dnsString);

                DnsResponseMessage response = resolver.Resolve(dnsServerIp, domainName);
                PrintResult(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static Tuple<string, string> GetUserInput()
        {
            AppSettingsReader appSettings = new AppSettingsReader();
            string dn = appSettings.GetValue("DomainNameToResolve", typeof(string)).ToString();
            string targetDnsServerName = appSettings.GetValue("TargetDnsServerName", typeof(string)).ToString();

            Console.WriteLine($"Domain Name to resolve defaults to {dn}");

            string domainName = Console.ReadLine();
            if (string.IsNullOrEmpty(domainName))
            {
                domainName = dn;
            }

            Console.WriteLine($"Target DNS Server name defaults to {targetDnsServerName} ");
            string dnsServerName = Console.ReadLine();
            if (string.IsNullOrEmpty(dnsServerName))
            {
                dnsServerName = targetDnsServerName;
            }

            return new Tuple<string, string>(domainName, dnsServerName);
        }

        private static void PrintResult(DnsResponseMessage response)
        {
            Console.WriteLine("\nDomain Name\t\tRecord Type\tRecord Class\tTime to live\tIP Address");
            Console.WriteLine("--------------------------------------------------------------------------------------------|");
            foreach (DnsResourceRecord nextRec in response.Answers)
            {
                ARecord rec = nextRec as ARecord;
                Console.WriteLine($"{nextRec.DomainName.Value.TrimEnd('.')}\t\t{nextRec.RecordType}\t\t{nextRec.RecordClass}\t\t{nextRec.TimeToLive}\t\t{rec?.Address}");
            }
        }
    }
}
