//
//  Packet2Kick.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using System;
using System.Text;

namespace TShockProxy.Protocol.Packets
{
    public class Packet2Kick : BasePacket
    {
        private string Message;

        public Packet2Kick(string message) : base(0x02)
        {
            this.Message = message;
        }

        public override void ReadPayload(IByteBuffer buf)
        {
            Message = ReadString(buf);

        }

        public override void WritePayload(IByteBuffer buf)
        {
            WriteString(Message, buf);
        }
    }
}

