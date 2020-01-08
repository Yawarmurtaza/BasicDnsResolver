namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System;
    using System.Threading;
    using DnsProtocol;
    using MessageReaders;
    using Records;
    using RequestMessageModel;
    using ResponseMessageModel;

    /// <summary> Handles the request and response messages. </summary>
    public class DnsQueryMessageProcessor : INetworkMessageProcessor
    {
        private int _uniqueId;
        private readonly Random random;

        private readonly IDnsDatagramReader reader;
        private readonly IDnsString dnsString;

        public DnsQueryMessageProcessor(IDnsDatagramReader reader, IDnsString dnsString)
        {
            this.reader = reader;
            this.dnsString = dnsString;
            random = new Random();
        }

        /// <summary> Constructs a DNS query request object for given domain name to resolve. </summary>
        /// <param name="domainNameToResolve">Domain name to resolve. E.g. www.microsoft.com</param>
        /// <returns>Dns query request object.</returns>
        public DnsRequestMessage ProcessRequest(string domainNameToResolve)
        {
            ushort headerId = GetNextUniqueId();
            DnsRequestHeader header = new DnsRequestHeader(headerId, true, DnsOpCode.Query);

            DnsQuestion question = new DnsQuestion(dnsString.Parse(domainNameToResolve), QueryType.A, QueryClass.IN);
            DnsRequestMessage message = new DnsRequestMessage(header, question);
            return message;
        }

        /// <summary> Constructs the response object based upon the data received from the DNS server. </summary>
        /// <param name="responseData">The data (byte[]) from the DNS server.</param>
        /// <returns>Response message object.</returns>
        public DnsResponseMessage ProcessResponse(ArraySegment<byte> responseData)
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

        private ushort GetNextUniqueId()
        {
            if (_uniqueId == ushort.MaxValue || _uniqueId == 0)
            {
                Interlocked.Exchange(ref _uniqueId, random.Next(ushort.MaxValue / 2));
                return (ushort)_uniqueId;
            }

            return unchecked((ushort)Interlocked.Increment(ref _uniqueId));
        }
    }
}