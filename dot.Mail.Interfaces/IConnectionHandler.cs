using System.Net.Sockets;
using System.Threading.Tasks;

namespace dot.Mail.Interfaces
{
    public interface IConnectionHandler
    {
        // Must use Task return type for async/await methods
        Task HandleConnectionAsync(TcpClient tcpClient);
    }
}
