using MultiplayerGameServer.Network;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Yitter.IdGenerator;

namespace MultiplayerGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new IdGeneratorOptions
            {
                WorkerId = 1,
                WorkerIdBitLength = 6,
                SeqBitLength = 6,
                Method = 1
            };
            YitIdHelper.SetIdGenerator(options);

            Server server = new Server(6666);
            Console.WriteLine("服务端启动...");
            Console.Read();
        }
    }
}