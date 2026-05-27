using MultiplayerGameServer.Network;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiplayerGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(6666);
            Console.Read();
        }
    }
}