using System.Net;

namespace dot.Mail.Interfaces
{
    public interface INetworkLayerClient
    {
        void Connect(IPAddress ip, int port);
        void Send(byte[] bytes);
        byte[] Read();
    }
}
