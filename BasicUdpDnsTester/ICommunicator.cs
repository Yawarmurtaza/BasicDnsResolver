using System;
using System.Net.Sockets;

namespace InfraServiceJobPackage.Library.DnsHelper
{
    /// <summary>
    /// Allows access to the remote systems using Tcp and Udp sockets. Its essentially a wrapper around UdpClient type.
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        IUdpSocketProxy Client { get; }
    }

    public class UdpCommunicator : ICommunicator
    {
        private IUdpSocketProxy udpClient;

        public UdpCommunicator()
        {
            udpClient = new UdpSocketProxy();
        }

        public IUdpSocketProxy SetAddressFamilyToIpv6()
        {
            udpClient = new UdpSocketProxy(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            return udpClient;
        }

        public IUdpSocketProxy Client
        {
            get
            {
                return udpClient;
            }
        }


        public void Dispose()
        {
            udpClient = null;
        }
    }
}