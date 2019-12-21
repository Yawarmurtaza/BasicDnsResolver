namespace InfraServiceJobPackage.Library.DnsHelper.DnsProtocol
{
    public static class DnsHeader
    {
        /// <summary>
        /// Represents the OpCode part of the header. Defines the type of query, 0 = standard, 1 = inverse, 2 = if its a server status request. 
        /// </summary>
        public static readonly ushort OPCodeMask = 0x7800;
        public static readonly ushort OPCodeShift = 11;

        /// <summary>
        /// Represents the status of the error in response. Its a 4 bit field. Only authoritative server can populate this.
        /// </summary>
        public static readonly ushort RCodeMask = 0x000F;
    }
}