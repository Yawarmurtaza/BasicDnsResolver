﻿using System;
using System.Linq;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    /// <summary>
    /// The header section of a <see cref="IDnsQueryResponse"/>.
    /// </summary>
    public class  DnsResponseHeader
    {
        private readonly ushort _flags;

        /// <summary> Gets the number of additional records. </summary>
        public int AdditionalCount { get; }

        /// <summary> Gets the number of answer records.</summary>
        public int AnswerCount { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has authority answers.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has authority answers; otherwise, <c>false</c>.
        /// </value>
        public bool HasAuthorityAnswer => HasFlag(DnsHeaderFlag.HasAuthorityAnswer);

        internal DnsHeaderFlag HeaderFlags => (DnsHeaderFlag)_flags;

        /// <summary> Gets the identifier. It is used to match the response with the query that was sent by the client.
        /// We use different (random) identifier number each time sending a query to the DNS server.
        /// The DNS server duplicates this number in the corresponding response. This helps up to pair up the query and response messages.</summary>
        public int Identifier { get; }

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

        /// <summary> Gets the number of questions records. </summary>
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
        //[CLSCompliant(false)]
        public DnsResponseHeader(int id, ushort flags, int questionCount, int answerCount, int additionalCount, int serverCount)
        {
            Identifier = id;
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
            var head = $";; ->>HEADER<<- opcode: {OPCode}, status: {DnsResponseCodeText.GetErrorText(ResponseCode)}, id: {Identifier}";
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
}