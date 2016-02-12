//
//  PacketInputStream.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using LibMultiplicity;
using LibMultiplicity.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TShockProxy.Protocol.Packets
{
    public class PacketInputStream
    {
        private BinaryReader reader;
        private TrackingInputStream tracker;

        public PacketInputStream(NetworkStream stream)
        {
            tracker = new TrackingInputStream(stream);
            reader = new BinaryReader(tracker);
        }

        public byte[] readPacket()
        {
            tracker.Out.ResetReaderIndex();
            tracker.Out.ResetWriterIndex();

            tracker.ReadByte();
            tracker.ReadByte();
            int id = tracker.ReadByte();
            if (id == -1)
            {
                throw new EndOfStreamException();
            }
            if (! TerrariaPacket.deserializerMap.ContainsKey((PacketTypes)id))
            {
                throw new ArgumentException("No packet id: 0x" + id.ToString("X2"));
            }

            return tracker.Out.ToArray();
        }

        private class TrackingInputStream : MemoryStream
        {
            public IByteBuffer Out = Unpooled.Buffer();
            private NetworkStream Wrapped;

            public TrackingInputStream(NetworkStream wrapped)
            {
                this.Wrapped = wrapped;
            }

            public override int ReadByte()
            {
                int Ret = Wrapped.ReadByte();
                Out.WriteInt(Ret);
                return Ret;
            }
        }
    }
}