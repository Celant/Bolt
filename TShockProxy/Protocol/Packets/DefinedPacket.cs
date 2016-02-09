//
//  BasePacket.cs
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
    public abstract class DefinedPacket
    {
        public static void WriteString(String s, IByteBuffer buf)
        {
            byte[] Bytes = Encoding.ASCII.GetBytes(s);
            Write7BitEncodedInt(Bytes.Length, buf);
            buf.WriteBytes(Bytes);
        }

        public static String ReadString(IByteBuffer buf)
        {
            int len = Read7BitEncodedInt(buf);

            byte[] b = new byte[len];
            buf.ReadBytes(b);

            return Encoding.ASCII.GetString(b);
        }

        public static void WriteInt(int value, IByteBuffer buf)
        {
            buf.WithOrder(ByteOrder.LittleEndian).WriteInt(value);
        }

        public static int ReadInt(IByteBuffer buf)
        {
            return buf.ReadInt();
        }

        public static void Write7BitEncodedInt(int value, IByteBuffer buf) {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint) value;   // support negative numbers
            while (v >= 0x80) {
                WriteByte((byte) (v | 0x80), buf);
                v >>= 7;
            }
            WriteByte((byte)v, buf);
        }

        public static int Read7BitEncodedInt(IByteBuffer buf) {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            do {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new FormatException("Failed to read byte array into a 7-Bit encoded int");

                // ReadByte handles end of stream cases for us.
                b = buf.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        public static void WriteShort(short value, IByteBuffer buf)
        {
            buf.WithOrder(ByteOrder.LittleEndian).WriteShort(value);
        }

        public static short ReadShort(IByteBuffer buf)
        {
            return buf.ReadShort();
        }

        public static void WriteByte(byte value, IByteBuffer buf)
        {
            buf.WithOrder(ByteOrder.LittleEndian).WriteByte(value);
        }

        public static byte ReadByte(IByteBuffer buf)
        {
            return buf.ReadByte();
        }

        public virtual void Read(IByteBuffer buf)
        {
            throw new NotImplementedException("Packet must implement read method");
        }

        public virtual void Write(IByteBuffer buf)
        {
            throw new NotImplementedException("Packet must implement write method");
        }

        public virtual void ReadPayload(IByteBuffer buf)
        {
            throw new NotImplementedException("Packet must implement read method");
        }

        public virtual void WritePayload(IByteBuffer buf)
        {
            throw new NotImplementedException("Packet must implement write method");
        }

    }
}

