﻿using System;
using System.Collections.Generic;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    /// <summary>
    /// A DnsClient specific exception transporting additional information about the query causing this exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DnsResponseException : Exception
    {
        /// <summary>
        /// Gets the response code.
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        public DnsResponseCode Code { get; }

        /// <summary>
        /// Gets the audit trail if <see cref="ILookupClient.EnableAuditTrail"/>. as set to <c>true</c>, <c>null</c> otherwise.
        /// </summary>
        /// <value>
        /// The audit trail.
        /// </value>
        public string AuditTrail { get; internal set; }

        /// <summary>
        /// Gets a human readable error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string DnsError { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsResponseException"/> class 
        /// with <see cref="Code"/> set to <see cref="DnsResponseCode.Unassigned"/>
        /// and a custom <paramref name="message"/>.
        /// </summary>
        public DnsResponseException(string message) : base(message)
        {
            Code = DnsResponseCode.Unassigned;
            DnsError = DnsResponseCodeText.GetErrorText(Code);
        }
    }
}