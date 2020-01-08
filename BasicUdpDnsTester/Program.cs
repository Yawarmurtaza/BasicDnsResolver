using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using InfraServiceJobPackage.Library.DnsHelper;
using InfraServiceJobPackage.Library.DnsHelper.MessageReaders;
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

                for (int i = 0; i < 100; i++)
                {
                    // use IoC to resolve these...
                    IUdpCommunicator communicator = new UdpCommunicator();
                    IDnsString dnsString = new DnsString();
                    //IStringBuilderObjectPool stringBuilder = new StringBuilderObjectPool();
                    IDnsDatagramReader reader = new DnsDatagramReader(dnsString);
                    INetworkMessageProcessor messageProcessor = new DnsQueryMessageProcessor(reader, dnsString);
                    DnsResolver resolver = new DnsResolver(communicator, messageProcessor);

                    DnsResponseMessage response = resolver.Resolve(dnsServerIp, domainName);
                    PrintResult(response);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        public static string GetDnsAddresses()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                IPAddress ipv4Dns = dnsServers.First(s => s.AddressFamily == AddressFamily.InterNetwork);
                return ipv4Dns.ToString();

                //if (dnsServers.Count > 0)
                //{
                //    Console.WriteLine(adapter.Description);
                //    foreach (IPAddress dns in dnsServers)
                //    {
                //        Console.WriteLine("  DNS Servers ............................. : {0}", dns.ToString());
                //    }
                //    Console.WriteLine();
                //}
            }

            return null;
        }

        private static Tuple<string, string> GetUserInput()
        {
            AppSettingsReader appSettings = new AppSettingsReader();
            string dn = appSettings.GetValue("DomainNameToResolve", typeof(string)).ToString();

            // string targetDnsServerName = appSettings.GetValue("TargetDnsServerName", typeof(string)).ToString();
            string targetDnsServerName = GetDnsAddresses();

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
