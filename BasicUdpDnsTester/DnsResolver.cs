namespace InfraServiceJobPackage.Library.DnsHelper
{
    using MessageWriters;
    using Records;
    using RequestMessageModel;
    using ResponseMessageModel;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public class DnsResolver : IDnsResolver
    {   
        private const int dnsPortNumber53 = 53;
        private const int ReadSize = 4096;

        private readonly IUdpCommunicator communicator;
        private readonly INetworkMessageProcessor messageProcessor;

        public DnsResolver(IUdpCommunicator communicator, INetworkMessageProcessor messageProcessor)
        {
            this.communicator = communicator;
            this.messageProcessor = messageProcessor;
        }

        public DnsResponseMessage Resolve(string dseServerName, string domainNameToResolve)
        {
            string targetDnsServerIpAddress = Dns.GetHostAddresses(dseServerName).First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(targetDnsServerIpAddress), dnsPortNumber53);
            DnsRequestMessage request = messageProcessor.ProcessRequest(domainNameToResolve);
            return Resolve(server, request);
        }

        public DnsResponseMessage Resolve(IPEndPoint server, string domainNameToResolve)
        {
            DnsRequestMessage request = messageProcessor.ProcessRequest(domainNameToResolve);
            return Resolve(server, request);
        }

        public DnsResponseMessage Resolve(IPEndPoint server, DnsRequestMessage dnsRequest)
        {
            using (communicator)
            {
                IUdpAdapter clientSocket = communicator.Client;
                int timeoutInMillis = 5000;
                clientSocket.ReceiveTimeout = timeoutInMillis;
                clientSocket.SendTimeout = timeoutInMillis;

                //
                // write the dns query request data on in bytes to send to the DNS server.
                //
                using (IDnsDatagramWriter writer = new DnsDatagramWriter())
                {
                    ConstructRequestData(dnsRequest, writer);
                    clientSocket.SendTo(writer.Data.Array, 0, writer.Data.Count, SocketFlags.None, server);
                }

                //
                // Read the dns query response from the DNS server.
                //
                using (PooledBytes memory = new PooledBytes(ReadSize))
                {
                    int received = clientSocket.Receive(memory.Buffer, 0, ReadSize, SocketFlags.None);
                    DnsResponseMessage response = messageProcessor.ProcessResponse(memory.BufferSegment);
                    if (dnsRequest.Header.Identifier != response.Header.Identifier)
                    {
                        throw new DnsResponseException("Header id mismatch.");
                    }

                    return response;
                }   
            }           
        }
        private void ConstructRequestData(DnsRequestMessage request, IDnsDatagramWriter writer)
        {
            DnsQuestion question = request.Question;

            /*
                                                1  1  1  1  1  1
                  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                      ID                       |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    QDCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    ANCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    NSCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    ARCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            * */
            // 4 more bytes for the type and class

            writer.WriteInt16NetworkOrder((short)request.Header.Identifier);
            writer.WriteUInt16NetworkOrder(request.Header.RawFlags);
            writer.WriteInt16NetworkOrder(1);   // we support single question only... (as most DNS servers anyways).
            writer.WriteInt16NetworkOrder(0);
            writer.WriteInt16NetworkOrder(0);
            writer.WriteInt16NetworkOrder(1);    // one additional for the Opt record.

            writer.WriteHostName(question.QueryName.Value);
            writer.WriteUInt16NetworkOrder((ushort)question.QuestionType);
            writer.WriteUInt16NetworkOrder((ushort)question.QuestionClass);

            /*
               +------------+--------------+------------------------------+
               | Field Name | Field Type   | Description                  |
               +------------+--------------+------------------------------+
               | NAME       | domain name  | MUST be 0 (root domain)      |
               | TYPE       | u_int16_t    | OPT (41)                     |
               | CLASS      | u_int16_t    | requester's UDP payload size |
               | TTL        | u_int32_t    | extended RCODE and flags     |
               | RDLEN      | u_int16_t    | length of all RDATA          |
               | RDATA      | octet stream | {attribute,value} pairs      |
               +------------+--------------+------------------------------+
            * */

            var opt = new OptRecord();

            writer.WriteHostName("");
            writer.WriteUInt16NetworkOrder((ushort)opt.RecordType);
            writer.WriteUInt16NetworkOrder((ushort)opt.RecordClass);
            writer.WriteUInt32NetworkOrder((ushort)opt.InitialTimeToLive);
            writer.WriteUInt16NetworkOrder(0);
        }

    }
}