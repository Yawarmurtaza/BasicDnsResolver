using System.Net;
using System.Net.Sockets;

namespace InfraServiceJobPackage.Library.DnsHelper
{
    public interface IUdpSocketProxy
    {
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }
        void SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);
    }

    public class UdpSocketProxy : IUdpSocketProxy
    {
        private readonly Socket socket;
        public UdpSocketProxy()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public UdpSocketProxy(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            socket = new Socket(addressFamily, socketType, protocolType);
        }

        public int ReceiveTimeout { get => socket.ReceiveTimeout; set => socket.ReceiveTimeout = value; }
        public int SendTimeout { get => socket.SendTimeout; set => socket.SendTimeout = value; }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            return socket.Receive(buffer, offset, size, socketFlags);
        }

        public void SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            socket.SendTo(buffer, offset, size, socketFlags, remoteEP);
        }
    }
}