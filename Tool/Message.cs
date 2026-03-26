using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocal;
using Google.Protobuf;

namespace MultiplayerGameServer.Tool
{
    internal class Message
    {
        private byte[] buffer = new byte[1024];
        private int startIndex;

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public int StartIndex
        {
            get { return startIndex; }
        }

        public int RemSize
        {
            get { return buffer.Length - startIndex; }
        }

        public void ReadBuffer(int _len)
        {
            startIndex += _len;

            if (startIndex <= 4)
            {
                return;
            }

            int _count = BitConverter.ToInt32(buffer, 0);

            while (startIndex >= _count + 4)
            {
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, _count);
                Array.Copy(buffer, _count + 4, buffer, 0, startIndex - _count - 4);
                startIndex -= _count + 4;
            }

            while (true)
            {
                if (startIndex >= _count + 4)
                {
                    //执行一段代码
                }
                else
                {
                    break;
                }
            }
        }

        public static byte[] PackData(MainPack pack)
        {
            byte[] _data = pack.ToByteArray(); //包体
            byte[] _head = BitConverter.GetBytes(_data.Length); //包头
            return _head.Concat(_data).ToArray();
        }
    }
}
