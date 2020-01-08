namespace InfraServiceJobPackage.Library.DnsHelper
{
    using System.Net;
    using System.Net.Sockets;

    public class UdpAdapter : IUdpAdapter
    {
        private readonly Socket socket;
        public UdpAdapter()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public UdpAdapter(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
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