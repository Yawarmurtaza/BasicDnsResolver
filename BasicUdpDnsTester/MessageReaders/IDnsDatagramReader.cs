namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

    public interface IDnsDatagramReader
    {
        int Index { get; set; }

        void Advance(int length);
        byte ReadByte();
        IPAddress ReadIPAddress();
        ICollection<ArraySegment<byte>> ReadLabels();
        IDnsString ReadQuestionQueryString();
        ushort ReadUInt16NetworkOrder();
        uint ReadUInt32NetworkOrder();
    }
}