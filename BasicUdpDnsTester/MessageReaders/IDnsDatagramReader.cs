namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using System.Net;
    using RequestMessageModel;

    public interface IDnsDatagramReader
    {
        /// <summary>Gets the index of datagram reader. </summary>
        int Index { get; }

        /// <summary>Moves the index from its current position to given length. E.g. current = 10, length = 5, index = 15.</summary>
        /// <param name="length">Length to move index to.</param>
        void Advance(int length);

        IPAddress ReadIPAddress(ArraySegment<byte> data);

        IDnsString ReadQuestionQueryString(ArraySegment<byte> data);
        ushort ReadUInt16NetworkOrder(ArraySegment<byte> data);

        uint ReadUInt32NetworkOrder(ArraySegment<byte> data);
        
    }
}