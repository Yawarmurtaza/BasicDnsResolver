using System;
using System.Net;
using System.Text;

namespace InfraServiceJobPackage.Library.DnsHelper.MessageWriters
{
    public class DnsDatagramWriter : IDnsDatagramWriter
    {
        /// <summary> Queries can only be 255 octets + some header bytes, so that size is pretty safe... </summary>
        private const int BufferSize = 1024;
        private const byte DotByte = 46;
        private readonly PooledBytes _pooledBytes;
        private ArraySegment<byte> _buffer;
        private int Index;
        public ArraySegment<byte> Data
        {
            get
            {
                return new ArraySegment<byte>(_buffer.Array, 0, Index);
            }
        }

        public DnsDatagramWriter()
        {
            _pooledBytes = new PooledBytes(BufferSize);
            // https://stackoverflow.com/questions/4600024/what-is-the-use-of-the-arraysegmentt-class
            _buffer = new ArraySegment<byte>(_pooledBytes.Buffer, 0, BufferSize);
        }

        public void WriteHostName(string queryName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(queryName);
            int lastOctet = 0;
            int index = 0;
            if (bytes.Length <= 1)
            {
                WriteByte(0);
                return;
            }
            foreach (byte b in bytes)
            {
                if (b == DotByte)
                {
                    WriteByte((byte)(index - lastOctet)); // length
                    WriteBytes(bytes, lastOctet, index - lastOctet); // label e.g. www e.g. microsoft e.g. com 
                    lastOctet = index + 1;
                }

                index++;
            }

            WriteByte(0);
        }

        public void WriteInt16NetworkOrder(short value)
        {
            short s = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes(s);
            WriteBytes(bytes, bytes.Length);
        }

        public void WriteUInt16NetworkOrder(ushort value)
        {
            WriteInt16NetworkOrder((short)value);
        }

        public void WriteUInt32NetworkOrder(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)value));
            WriteBytes(bytes, bytes.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pooledBytes?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>Writes one byte in the buffer array (byte[]) in current position of index and increments the index.</summary>
        /// <param name="b">The byte to write in the buffer (byte[]).</param>
        private void WriteByte(byte b)
        {
            _buffer.Array[_buffer.Offset + Index++] = b;
        }

        private void WriteBytes(byte[] data, int length)
        {
            WriteBytes(data, 0, length);
        }

        private void WriteBytes(byte[] data, int dataOffset, int length)
        {
            Buffer.BlockCopy(data, dataOffset, _buffer.Array, _buffer.Offset + Index, length);
            Index += length;
        }
    }
}