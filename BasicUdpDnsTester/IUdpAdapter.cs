using System.Net;
using System.Net.Sockets;

namespace InfraServiceJobPackage.Library.DnsHelper
{
    public interface IUdpAdapter
    {
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }
        void SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);
    }
}