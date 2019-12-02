using System;
using System.Collections.Generic;
using System.Linq;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    public class DnsResponseMessage
    {
        public DnsResponseMessage(DnsResponseHeader header, int messageSize)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            MessageSize = messageSize;
        }

        public List<DnsResourceRecord> Additionals { get; } = new List<DnsResourceRecord>();

        public List<DnsResourceRecord> Answers { get; } = new List<DnsResourceRecord>();

        public List<DnsResourceRecord> Authorities { get; } = new List<DnsResourceRecord>();

        public DnsResponseHeader Header { get; }

        public int MessageSize { get; }

        public List<DnsQuestion> Questions { get; } = new List<DnsQuestion>();

        public void AddAdditional(DnsResourceRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Additionals.Add(record);
        }

        public void AddAnswer(DnsResourceRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Answers.Add(record);
        }

        public void AddAuthority(DnsResourceRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Authorities.Add(record);
        }

        public void AddQuestion(DnsQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            Questions.Add(question);
        }
    }
}