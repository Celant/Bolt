//
//  PacketInputStream.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Bolt.Protocol
{
    struct TerrariaPacketHeader
    {
        public short length;
        public PacketTypes type;

        public bool IsValid()
        {
            return length > 0 && length < 0x7FFF;
        }
    }

    public class PacketInputStream
    {
        protected NetworkStream stream;
        private const int kTerrariaPacketHeaderLength = 3;

        private byte[] lastBuffer;

        public PacketInputStream(NetworkStream stream)
        {
            this.stream = stream;
        }

        public byte[] readPacket()
        {
            byte[] stagingBuffer = new byte[kTerrariaPacketHeaderLength];

            /*
             * This method follows two principals:
             * 
             * - Waits indefinitely for three bytes in the network stream which makes up the terraria packet header
             * - Verifies header, and reads n bytes from the strean according to the decoded packet length.
             * 
             * Note that the terraria packet length up to 65kB long, and includes the three header bytes.
             */

            int bytesRead = 0;

            bytesRead = stream.Read(stagingBuffer, 0, kTerrariaPacketHeaderLength);
            if (bytesRead != kTerrariaPacketHeaderLength)
            {
                //throw new System.Exception("Failed to read packet header from stream");
                return new byte[0];
            }

            TerrariaPacketHeader packetHeader = ParseHeader(stagingBuffer, 0);
            if (packetHeader.IsValid() == false)
            {
                Console.WriteLine("[Bolt] Packet is invalid");
            }

            Console.WriteLine ("[Bolt] [PacketInputStream] {0}", packetHeader.type);

            if (packetHeader.length - kTerrariaPacketHeaderLength == 0)
            {
                /*
                 * If there is no packet payload then skip the rest of the read calls.
                 */
                return stagingBuffer;
            }

            // Array needs to grow to accomodate the rest of the packet.
            Array.Resize(ref stagingBuffer, packetHeader.length);

            int pos = 0;

            //The rest of the packet may require more than one read call.
            //Loop read calls until all the bytes have been read from the
            //stream into the buffer

            do
            {
                pos += stream.Read(stagingBuffer, kTerrariaPacketHeaderLength + pos, packetHeader.length - kTerrariaPacketHeaderLength - pos);
            } while (pos < packetHeader.length - kTerrariaPacketHeaderLength);

            lastBuffer = stagingBuffer;
            return stagingBuffer;
        }

        private TerrariaPacketHeader ParseHeader(byte[] buffer, int offset)
        {
            TerrariaPacketHeader header = new TerrariaPacketHeader();

            header.length = BitConverter.ToInt16(buffer, offset);

            if (Enum.IsDefined(typeof(PacketTypes), buffer[offset + 2]) == false)
            {
                throw new System.Exception($"Packet type {buffer[offset + 2]:X2} is unknown");
                //header.length = 0;
                //return header;
            }

            header.type = (PacketTypes)buffer[offset + 2];

            return header;
        }

    }
}