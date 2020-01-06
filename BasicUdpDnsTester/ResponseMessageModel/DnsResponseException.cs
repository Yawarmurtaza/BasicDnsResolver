namespace InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel
{
    using System;
    using DnsProtocol;

    /// <summary> A DnsClient specific exception transporting additional information about the query causing this exception. </summary>
    /// <seealso cref="System.Exception" />
    public class DnsResponseException : Exception
    {
        /// <summary> Gets the response code. </summary>
        /// <value> The response code. </value>
        public DnsResponseCode Code { get; }

        /// <summary> Gets a human readable error message. </summary>
        /// <value> The error message. </value>
        public string DnsError { get; }

        /// <summary> Initializes a new instance of the DnsResponseException class  with Code set to DnsResponseCode.Unassigned and a custom message </summary>
        public DnsResponseException(string message) : base(message)
        {
            Code = DnsResponseCode.Unassigned;
            DnsError = DnsResponseCodeText.GetErrorText(Code);
        }
    }
}