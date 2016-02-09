//
//  PluginMessage.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using System;
using System.IO;

namespace TShockProxy.Protocol.Packets
{
    public class BasePacket : DefinedPacket
    {
        public short Length;
        public byte PacketID;
        public byte[] Data;

        public BasePacket() : this(0)
        {
        }

        public BasePacket(byte packetid)
        {
            this.PacketID = packetid;
            if (Data != null)
            {
                this.Length = (short)(Data.Length + 3);
            }
            else
            {
                this.Length = (short)(3);
            }

        }

        public override void Read(IByteBuffer buf)
        {
            Length = ReadShort(buf);
            PacketID = ReadByte(buf);
            Data = new byte[buf.ReadableBytes];
            ReadPayload(buf);
        }

        public override void Write(IByteBuffer buf)
        {
            buf.SetWriterIndex(2);
            WriteByte(PacketID, buf);
            WritePayload(buf);

            int length = buf.WriterIndex;
            buf.SetWriterIndex(0);
            WriteShort((short)length, buf);
            buf.SetWriterIndex(length);
        }
           

        public override void ReadPayload(IByteBuffer buf)
        {
        }

        public override void WritePayload(IByteBuffer buf)
        {
        }
    }
}

