namespace InfraServiceJobPackage.Library.DnsHelper
{
    using InfraServiceJobPackage.Library.DnsHelper.DnsProtocol;
    using InfraServiceJobPackage.Library.DnsHelper.MessageReaders;
    using InfraServiceJobPackage.Library.DnsHelper.MessageWriters;
    using InfraServiceJobPackage.Library.DnsHelper.Records;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;
    using InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class DnsResolver : IDnsResolver
    {
        private const int dnsPortNumber = 53;
        private int _uniqueId;
        private Random _random = new Random();
        private const int ReadSize = 4096;
        private readonly ICommunicator communicator;
        private readonly IDnsString dnsString;

        public DnsResolver(ICommunicator communicator, IDnsString dnsString)
        {
            this.communicator = communicator;
            this.dnsString = dnsString;
        }

        public DnsResponseMessage Resolve(string dseServerName, string domainNameToResolve)
        {
            string targetDnsServerIpAddress = Dns.GetHostAddresses(dseServerName).First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(targetDnsServerIpAddress), dnsPortNumber);

            DnsRequestMessage request = GetDnsRequestMessage(domainNameToResolve);
            return Resolve(server, request);
        }

        public DnsResponseMessage Resolve(IPEndPoint server, string domainNameToResolve)
        {
            DnsRequestMessage request = GetDnsRequestMessage(domainNameToResolve);
            return Resolve(server, request);
        }

        public DnsResponseMessage Resolve(IPEndPoint server, DnsRequestMessage dnsRequest)
        {
            using (communicator)
            {
                IUdpSocketProxy clientSocket = communicator.Client;
                int timeoutInMillis = 5000;
                clientSocket.ReceiveTimeout = timeoutInMillis;
                clientSocket.SendTimeout = timeoutInMillis;
                using (DnsDatagramWriter writer = new DnsDatagramWriter())
                {
                    ConstructRequestData(dnsRequest, writer);
                    clientSocket.SendTo(writer.Data.Array, 0, writer.Data.Count, SocketFlags.None, server);
                }

                using (PooledBytes memory = new PooledBytes(ReadSize))
                {
                    int received = clientSocket.Receive(memory.Buffer, 0, ReadSize, SocketFlags.None);
                    DnsResponseMessage response = this.GetResponseMessage(memory.BufferSegment);
                    if (dnsRequest.Header.Identifier != response.Header.Identifier)
                    {
                        throw new DnsResponseException("Header id mismatch.");
                    }

                    return response;
                }
            }           
        }

        private DnsRequestMessage GetDnsRequestMessage(string domainNameToResolve)
        {
            ushort headerId = GetNextUniqueId();
            DnsRequestHeader header = new DnsRequestHeader(headerId, true, DnsOpCode.Query);

            DnsQuestion question = new DnsQuestion(dnsString.Parse(domainNameToResolve), QueryType.A, QueryClass.IN);
            DnsRequestMessage message = new DnsRequestMessage(header, question);
            return message;
        }

        private ushort GetNextUniqueId()
        {
            if (_uniqueId == ushort.MaxValue || _uniqueId == 0)
            {
                Interlocked.Exchange(ref _uniqueId, _random.Next(ushort.MaxValue / 2));
                return (ushort)_uniqueId;
            }

            return unchecked((ushort)Interlocked.Increment(ref _uniqueId));
        }

        private void ConstructRequestData(DnsRequestMessage request, DnsDatagramWriter writer)
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

        private DnsResponseMessage GetResponseMessage(ArraySegment<byte> responseData)
        {
            IDnsDatagramReader reader = new DnsDatagramReader(responseData, dnsString);
            DnsRecordFactory factory = new DnsRecordFactory(reader);

            ushort id = reader.ReadUInt16NetworkOrder();
            ushort flags = reader.ReadUInt16NetworkOrder();
            ushort questionCount = reader.ReadUInt16NetworkOrder();
            ushort answerCount = reader.ReadUInt16NetworkOrder();
            ushort nameServerCount = reader.ReadUInt16NetworkOrder();
            ushort additionalCount = reader.ReadUInt16NetworkOrder();

            var header = new DnsResponseHeader(id, flags, questionCount, answerCount, additionalCount, nameServerCount);
            var response = new DnsResponseMessage(header, responseData.Count);

            for (int questionIndex = 0; questionIndex < questionCount; questionIndex++)
            {
                var question = new DnsQuestion(reader.ReadQuestionQueryString(), (QueryType)reader.ReadUInt16NetworkOrder(), (QueryClass)reader.ReadUInt16NetworkOrder());
                response.AddQuestion(question);
            }

            for (int answerIndex = 0; answerIndex < answerCount; answerIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo();
                DnsResourceRecord record = factory.GetRecord(info);
                response.AddAnswer(record);
            }

            for (int serverIndex = 0; serverIndex < nameServerCount; serverIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo();
                DnsResourceRecord record = factory.GetRecord(info);
                response.AddAuthority(record);
            }

            for (int additionalIndex = 0; additionalIndex < additionalCount; additionalIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo();
                DnsResourceRecord record = factory.GetRecord(info);
                response.AddAdditional(record);
            }

            return response;
        }
    }
}