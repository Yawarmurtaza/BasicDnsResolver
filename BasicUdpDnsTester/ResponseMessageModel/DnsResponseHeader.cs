using System;
using System.Collections.Generic;
using System.Linq;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    /// <summary>
    /// The header section of a <see cref="IDnsQueryResponse"/>.
    /// </summary>
    public class DnsResponseHeader
    {
        private readonly ushort _flags = 0;

        /// <summary>
        /// Gets the number of additional records in the <see cref="IDnsQueryResponse"/>.
        /// </summary>
        /// <value>
        /// The number of additional records.
        /// </value>
        public int AdditionalCount { get; }

        /// <summary>
        /// Gets the number of answer records in the <see cref="IDnsQueryResponse"/>.
        /// </summary>
        /// <value>
        /// The number of answer records.
        /// </value>
        public int AnswerCount { get; }

        /// <summary>
        /// Gets a value indicating whether the future use flag is set.
        /// </summary>
        /// <value>
        ///   The future use flag.
        /// </value>
        public bool FutureUse => HasFlag(DnsHeaderFlag.FutureUse);

        /// <summary>
        /// Gets a value indicating whether this instance has authority answers.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has authority answers; otherwise, <c>false</c>.
        /// </value>
        public bool HasAuthorityAnswer => HasFlag(DnsHeaderFlag.HasAuthorityAnswer);

        internal DnsHeaderFlag HeaderFlags => (DnsHeaderFlag)_flags;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets a value indicating whether the result is authentic data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the result is authentic; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticData => HasFlag(DnsHeaderFlag.IsAuthenticData);

        /// <summary>
        /// Gets a value indicating whether checking is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checking is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckingDisabled => HasFlag(DnsHeaderFlag.IsCheckingDisabled);

        /// <summary>
        /// Gets a value indicating whether this instance has a query.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a query; otherwise, <c>false</c>.
        /// </value>
        public bool HasQuery => HasFlag(DnsHeaderFlag.HasQuery);

        /// <summary>
        /// Gets the number of name servers.
        /// </summary>
        /// <value>
        /// The number of name servers.
        /// </value>
        public int NameServerCount { get; }

        /// <summary>
        /// Gets the kind of query defined by <see cref="DnsOpCode"/>.
        /// </summary>
        /// <value>
        /// The query kind.
        /// </value>
        public DnsOpCode OPCode => (DnsOpCode)((DnsHeader.OPCodeMask & _flags) >> DnsHeader.OPCodeShift);

        /// <summary>
        /// Gets the number of questions of the <see cref="IDnsQueryResponse"/>.
        /// </summary>
        /// <value>
        /// The number of questions.
        /// </value>
        public int QuestionCount { get; }

        /// <summary>
        /// Gets a value indicating whether recursion is available on the DNS server.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recursion is available; otherwise, <c>false</c>.
        /// </value>
        public bool RecursionAvailable => HasFlag(DnsHeaderFlag.RecursionAvailable);

        /// <summary>
        /// Gets the response code.
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        public DnsResponseCode ResponseCode => (DnsResponseCode)(_flags & DnsHeader.RCodeMask);

        /// <summary>
        /// Gets a value indicating whether the result was truncated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the result was truncated; otherwise, <c>false</c>.
        /// </value>
        public bool ResultTruncated => HasFlag(DnsHeaderFlag.ResultTruncated);

        /// <summary>
        /// Gets a value indicating whether recursion desired flag was set by the request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the recursion desired flag was set; otherwise, <c>false</c>.
        /// </value>
        public bool RecursionDesired => HasFlag(DnsHeaderFlag.RecursionDesired);

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsResponseHeader"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="questionCount">The question count.</param>
        /// <param name="answerCount">The answer count.</param>
        /// <param name="additionalCount">The additional count.</param>
        /// <param name="serverCount">The server count.</param>
        [CLSCompliant(false)]
        public DnsResponseHeader(int id, ushort flags, int questionCount, int answerCount, int additionalCount, int serverCount)
        {
            Id = id;
            _flags = flags;
            QuestionCount = questionCount;
            AnswerCount = answerCount;
            AdditionalCount = additionalCount;
            NameServerCount = serverCount;
        }

        private bool HasFlag(DnsHeaderFlag flag) => (HeaderFlags & flag) != 0;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var head = $";; ->>HEADER<<- opcode: {OPCode}, status: {DnsResponseCodeText.GetErrorText(ResponseCode)}, id: {Id}";
            var flags = new string[] {
                HasQuery ? "qr" : "",
                HasAuthorityAnswer ? "aa" : "",
                RecursionDesired ? "rd" : "",
                RecursionAvailable ? "ra" : "",
                ResultTruncated ? "tc" : "",
                IsCheckingDisabled ? "cd" : "",
                IsAuthenticData ? "ad" : ""
            };

            var flagsString = string.Join(" ", flags.Where(p => p != ""));
            return $"{head}\r\n;; flags: {flagsString}; QUERY: {QuestionCount}, " +
                   $"ANSWER: {AnswerCount}, AUTORITY: {NameServerCount}, ADDITIONAL: {AdditionalCount}";
        }
    }

    public static class DnsResponseCodeText
    {
        internal const string BADALG = "Algorithm not supported";
        internal const string BADCOOKIE = "Bad/missing Server Cookie";
        internal const string BADKEY = "Key not recognized";
        internal const string BADMODE = "Bad TKEY Mode";
        internal const string BADNAME = "Duplicate key name";
        internal const string BADSIG = "TSIG Signature Failure";
        internal const string BADTIME = "Signature out of time window";
        internal const string BADTRUNC = "Bad Truncation";
        internal const string BADVERS = "Bad OPT Version";
        internal const string FormErr = "Format Error";
        internal const string NoError = "No Error";
        internal const string NotAuth = "Server Not Authoritative for zone or Not Authorized";
        internal const string NotImp = "Not Implemented";
        internal const string NotZone = "Name not contained in zone";
        internal const string NXDomain = "Non-Existent Domain";
        internal const string NXRRSet = "RR Set that should exist does not";
        internal const string Refused = "Query Refused";
        internal const string ServFail = "Server Failure";
        internal const string Unassigned = "Unknown Error";
        internal const string YXDomain = "Name Exists when it should not";
        internal const string YXRRSet = "RR Set Exists when it should not";

        private static readonly Dictionary<DnsResponseCode, string> errors = new Dictionary<DnsResponseCode, string>()
        {
            { DnsResponseCode.NoError, DnsResponseCodeText.NoError },
            { DnsResponseCode.FormatError, DnsResponseCodeText.FormErr },
            { DnsResponseCode.ServerFailure, DnsResponseCodeText.ServFail },
            { DnsResponseCode.NotExistentDomain, DnsResponseCodeText.NXDomain },
            { DnsResponseCode.NotImplemented, DnsResponseCodeText.NotImp },
            { DnsResponseCode.Refused, DnsResponseCodeText.Refused },
            { DnsResponseCode.ExistingDomain, DnsResponseCodeText.YXDomain },
            { DnsResponseCode.ExistingResourceRecordSet, DnsResponseCodeText.YXRRSet },
            { DnsResponseCode.MissingResourceRecordSet, DnsResponseCodeText.NXRRSet },
            { DnsResponseCode.NotAuthorized, DnsResponseCodeText.NotAuth },
            { DnsResponseCode.NotZone, DnsResponseCodeText.NotZone },
            { DnsResponseCode.BadVersionOrBadSignature, DnsResponseCodeText.BADVERS },
            { DnsResponseCode.BadKey, DnsResponseCodeText.BADKEY },
            { DnsResponseCode.BadTime, DnsResponseCodeText.BADTIME },
            { DnsResponseCode.BadMode, DnsResponseCodeText.BADMODE },
            { DnsResponseCode.BadName, DnsResponseCodeText.BADNAME },
            { DnsResponseCode.BadAlgorithm, DnsResponseCodeText.BADALG },
            { DnsResponseCode.BadTruncation, DnsResponseCodeText.BADTRUNC },
            { DnsResponseCode.BadCookie, DnsResponseCodeText.BADCOOKIE },
        };

        public static string GetErrorText(DnsResponseCode code)
        {
            if (!errors.ContainsKey(code))
            {
                return Unassigned;
            }

            return errors[code];
        }
    }
}