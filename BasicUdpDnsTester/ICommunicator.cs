namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System;

    /// <summary> Allows access to the remote systems using Tcp and Udp sockets. Its essentially a wrapper around UdpClient type. </summary>
    public interface IUdpCommunicator : IDisposable
    {
        IUdpAdapter Client { get; }
    }

    public class UdpCommunicator : IUdpCommunicator
    {
        public UdpCommunicator()
        {
            Client = new UdpAdapter();
        }

        public IUdpAdapter Client { get; private set; }
        
        public void Dispose()
        {
            Client = null;
        }
    }
}