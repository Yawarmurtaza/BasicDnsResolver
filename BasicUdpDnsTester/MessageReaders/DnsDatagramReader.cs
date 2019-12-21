namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel;

    public class DnsDatagramReader : IDnsDatagramReader
    {
        public const int IPv4Length = 4;
        private const byte ReferenceByte = 0xc0;
        
        private readonly byte[] _ipV4Buffer = new byte[4];
        private readonly IDnsString dnsString;

        private readonly ArraySegment<byte> _data;
        private readonly int _count;
        private int _index;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value < 0 || value > _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _index = value;
            }
        }

        public DnsDatagramReader(ArraySegment<byte> data, IDnsString dnsString, int startIndex = 0)
        {
            _data = data;
            _count = data.Count;
            Index = startIndex;
            this.dnsString = dnsString;
        }

        public byte ReadByte()
        {
            try
            {
                return _data.Array[_data.Offset + _index++];
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Cannot read byte {_index}, out of range.");
            }
        }        

        public IPAddress ReadIPAddress()
        {
            try
            {
                _ipV4Buffer[0] = _data.Array[_data.Offset + _index];
                _ipV4Buffer[1] = _data.Array[_data.Offset + _index + 1];
                _ipV4Buffer[2] = _data.Array[_data.Offset + _index + 2];
                _ipV4Buffer[3] = _data.Array[_data.Offset + _index + 3];

                Index += IPv4Length;
                return new IPAddress(_ipV4Buffer);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Reading IPv4 address, expected {IPv4Length} bytes.");
            }
        }

        public void Advance(int length)
        {
            Index += length;
        }

        // only used by the DnsQuestion as we don't expect any escaped chars in the actual query posted to and send back from the DNS Server (not supported).
        public IDnsString ReadQuestionQueryString()
        {
            StringBuilder result = StringBuilderObjectPool.Default.Get();
            foreach (var labelArray in ReadLabels())
            {
                var label = Encoding.UTF8.GetString(labelArray.Array, labelArray.Offset, labelArray.Count);
                result.Append(label);
                result.Append(".");
            }

            string value = result.ToString();
            StringBuilderObjectPool.Default.Return(result);
            return dnsString.FromResponseQueryString(value);
        }

        public ICollection<ArraySegment<byte>> ReadLabels()
        {
            var result = new List<ArraySegment<byte>>(10);

            // read the length byte for the label, then get the content from offset+1 to length
            // proceed till we reach zero length byte.
            byte length;
            while ((length = ReadByte()) != 0)
            {
                // respect the reference bit and lookup the name at the given position
                // the reader will advance only for the 2 bytes read.
                if ((length & ReferenceByte) != 0)
                {
                    int subIndex = (length & 0x3f) << 8 | ReadByte();
                    if (subIndex >= _data.Array.Length - 1)
                    {
                        // invalid length pointer, seems to be actual length of a label which exceeds 63 chars...
                        // get back one and continue other labels
                        Index--;
                        result.Add(_data.SubArray(_index, length));
                        Index += length;
                        continue;
                    }

                    DnsDatagramReader subReader = new DnsDatagramReader(_data.SubArrayFromOriginal(subIndex), dnsString);
                    ICollection<ArraySegment<byte>> newLabels = subReader.ReadLabels();
                    result.AddRange(newLabels); // add range actually much faster than Concat and equal to or faster than foreach.. (array copy would work maybe)
                    return result;
                }

                if (Index + length >= _count)
                {
                    throw new IndexOutOfRangeException(
                        $"Found invalid label position '{Index - 1}' with length '{length}' in the source data.");
                }

                var label = _data.SubArray(_index, length);

                // maybe store orignial bytes in this instance too?
                result.Add(label);

                Index += length;
            }

            return result;
        }

        public ushort ReadUInt16NetworkOrder()
        {
            if (_count < _index + 2)
            {
                throw new IndexOutOfRangeException("Cannot read more data.");
            }

            return (ushort)(ReadByte() << 8 | ReadByte());
        }

        public uint ReadUInt32NetworkOrder()
        {
            return (uint)(ReadUInt16NetworkOrder() << 16 | ReadUInt16NetworkOrder());
        }
    }
}