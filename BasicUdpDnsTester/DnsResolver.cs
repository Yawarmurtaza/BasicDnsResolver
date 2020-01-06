namespace InfraServiceJobPackage.Library.DnsHelper
{
    using DnsProtocol;
    using MessageReaders;
    using MessageWriters;
    using Records;
    using RequestMessageModel;
    using ResponseMessageModel;
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
        private readonly IDnsDatagramReader reader;

        public DnsResolver(ICommunicator communicator, IDnsString dnsString, IDnsDatagramReader reader)
        {
            this.communicator = communicator;
            this.dnsString = dnsString;
            this.reader = reader;
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
                    DnsResponseMessage response = GetResponseMessage(memory.BufferSegment);
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

        private DnsResponseMessage GetResponseMessage(ArraySegment<byte> responseData)
        {
            IDnsRecordFactory factory = new DnsRecordFactory(reader);

            /*
             * From index 0 till 11 bytes in the responseData array, we have the header items.             
             */
            ushort id = reader.ReadUInt16NetworkOrder(responseData);
            ushort flags = reader.ReadUInt16NetworkOrder(responseData);
            ushort questionCount = reader.ReadUInt16NetworkOrder(responseData);
            ushort answerCount = reader.ReadUInt16NetworkOrder(responseData);
            ushort nameServerCount = reader.ReadUInt16NetworkOrder(responseData);
            ushort additionalCount = reader.ReadUInt16NetworkOrder(responseData);

            /*
             * We have got above data, now build the response header. 
             */
            DnsResponseHeader header = new DnsResponseHeader(id, flags, questionCount, answerCount, additionalCount, nameServerCount);
            DnsResponseMessage response = new DnsResponseMessage(header, responseData.Count);

            /*
             * Next item in the response data is question that we sent to the DNS server which the server has copied onto the response message.             
             */
            for (int questionIndex = 0; questionIndex < questionCount; questionIndex++)
            {
                IDnsString questionString = reader.ReadQuestionQueryString(responseData);
                DnsQuestion question = new DnsQuestion(questionString, (QueryType)reader.ReadUInt16NetworkOrder(responseData), (QueryClass)reader.ReadUInt16NetworkOrder(responseData));
                response.AddQuestion(question);
            }

            for (int answerIndex = 0; answerIndex < answerCount; answerIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo(responseData);
                DnsResourceRecord record = factory.GetRecord(info, responseData);
                response.AddAnswer(record);
            }

            for (int serverIndex = 0; serverIndex < nameServerCount; serverIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo(responseData);
                DnsResourceRecord record = factory.GetRecord(info, responseData);
                response.AddAuthority(record);
            }

            for (int additionalIndex = 0; additionalIndex < additionalCount; additionalIndex++)
            {
                BaseResourceRecordInfo info = factory.ReadRecordInfo(responseData);
                DnsResourceRecord record = factory.GetRecord(info, responseData);
                response.AddAdditional(record);
            }

            return response;
        }
    }
}