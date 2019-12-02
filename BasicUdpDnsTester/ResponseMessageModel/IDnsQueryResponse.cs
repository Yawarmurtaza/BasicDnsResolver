using System.Collections.Generic;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    /// <summary>
    /// Contract defining the result of a query performed by <see cref="IDnsQuery"/>.
    /// </summary>
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
        /// Gets the audit trail if <see cref="ILookupClient.EnableAuditTrail"/>. as set to <c>true</c>, <c>null</c> otherwise.
        /// </summary>
        /// <value>
        /// The audit trail.
        /// </value>
        string AuditTrail { get; }

        /// <summary>
        /// Returns a string value representing the error response code in case an error occured,
        /// otherwise '<see cref="DnsResponseCode.NoError"/>'.
        /// </summary>
        string ErrorMessage { get; }
    }
}