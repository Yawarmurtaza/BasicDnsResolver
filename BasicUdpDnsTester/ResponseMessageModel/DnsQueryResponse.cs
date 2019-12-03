using System.Collections.Generic;
using System.Linq;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;
using BasicUdpDnsTester.ConsoleRunner.RequestMessageModel;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    /// <summary>
    /// The response returned by any query performed by <see cref="IDnsQuery"/> with all answer sections, header and message information.
    /// </summary>
    public class DnsQueryResponse : IDnsQueryResponse
    {
        private int? _hashCode;

        /// <summary>
        /// Gets a list of additional records.
        /// </summary>
        public IReadOnlyList<DnsResourceRecord> Additionals { get; }

        /// <summary>
        /// Gets a list of all answers, additional and authority records.
        /// </summary>
        public IEnumerable<DnsResourceRecord> AllRecords
        {
            get
            {
                return Answers.Concat(Additionals).Concat(Authorities);
            }
        }       

        /// <summary>
        /// Gets a list of answer records.
        /// </summary>
        public IReadOnlyList<DnsResourceRecord> Answers { get; }

        /// <summary>
        /// Gets a list of authority records.
        /// </summary>
        public IReadOnlyList<DnsResourceRecord> Authorities { get; }

        /// <summary>
        /// Returns a string value representing the error response code in case an error occured,
        /// otherwise '<see cref="DnsResponseCode.NoError"/>'.
        /// </summary>
        public string ErrorMessage => DnsResponseCodeText.GetErrorText(Header.ResponseCode);

        /// <summary>
        /// Gets the header of the response.
        /// </summary>
        public DnsResponseHeader Header { get; }

        /// <summary>
        /// Gets the list of questions.
        /// </summary>
        public IReadOnlyList<DnsQuestion> Questions { get; }

        //internal LookupClientAudit Audit { get; }
        

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is DnsQueryResponse response))
            {
                return false;
            }

            return
                Header.ToString().Equals(response.Header.ToString())
                && string.Join("", Questions).Equals(string.Join("", response.Questions))
                && string.Join("", AllRecords).Equals(string.Join("", response.AllRecords));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                _hashCode = (Header + string.Join("", Questions) + string.Join("", AllRecords)).GetHashCode();
            }

            return _hashCode.Value;
        }
    }
}