using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    public class LookupClientAudit
    {
        private const string c_placeHolder = "$$REPLACEME$$";
        private static readonly int s_printOffset = -32;
        private StringBuilder _auditWriter = new StringBuilder();
        private Stopwatch _swatch;

        public DnsQuerySettings Settings { get; }

        public LookupClientAudit(DnsQuerySettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string Build(IDnsQueryResponse queryResponse)
        {
            if (!Settings.EnableAuditTrail)
            {
                return string.Empty;
            }

            var writer = new StringBuilder();

            if (queryResponse != null)
            {
                if (queryResponse.Questions.Count > 0)
                {
                    writer.AppendLine(";; QUESTION SECTION:");
                    foreach (var question in queryResponse.Questions)
                    {
                        writer.AppendLine(question.ToString(s_printOffset));
                    }
                    writer.AppendLine();
                }

                if (queryResponse.Answers.Count > 0)
                {
                    writer.AppendLine(";; ANSWER SECTION:");
                    foreach (var answer in queryResponse.Answers)
                    {
                        writer.AppendLine(answer.ToString(s_printOffset));
                    }
                    writer.AppendLine();
                }

                if (queryResponse.Authorities.Count > 0)
                {
                    writer.AppendLine(";; AUTHORITIES SECTION:");
                    foreach (var auth in queryResponse.Authorities)
                    {
                        writer.AppendLine(auth.ToString(s_printOffset));
                    }
                    writer.AppendLine();
                }

                if (queryResponse.Additionals.Count > 0)
                {
                    writer.AppendLine(";; ADDITIONALS SECTION:");
                    foreach (var additional in queryResponse.Additionals)
                    {
                        writer.AppendLine(additional.ToString(s_printOffset));
                    }
                    writer.AppendLine();
                }
            }

            var all = _auditWriter.ToString();
            var dynamic = writer.ToString();

            return all.Replace(c_placeHolder, dynamic);
        }
    }
}