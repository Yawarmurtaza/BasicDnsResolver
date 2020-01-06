using System;

namespace InfraServiceJobPackage.Library.DnsHelper.MessageWriters
{
    public interface IDnsDatagramWriter : IDisposable
    {
        ArraySegment<byte> Data { get; }
        
        void WriteHostName(string queryName);
        void WriteInt16NetworkOrder(short value);
      
        void WriteUInt16NetworkOrder(ushort value);
        void WriteUInt32NetworkOrder(uint value);
    }
}