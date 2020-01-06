namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    /*
     *
     * Reference: [RFC6895][RFC1035]
        0	    Query	                            [RFC1035]
        1	    IQuery (Inverse Query, OBSOLETE)	[RFC3425]
        2	    Status	                            [RFC1035]
        3	    Unassigned
        4	    Notify	                            [RFC1996]
        5	    Update	                            [RFC2136]
        6-15	Unassigned
     * */

    /// <summary> Specifies kind of query in this message. This value is set by the originator of a query and copied into the response. </summary>
    public enum DnsOpCode : short
    {
        /// <summary> A standard query. </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc1035">RFC 1035</seealso>
        Query
    }
}