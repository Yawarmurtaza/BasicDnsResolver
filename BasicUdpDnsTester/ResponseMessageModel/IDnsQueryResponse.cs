namespace InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel
{
    using System.Collections.Generic;
    using InfraServiceJobPackage.Library.DnsHelper.Records;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

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