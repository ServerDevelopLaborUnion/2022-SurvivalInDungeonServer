using Main.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class Session
    {
        public Socket Socket;
        public string Id;

        public Account? Account;

        private SocketAsyncEventArgs _receiveArgs;

        private Packet _currentPacket = new();

        public Session(Socket socket)
        {
            Socket = socket;
            Id = Guid.NewGuid().ToString();

            _receiveArgs = new SocketAsyncEventArgs();
            _receiveArgs.Completed += OnReceiveCompleted;
            _receiveArgs.SetBuffer(new byte[Config.Server.BufferSize], 0, Config.Server.BufferSize);

            MainServer.Sessions.Add(Id, this);
            Socket.ReceiveAsync(_receiveArgs);
        }

        public void SendPacket(Packet packet)
        {
            byte[] data = packet.ToBytes();

            try
            {
                Socket.SendAsync(data, SocketFlags.None);
            }
            catch
            {
                Disconnect();
                Logger.Info($"Session disconnected: {Id}");
            }
        }

        private void Disconnect()
        {
            MainServer.Sessions.Remove(Id);
            Socket.Close();
        }

        private void OnReceiveCompleted(object? sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (e.BytesTransferred > 0)
                {
                    if (e.Buffer != null)
                    {
                        ReciveData(e.Buffer, e.BytesTransferred);
                    }
                }
            }

            _receiveArgs.SetBuffer(new byte[Config.Server.BufferSize], 0, Config.Server.BufferSize);
            Socket.ReceiveAsync(_receiveArgs);
        }

        private void ReciveData(byte[] data, int length)
        {
            if (_currentPacket.State == PacketState.None || _currentPacket.State == PacketState.Complete)
            {
                _currentPacket = Packet.StartReceive(data, length);

                if (_currentPacket.State == PacketState.Complete)
                {
                    HandlePacket(_currentPacket);
                }
            }
            else
            {
                var tempData = _currentPacket.ReceiveResume(data, length);

                if (tempData != null)
                {
                    _currentPacket = Packet.StartReceive(tempData, tempData.Length);
                }

                if (_currentPacket.State == PacketState.Complete)
                {
                    HandlePacket(_currentPacket);
                }
            }
        }

        private void HandlePacket(Packet packet)
        {
            if (packet.State != PacketState.Complete) return;
            Logger.Info($"Packet received: {packet.Type}, {packet.Data.Length} bytes from {Id}");

            
        }
    }
}
