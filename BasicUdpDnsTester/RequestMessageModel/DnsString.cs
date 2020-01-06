using System;
using System.Globalization;

namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    /// <summary>The DnsString type is used to normalize and validate domain names and labels.</summary>
    public class DnsString : IDnsString
    {
        private const char Dot = '.';
        private const string DotStr = ".";

        /// <summary>The ACE prefix indicates that the domain name label contains not normally supported characters and that the label has been encoded.</summary>
        public const string ACEPrefix = "xn--";

        /*
         * 4.1.2. Question section format  
         * QNAME   
         *   a domain name represented as a sequence of labels, where
         *   each label consists of a length octet followed by that
         *   number of octets.  The domain name terminates with the
         *   zero length octet for the null label of the root.
         *   www.microsoft.com has 3 labels.
         *   Total length allowed for 1 label is 63
         */
        /// <summary>The maximum length in bytes for one label. </summary>
        public const int MaxLabelLength = 63;

        /// <summary>The maximum supported total length in bytes for a domain name. The calculation of the actual
        /// bytes this <see cref="DnsString"/> consumes includes all bytes used for to encode it as octet string.</summary>
        public const int MaxQueryLength = 255;

        /// <summary>The root label ".".</summary>
        public IDnsString RootLabel { get { return new DnsString(DotStr, DotStr); } }

        /// <summary>Gets the original value.</summary>
        public string Original { get; }

        /// <summary>Gets the validated and eventually modified value.</summary>
        public string Value { get; }

        public DnsString()
        {
            Original = Value = DotStr;
        }

        public DnsString(string original, string value)
        {
            Original = original;
            Value = value;
        }

        /// <summary>Performs an implicit conversion from DnsString string.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(DnsString name) => name?.Value;

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.ToString().Equals(Value);
        }

        /// <summary>Parses the given query and validates all labels.</summary>
        /// <remarks>An empty string will be interpreted as root label.</remarks>
        /// <param name="query">A domain name.</param>
        /// <returns>The IDnsString representing the given query.</returns>
        /// <exception cref="ArgumentNullException">If query is null.</exception>
        public IDnsString Parse(string query)
        {
            #region Validation.

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }           

            if (query.Length > 1 && query[0] == Dot)
            {
                throw new ArgumentException($"'{query}' is not a legal name, found leading root label.", nameof(query));
            }

            if (query.Length == 0 || query.Length == 1 && query.Equals(DotStr))
            {
                return RootLabel;
            }
            #endregion Validation.

            int charCount = 0;
            // keeps tract of the length of current label in the given query.
            int currentLabelLength = 0;
            int labelsCount = 0;

            foreach(char nextChar in query)            
            {                
                if (nextChar.Equals(Dot)) // check if current char from query is a "." 
                {
                    // it means we have completion of a label in the query since they are saperated by a "." - www.microsoft.com

                    if (currentLabelLength > MaxLabelLength)
                    {
                        throw new ArgumentException($"Label '{labelsCount + 1}' is longer than {MaxLabelLength} bytes.", nameof(query));
                    }

                    labelsCount++;
                    currentLabelLength = 0;
                }
                else
                {
                    currentLabelLength++;
                    charCount++;
                    #region
                    /*if (!(c == '-' || c == '_' ||
                          c >= 'a' && c <= 'z' ||
                          c >= 'A' && c <= 'Z' ||
                          c >= '0' && c <= '9'))
                    {
                        try
                        {
                            string result = IDN.GetAscii(query);
                            if (result[result.Length - 1] != Dot)
                            {
                                result += Dot;
                            }

                            return new DnsString(query, result);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException($"'{query}' is not a valid hostname.", nameof(query), ex);
                        }
                    }*/
                    #endregion
                }
            }

            // check the last label
            if (currentLabelLength > 0)
            {
                labelsCount++;
                // check again the last label max length
                if (currentLabelLength > MaxLabelLength)
                {
                    throw new ArgumentException($"Label '{labelsCount}' is longer than {MaxLabelLength} bytes.", nameof(query));
                }
            }

            // octets length length bit per label + 2(start + end)
            if (charCount + labelsCount + 1 > MaxQueryLength)
            {
                throw new ArgumentException($"Octet length of '{query}' exceeds maximum of {MaxQueryLength} bytes.", nameof(query));
            }

            // if the given query doesnt end with a '.', its added.
            if (query[query.Length - 1] != Dot)
            {
                return new DnsString(query, query + Dot);
            }

            return new DnsString(query, query);
        }

        /// <summary>Transforms names with the <see cref="ACEPrefix"/> to the unicode variant and adds a trailing '.' at the end if not present. The original value will be kept in this instance in case it is needed.</summary>
        /// <remarks>/// The method does not parse the domain name unless it contains a <see cref="ACEPrefix"/>./// </remarks>
        /// <param name="query">The value to check.</param>
        /// <returns>The <see cref="DnsString"/> representation.</returns>
        public IDnsString FromResponseQueryString(string query)
        {
            if (query.Length == 0 || query[query.Length - 1] != Dot)
            {
                query += DotStr;
            }

            if (query.Contains(ACEPrefix))
            {
                IdnMapping idnMapping = new IdnMapping();
                string unicode = idnMapping.GetUnicode(query);
                return new DnsString(unicode, query);
            }

            return new DnsString(query, query);
        }
    }
}