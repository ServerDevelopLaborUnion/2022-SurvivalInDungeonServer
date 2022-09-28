using System.Collections.Generic;
using Main.Util;
using System.Net;
using System.Net.Sockets;

namespace Main
{
    public partial class MainServer
    {
        private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        private Database _database;
        private Socket _serverSocket;

        public MainServer()
        {
            _database = new Database();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var types = this.GetType().Assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.GetInterface("IPakcetHandler") != null)
                {
                    var handler = (IPakcetHandler?)System.Activator.CreateInstance(type);
                    if (handler != null)
                        _packetHandlers.Add(handler.Type, handler);
                }
            }
        }

        public async Task StartUp()
        {
            await _database.Connect();
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Config.Server.Port));
            _serverSocket.Listen(100);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            _serverSocket.AcceptAsync(args);

            Logger.Info($"Server is running on port {Config.Server.Port}");

            while (_serverSocket.IsBound)
            {
                await Task.Delay(1000);
            }
        }

        private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (e.AcceptSocket != null)
                {
                    Session session = new(e.AcceptSocket);
                    Logger.Info($"New session connected: {session.Id}");
                }
            }

            e.AcceptSocket = null;
            _serverSocket.AcceptAsync(e);
        }

        public async Task Shutdown()
        {
            await _database.Disconnect();
            _serverSocket.Close();
        }
    }

    public partial class MainServer
    {
        public static MainServer Instance { get; private set; }
        public static Dictionary<string, Session> Sessions => Instance._sessions;
        public static Database Database => Instance._database;

        private static Dictionary<PacketType, IPakcetHandler> _packetHandlers = new();

        static MainServer()
        {
            Instance = new MainServer();
        }

        public static void HandlePacket(Session session, Packet packet)
        {
            if (_packetHandlers.TryGetValue(packet.Type, out IPakcetHandler? handler))
            {
                handler.Handle(session, packet);
            }
            else
            {
                Logger.Error($"Unhandled packet type: {packet.Type}");
            }
        }
        
        public static void Broadcast(Packet packet, Session? sender = null)
        {
            foreach (Session session in Sessions.Values)
            {
                if (sender != session)
                    session.SendPacket(packet);
            }
        }
    }
}
