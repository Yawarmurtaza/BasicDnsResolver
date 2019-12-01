using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;
using BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner
{
    class Program
    {
        private static int _uniqueId;
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            Tuple<string, string> userInput = GetUserInput();
            string domainName = userInput.Item1;
            string dnsServerIp = userInput.Item2;
            try
            {
                DnsResolver resolver = new DnsResolver();
                IPEndPoint server = new IPEndPoint(IPAddress.Parse(dnsServerIp), 53);
                DnsRequestMessage dnsRequest = GetDnsRequestMessage(domainName);
                DnsResponseMessage response = resolver.Query(server, dnsRequest);
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
            string dn = "killer.net";
            string ipaddress = "172.20.10.1";

            Console.WriteLine($"Domain name: defaults to {dn}");

            string domainName = Console.ReadLine();
            if (string.IsNullOrEmpty(domainName))
            {
                domainName = dn;
            }

            Console.WriteLine("DSE Server IP Address: ");
            string dseServerIpAddress = Console.ReadLine();
            if (string.IsNullOrEmpty(dseServerIpAddress))
            {
                dseServerIpAddress = ipaddress;
            }

            return new Tuple<string, string>(domainName, dseServerIpAddress);
        }

        private static DnsRequestMessage GetDnsRequestMessage(string domainNameToResolve)
        {
            ushort headerId = GetNextUniqueId();
            DnsRequestHeader header = new DnsRequestHeader(headerId, DnsOpCode.Query);
            DnsQuestion question = new DnsQuestion(domainNameToResolve, QueryType.A);
            DnsRequestMessage message = new DnsRequestMessage(header, question);
            return message;
        }

        private static ushort GetNextUniqueId()
        {
            if (_uniqueId == ushort.MaxValue || _uniqueId == 0)
            {
                Interlocked.Exchange(ref _uniqueId, _random.Next(ushort.MaxValue / 2));
                return (ushort)_uniqueId;
            }

            return unchecked((ushort)Interlocked.Increment(ref _uniqueId));
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
