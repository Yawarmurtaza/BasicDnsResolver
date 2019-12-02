using System.Collections.Generic;
using BasicUdpDnsTester.ConsoleRunner.DnsProtocol;

namespace BasicUdpDnsTester.ConsoleRunner.ResponseMessageModel
{
    public static class DnsResponseCodeText
    {
        private const string BADALG = "Algorithm not supported";
        private const string BADCOOKIE = "Bad/missing Server Cookie";
        private const string BADKEY = "Key not recognized";
        private const string BADMODE = "Bad TKEY Mode";
        private const string BADNAME = "Duplicate key name";
        private const string BADTIME = "Signature out of time window";
        private const string BADTRUNC = "Bad Truncation";
        private const string BADVERS = "Bad OPT Version";
        private const string FormErr = "Format Error";
        private const string NoError = "No Error";
        private const string NotAuth = "Server Not Authoritative for zone or Not Authorized";
        private const string NotImp = "Not Implemented";
        private const string NotZone = "Name not contained in zone";
        private const string NXDomain = "Non-Existent Domain";
        private const string NXRRSet = "RR Set that should exist does not";
        private const string Refused = "Query Refused";
        private const string ServFail = "Server Failure";
        private const string Unassigned = "Unknown Error";
        private const string YXDomain = "Name Exists when it should not";
        private const string YXRRSet = "RR Set Exists when it should not";

        private static readonly Dictionary<DnsResponseCode, string> errors = new Dictionary<DnsResponseCode, string>()
        {
            { DnsResponseCode.NoError, NoError },
            { DnsResponseCode.FormatError, FormErr },
            { DnsResponseCode.ServerFailure, ServFail },
            { DnsResponseCode.NotExistentDomain, NXDomain },
            { DnsResponseCode.NotImplemented, NotImp },
            { DnsResponseCode.Refused, Refused },
            { DnsResponseCode.ExistingDomain, YXDomain },
            { DnsResponseCode.ExistingResourceRecordSet, YXRRSet },
            { DnsResponseCode.MissingResourceRecordSet, NXRRSet },
            { DnsResponseCode.NotAuthorized, NotAuth },
            { DnsResponseCode.NotZone, NotZone },
            { DnsResponseCode.BadVersionOrBadSignature, BADVERS },
            { DnsResponseCode.BadKey, BADKEY },
            { DnsResponseCode.BadTime, BADTIME },
            { DnsResponseCode.BadMode, BADMODE },
            { DnsResponseCode.BadName, BADNAME },
            { DnsResponseCode.BadAlgorithm, BADALG },
            { DnsResponseCode.BadTruncation, BADTRUNC },
            { DnsResponseCode.BadCookie, BADCOOKIE },
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