using System;
using System.Collections.Generic;
using System.Net;

namespace InfraServiceJobPackage.Library.DnsHelper.ResponseMessageModel
{
    /// <summary>Represents a name server instance. Also, comes with some static methods to resolve name servers from the local network configuration.</summary>
    public class NameServer : IEquatable<NameServer>
    {
        /// <summary>The default DNS server port.</summary>
        public const int DefaultPort = 53;

        /// <summary>Initializes a new instance of the <see cref="NameServer"/> class.</summary>
        /// <param name="endPoint">The name server endpoint.</param>
        public NameServer(IPAddress endPoint) : this(new IPEndPoint(endPoint, DefaultPort))
        {
        }
        
        /// <summary>Initializes a new instance of the <see cref="NameServer"/> class.</summary>
        /// <param name="endPoint">The name server endpoint.</param>        
        public NameServer(IPEndPoint endPoint)
        {
            IPEndPoint = endPoint;
        }

        /// <summary>Initializes a new instance of the <see cref="NameServer"/> class from a <see cref="IPEndPoint"/>.</summary>
        /// <param name="endPoint">The endpoint.</param>
        public static implicit operator NameServer(IPEndPoint endPoint)
        {
            return endPoint == null ? null : new NameServer(endPoint);            
        }

        /// <summary>Initializes a new instance of the <see cref="NameServer"/> class from a <see cref="IPAddress"/>.</summary>
        /// <param name="address">The address.</param>
        public static implicit operator NameServer(IPAddress address)
        {
            return address == null ? null : new NameServer(address);
        }
        
        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port => IPEndPoint.Port;

        public IPEndPoint IPEndPoint { get; }
        
        public override string ToString()
        {
            return $"{IPEndPoint.Address}:{Port} (Udp: 512)";
        }

        /// <inheritdocs />
        public override bool Equals(object obj)
        {
            return Equals(obj as NameServer);
        }

        /// <inheritdocs />
        public bool Equals(NameServer other)
        {
            return other != null
                   && EqualityComparer<IPEndPoint>.Default.Equals(IPEndPoint, other.IPEndPoint);
        }

        /// <inheritdocs />
        public override int GetHashCode()
        {
            return EqualityComparer<IPEndPoint>.Default.GetHashCode(IPEndPoint);
        }
    }
}