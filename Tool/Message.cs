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

        public void ReadBuffer(int len)
        {
            startIndex += len;

            if (startIndex <= 4)
            {
                return;
            }

            int count = BitConverter.ToInt32(buffer, 0);

            while (startIndex >= count + 4)
            {
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, count);
                Array.Copy(buffer, count + 4, buffer, 0, startIndex - count - 4);
                startIndex -= count + 4;
            }

            while (true)
            {
                if (startIndex >= count + 4)
                {
                    //执行一段代码
                }
                else
                {
                    break;
                }
            }
        }
    }
}
