using System.Collections.Generic;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    public interface IDnsQueryResponse
    {
        /// <summary>
        /// Gets the list of questions.
        /// </summary>
        IReadOnlyList<DnsQuestion> Questions { get; }

        /// <summary>
        /// Gets a list of additional records.
        /// </summary>
        IReadOnlyList<DnsResourceRecord> Additionals { get; }

        /// <summary>
        /// Gets a list of answer records.
        /// </summary>
        IReadOnlyList<DnsResourceRecord> Answers { get; }

        /// <summary>
        /// Gets a list of authority records.
        /// </summary>
        IReadOnlyList<DnsResourceRecord> Authorities { get; }

        /// <summary>
        /// Returns a string value representing the error response code in case an error occured,
        /// otherwise '<see cref="DnsResponseCode.NoError"/>'.
        /// </summary>
        string ErrorMessage { get; }
    }
}