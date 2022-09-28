using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public enum PacketType : uint
    {
        Ignore,
        Broadcast,
        AuthRequest,
        MoveRequrst,
        MoveResponse
    }

    public enum PacketState
    {
        None,
        Receiving,
        Complete,
    }

    public class Packet
    {
        public PacketType Type = PacketType.Ignore;
        public uint Length = 0;
        public byte[] Data = new byte[0];
        public int RemainPacket => (int)(Length - ReceivedLength);
        public PacketState State => Length == ReceivedLength ? PacketState.Complete : PacketState.Receiving;

        public int ReceivedLength = 0;
        public static Packet StartReceive(byte[] data, int length)
        {
            try
            {
                Packet packet = new();
                packet.Type = (PacketType)BitConverter.ToUInt32(data, 0);
                packet.Length = BitConverter.ToUInt32(data, 4);
                byte[] temp = BitConverter.GetBytes((uint)1);
                if (packet.Length > Config.Server.MaxPacketSize)
                {
                    throw new Exception("Packet size is too big");
                }
                packet.Data = new byte[packet.Length];
                Buffer.BlockCopy(data, 8, packet.Data, 0, length - 8);
                packet.ReceivedLength = length - 8;
                return packet;
            }
            catch
            {
                return new Packet();
            }
        }

        public Packet(PacketType type, byte[] data)
        {
            if (type == PacketType.Ignore)
                throw new Exception("Packet type can't be none");
            
            Type = type;
            Data = data;
            Length = (uint)data.Length;
        }

        public Packet()
        {
            Data = new byte[0];
            Length = 0;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((uint)Type));
            bytes.AddRange(BitConverter.GetBytes(Length));
            bytes.AddRange(Data);
            return bytes.ToArray();
        }

        public byte[]? ReceiveResume(byte[] data, int length)
        {
            if (State == PacketState.Complete) return data;

            if (length > RemainPacket)
            {
                Buffer.BlockCopy(data, 0, Data, (int)ReceivedLength, RemainPacket);
                ReceivedLength += RemainPacket;
                byte[] temp = new byte[length - RemainPacket];
                Buffer.BlockCopy(data, RemainPacket, temp, 0, length - RemainPacket);
                return temp;
            }
            else
            {
                Buffer.BlockCopy(data, 0, Data, (int)ReceivedLength, length);
                ReceivedLength += length;
                return null;
            }
        }
    }
}
