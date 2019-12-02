using System.Collections.Generic;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
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