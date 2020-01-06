namespace InfraServiceJobPackage.Library.DnsHelper.MessageReaders
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using RequestMessageModel;

    public class DnsDatagramReader : IDnsDatagramReader
    {
        private const byte ReferenceByte = 0xc0;
        private readonly IDnsString dnsString;
        //private readonly IStringBuilderObjectPool stringBuilder;

        /// <summary>Gets the index of datagram reader. </summary>
        public int Index { get; private set; }

        public DnsDatagramReader(IDnsString dnsString)
        {
            this.dnsString = dnsString;
            //this.stringBuilder = stringBuilder;
            Index = 0;
        }

        public IPAddress ReadIPAddress(ArraySegment<byte> data)
        {
            const int IPv4Length = 4;
            try
            {
                byte[] ipV4Buffer = new byte[4];
                for (int i = 0; i < 4; Index++, i++)
                {
                    ipV4Buffer[i] = data.Array[data.Offset + Index];
                }

                return new IPAddress(ipV4Buffer);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException($"Error while reading IPv4 address, expected {IPv4Length} bytes. {ex.Message}");
            }
        }

        // only used by the DnsQuestion as we don't expect any escaped chars in the actual query posted to and send back from the DNS Server (not supported).
        public IDnsString ReadQuestionQueryString(ArraySegment<byte> data)
        {
            // StringBuilder result = stringBuilder.Get();
            StringBuilder result = new StringBuilder();
            foreach (ArraySegment<byte> labelArray in ReadLabels(data))
            {
                string label = Encoding.UTF8.GetString(labelArray.Array, labelArray.Offset, labelArray.Count);
                result.Append(label);
                result.Append(".");
            }

            string value = result.ToString();
            // stringBuilder.Return(result);
            return dnsString.FromResponseQueryString(value);
        }


        private ICollection<ArraySegment<byte>> ReadLabels(ArraySegment<byte> data)
        {
            List<ArraySegment<byte>> result = new List<ArraySegment<byte>>(10);

            // read the length byte for the label, then get the content from offset+1 to length
            // proceed till we reach zero length byte.
            byte length;
            while ((length = ReadByte(data)) != 0)
            {
                // respect the reference bit and lookup the name at the given position
                // the reader will advance only for the 2 bytes read.
                if ((length & ReferenceByte) != 0)
                {
                    int subIndex = (length & 0x3f) << 8 | ReadByte(data);
                    if (subIndex >= data.Array.Length - 1)
                    {
                        // invalid length pointer, seems to be actual length of a label which exceeds 63 chars...
                        // get back one and continue other labels
                        Index--;
                        result.Add(data.SubArray(Index, length));
                        Index += length;
                        continue;
                    }

                    DnsDatagramReader subReader = new DnsDatagramReader(dnsString);
                    ArraySegment<byte> subData = data.SubArrayFromOriginal(subIndex);
                    ICollection <ArraySegment<byte>> newLabels = subReader.ReadLabels(subData); // recursion.
                    result.AddRange(newLabels); // add range actually much faster than Concat and equal to or faster than foreach.. (array copy would work maybe)
                    return result;
                }

                if (Index + length >= data.Count)
                {
                    throw new IndexOutOfRangeException($"Found invalid label position '{Index - 1}' with length '{length}' in the source data.");
                }

                ArraySegment<byte> label = data.SubArray(Index, length);

                // maybe store original bytes in this instance too?
                result.Add(label);
                Index += length;
            }

            return result;
        }

        public ushort ReadUInt16NetworkOrder(ArraySegment<byte> data)
        {
            if (data.Count < Index + 2)
            {
                throw new IndexOutOfRangeException("Cannot read more data.");
            }

            ushort result = (ushort)(ReadByte(data) << 8 | ReadByte(data));
            return result;
        }

        public uint ReadUInt32NetworkOrder(ArraySegment<byte> data)
        {
            uint result = (uint)(ReadUInt16NetworkOrder(data) << 16 | ReadUInt16NetworkOrder(data));
            return result;
        }

        /// <summary>Moves the index from its current position to given length. E.g. current = 10, length = 5, index = 15.</summary>
        /// <param name="length">Length to move index to.</param>
        public void Advance(int length)
        {
            Index += length;
        }

        /// <summary>Reads a byte on the current position of index of given array segment and advances the index by 1.</summary>
        /// <param name="data">Data to read a byte from.</param>
        /// <returns>A byte from given data on current index position.</returns>
        private byte ReadByte(ArraySegment<byte> data)
        {
            try
            {
                byte requiredByte = data.Array[data.Offset + Index++];
                return requiredByte;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Cannot read byte {Index}, out of range.");
            }
        }
    }
}